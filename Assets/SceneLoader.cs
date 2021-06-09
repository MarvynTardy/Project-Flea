using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadLevel(string p_SceneToLoad)
    {
        SceneManager.LoadScene(p_SceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
