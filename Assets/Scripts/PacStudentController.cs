using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    // Animations
    private static string IDLE_UP = "Idle_up";
    private static string IDLE_DOWN = "Idle_down";
    private static string IDLE_SIDE = "Idle_side";
    private static string WALK_UP = "Walk_up";
    private static string WALK_DOWN = "Walk_down";
    private static string WALK_SIDE = "Walk_side";

    private string currentAnimationState = IDLE_SIDE;
    
    // Components
    private Animator pacStudentAnimator;
    private Transform pacStudentTransform;
    private ParticleSystem pacStudentParticleSystem;
    private Tweener tweener;

    private bool isMoving;
    private KeyCode currentInput;
    private KeyCode lastInput;

    private float lastSteppingSoundTime;

    private int[] walkableTileTypes = { 0, 5, 6 };
    public float speed = 5f;
    public float steppingSoundInterval = 1.0f;

    void Start()
    {
        pacStudentAnimator = gameObject.GetComponent<Animator>();
        pacStudentTransform = gameObject.transform;
        pacStudentParticleSystem = gameObject.GetComponent<ParticleSystem>();
        tweener = gameObject.GetComponent<Tweener>();
    }

    void Update()
    {
        // Gather input
        GetInput();
        
        // If currently lerping, exit
        if (tweener.IsTweenActive())
        {
            return;
        }

        if (IsTileWalkable(lastInput))
        {
            UpdateCurrentInput();
            MovePlayer(lastInput);
        
        } else if (IsTileWalkable(currentInput))
        {
            MovePlayer(currentInput);
        }
        else
        {
            isMoving = false;
        }
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            lastInput = KeyCode.W;
        } else if (Input.GetKeyDown(KeyCode.A))
        {
            lastInput = KeyCode.A;
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            lastInput = KeyCode.S;
        } else if (Input.GetKeyDown(KeyCode.D))
        {
            lastInput = KeyCode.D;
        }
    }

    private void UpdateCurrentInput()
    {
        if (lastInput == KeyCode.W)
        {
            currentInput = KeyCode.W;
        } else if (lastInput == KeyCode.A)
        {
            currentInput = KeyCode.A;
        } else if (lastInput == KeyCode.S)
        {
            currentInput = KeyCode.S;
        } else if (lastInput == KeyCode.D)
        {
            currentInput = KeyCode.D;
        }
    }
    
    private bool IsTileWalkable(KeyCode input)
    {
        if (input == KeyCode.None)
        {
            return false;
        }

        Vector2Int tileToCheck = GetNextTileCoords(input);

        int tileType = LevelGenerator.Instance.levelMapList[-tileToCheck.y][tileToCheck.x];

        foreach (int walkableTileType in walkableTileTypes)
        {
            if (tileType == walkableTileType)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2Int GetNextTileCoords(KeyCode input)
    {
        float moveX = 0.0f;
        float moveY = 0.0f;

        switch (input)
        {
            case KeyCode.W:
                moveY = 1.0f;
                break;
            case KeyCode.A:
                moveX = -1.0f;
                break;
            case KeyCode.S:
                moveY = -1.0f;
                break;
            case KeyCode.D:
                moveX = 1.0f;
                break;
        }

        return new Vector2Int((int)(pacStudentTransform.position.x + moveX), (int)(pacStudentTransform.position.y + moveY));
    }

    private void MovePlayer(KeyCode input)
    {
        isMoving = true;

        Vector2Int newCoords = GetNextTileCoords(input);

        float lerpDuration = 2 / speed;

        tweener.SetActiveTween(new Tween(
            pacStudentTransform,
            pacStudentTransform.position,
            new Vector3(newCoords.x, newCoords.y, 0),
            Time.time,
            lerpDuration
            ));

        HandleAnimations(currentInput);
        HandleAudio();
        HandleParticles();
    }

    private void UpdateAnimationState(KeyCode input)
    {
        if (isMoving)
        {
            switch (input)
            {
                case KeyCode.W:
                    currentAnimationState = WALK_UP;
                    break;
                case KeyCode.A:
                    currentAnimationState = WALK_SIDE;
                    break;
                case KeyCode.S:
                    currentAnimationState = WALK_DOWN;
                    break;
                case KeyCode.D:
                    currentAnimationState = WALK_SIDE;
                    break;
            }
        } else
        {
            switch (input)
            {
                case KeyCode.W:
                    currentAnimationState = IDLE_UP;
                    break;
                case KeyCode.A:
                    currentAnimationState = IDLE_SIDE;
                    break;
                case KeyCode.S:
                    currentAnimationState = IDLE_DOWN;
                    break;
                case KeyCode.D:
                    currentAnimationState = IDLE_SIDE;
                    break;
            }
        }
    }
    
    private void HandleAnimations(KeyCode input)
    {
        UpdateAnimationState(input);
        pacStudentAnimator.Play(currentAnimationState);

        // If moving left, flip horizontally
        if (input == KeyCode.A)
        {
            Vector3 transformScale = pacStudentTransform.localScale;
            transformScale.x = -1;
            pacStudentTransform.localScale = transformScale;
        } else
        {
            Vector3 transformScale = pacStudentTransform.localScale;
            transformScale.x = 1;
            pacStudentTransform.localScale = transformScale;
        }
    }

    private void HandleAudio()
    {
        if (AudioController.Instance == null)
        {
            return;
        }
        
        if (!isMoving)
        {
            return;
        }

        if (Time.time - lastSteppingSoundTime >= steppingSoundInterval)
        {
            AudioController.Instance.PlaySoundEffect(AudioController.AudioAssetType.PacWalking);
            lastSteppingSoundTime = Time.time;
        }
    }

    private void HandleParticles()
    {
        var emission = pacStudentParticleSystem.emission;
        emission.enabled = isMoving;
    }
}
