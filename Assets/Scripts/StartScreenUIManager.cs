using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StartScreenUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform punchZone;
    [SerializeField] private GameObject pelletPrefab;
    private Rect punchZoneRect;
    private GameObject pellet;
    private float timeOfLastPellet;
    public float timeBetweenPelletSpawns = 0.2f;

    void Update()
    {
        punchZoneRect = punchZone.rect;

        bool isTooSoonForNewPellet = Time.time - timeOfLastPellet < timeBetweenPelletSpawns;
        if (isTooSoonForNewPellet)
        {
            return;
        }

        // Pick a random point inside punchZone
        Vector2 randomPoint = new Vector2(
            Random.Range(punchZoneRect.xMin, punchZoneRect.xMax),
            Random.Range(punchZoneRect.yMin, punchZoneRect.yMax )
            );

        // Place a pelletPrefab
        if (pellet == null)
        {
            pellet = Instantiate(pelletPrefab, punchZone, true);
        }
        pellet.transform.position = punchZone.TransformPoint(randomPoint);
        
        AudioController.Instance.PlaySoundEffect(AudioController.AudioAssetType.Punch);

        timeOfLastPellet = Time.time;
    }

    public void HandleLevel1ButtonClick()
    {
        LoadManager.Instance.LoadLevel1();
    }
}
