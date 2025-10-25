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
        PacDeath,
        Punch
    }
    
    [Serializable]
    private class AudioAsset
    {
        public AudioAssetType type;
        public AudioClip audioClip;
    }
    
    [SerializeField] private AudioSource mainAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;
    [SerializeField] private AudioAsset[] audioAssets;

    private void Awake()
    {
        ManagerFinder.AudioController = this;
    }

    private void Start()
    {
        ChangeBGM(AudioAssetType.IntroBGM);
    }

    public void ChangeBGM(AudioAssetType type)
    {
        mainAudioSource.clip = GetAudioClip(type);

        if (!mainAudioSource.isPlaying)
        {
            mainAudioSource.Play();
        }
    }

    public void PlaySoundEffect(AudioAssetType type)
    {
        soundEffectAudioSource.PlayOneShot(GetAudioClip(type));
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
