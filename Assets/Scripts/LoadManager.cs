using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private int level1BuildIndex = 1;

    public void LoadLevel1()
    {
        GameObject globalManagers = GameObject.FindWithTag("GlobalManager");
        DontDestroyOnLoad(globalManagers);

        ManagerFinder.AudioController.ChangeBGM(AudioController.AudioAssetType.IntroBGM);
        SceneManager.LoadScene(level1BuildIndex);
        
        // Change music
    }
}
