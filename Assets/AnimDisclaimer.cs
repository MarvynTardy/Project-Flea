using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnimDisclaimer : MonoBehaviour
{
    [SerializeField] Text m_TextToDisplay;
    [SerializeField] string m_LevelToLoad;
    SceneLoader m_SceneLoader;
    

    private void Start()
    {
        m_SceneLoader = GetComponent<SceneLoader>();
        m_TextToDisplay.CrossFadeAlpha(0, 0, true);
        m_TextToDisplay.CrossFadeAlpha(1, 2, true);
        StartCoroutine(IntroCo());
    }

    IEnumerator IntroCo()
    {
        yield return new WaitForSeconds(5f);

        m_TextToDisplay.CrossFadeAlpha(0, 2, true);

        yield return new WaitForSeconds(2f);
        m_SceneLoader.LoadLevel(m_LevelToLoad);
    }
}
