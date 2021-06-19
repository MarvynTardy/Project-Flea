using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitMainMenu : MonoBehaviour
{
    public Image m_FadeBlackOut;

    private void Start()
    {
        m_FadeBlackOut.gameObject.SetActive(false);
    }

    public void ExitMenu(string p_NextScene)
    {
        StartCoroutine(ExitMenuCO(p_NextScene));
    }

    IEnumerator ExitMenuCO(string p_NextScene)
    {
        m_FadeBlackOut.gameObject.SetActive(true);
        m_FadeBlackOut.CrossFadeAlpha(0, 0, true);
        m_FadeBlackOut.CrossFadeAlpha(1, 2, true);

        yield return new WaitForSeconds(3);

        GetComponent<SceneLoader>().LoadLevel(p_NextScene);
    }
}
