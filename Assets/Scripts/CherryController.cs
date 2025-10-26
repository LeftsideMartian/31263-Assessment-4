using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CherryController : MonoBehaviour
{
    private Transform cherryTransform;
    private Tweener tweener;
    private SpriteRenderer spriteRenderer;

    private float newCherryTimer;
    private Vector3 destination;
    private Vector3 centrePoint = new(13.5f, -14f, 0);
    private float centreRadius = 17.5f;

    public float spawnTime = 5;
    public float speed = 1;

    private void Start()
    {
        cherryTransform = gameObject.transform;
        tweener = gameObject.GetComponent<Tweener>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (newCherryTimer >= spawnTime)
        {
            newCherryTimer = 0;

            SpawnCherry();
            SelectDestination();
            MoveCherry();
        }

        if (!tweener.IsTweenActive())
        {
            spriteRenderer.enabled = false;
            newCherryTimer += Time.deltaTime;
        }
    }

    private void SpawnCherry()
    {
        float xMin = -4.0f, xMax = 31f;
        float yMin = 3.5f, yMax = -31.5f;
        float margin = 0.2f;

        int side = Random.Range(0, 4); // 0=left, 1=right, 2=bottom, 3=top
        float x, y;

        switch (side)
        {
            case 0: // left
                x = xMin - margin;
                y = Random.Range(yMin, yMax);
                break;
            case 1: // right
                x = xMax + margin;
                y = Random.Range(yMin, yMax);
                break;
            case 2: // bottom
                x = Random.Range(xMin, xMax);
                y = yMin - margin;
                break;
            default: // top
                x = Random.Range(xMin, xMax);
                y = yMax + margin;
                break;
        }

        Vector3 spawnPoint = new(x, y);

        cherryTransform.position = spawnPoint;
        spriteRenderer.enabled = true;
    }

    private void SelectDestination()
    {
        destination = centrePoint - (cherryTransform.position - centrePoint);
    }

    private void MoveCherry()
    {
        float lerpDuration = 2 / speed;

        tweener.SetActiveTween(new Tween(
            cherryTransform,
            cherryTransform.position,
            new Vector3(destination.x, destination.y, 0),
            Time.time,
            lerpDuration
        ));
    }
}
