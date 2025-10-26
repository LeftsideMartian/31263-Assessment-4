using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1UIManager : MonoBehaviour
{
    public void ExitLevel()
    {
        LoadManager.Instance.LoadStartScene();
    }
}
