using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private int startSceneBuildIndex = 0;
    private int level1BuildIndex = 1;

    public static LoadManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(startSceneBuildIndex);
        AudioController.Instance.ChangeBGM(AudioController.AudioAssetType.IntroBGM);
        AudioController.Instance.SoundEffectVolume = 0.2f;
    }

    public void LoadLevel1()
    {
        GameObject globalManagers = GameObject.FindWithTag("GlobalManager");
        DontDestroyOnLoad(globalManagers);

        SceneManager.LoadSceneAsync(level1BuildIndex);
        AudioController.Instance.ChangeBGM(AudioController.AudioAssetType.GhostNormalBGM);
        AudioController.Instance.SoundEffectVolume = 0.6f;
    }
}
