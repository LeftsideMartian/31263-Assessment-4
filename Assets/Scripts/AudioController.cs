using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public enum AudioAssetType
    {
        IntroBGM,
        StartMusic,
        GhostNormalBGM,
        GhostScaredBGM,
        GhostDeadBGM,
        PacWalking,
        EatPellet,
        WallCollide,
        PacDeath
    }
    
    [Serializable]
    private class AudioAsset
    {
        public AudioAssetType type;
        public AudioClip audioClip;
    }
    
    private AudioSource audioSource;
    [SerializeField] private AudioSource pacStudentAudioSource;
    [SerializeField] private AudioAsset[] audioAssets;
    private float timeOfLastSteppingSoundEffect = 0.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ChangeBGM(AudioAssetType.IntroBGM);
    }

    private void Update()
    {
        // If 3 seconds pass OR the audio clip finishes playing
        if (Time.time >= 3.0f || !audioSource.isPlaying)
        {
            ChangeBGM(AudioAssetType.GhostNormalBGM);
        }
    }

    public void ChangeBGM(AudioAssetType type)
    {
        audioSource.clip = GetAudioClip(type);

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PlaySoundEffect(AudioAssetType type)
    {
        pacStudentAudioSource.PlayOneShot(GetAudioClip(type));
    } 
    
    private AudioClip GetAudioClip(AudioAssetType type)
    {
        foreach (AudioAsset audioAsset in audioAssets)
        {
            if (audioAsset.type == type)
            {
                return audioAsset.audioClip;
            }
        }

        return null;
    }
}
