using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnimIntro : MonoBehaviour
{
    [SerializeField] Image m_FadeWhiteIn;
    [SerializeField] GameObject m_Logo;
    SceneLoader m_SceneLoader;

    private void Start()
    {
        m_SceneLoader = GetComponent<SceneLoader>();
        m_FadeWhiteIn.CrossFadeAlpha(0, 0, true);
        m_FadeWhiteIn.CrossFadeAlpha(1, 2, true);
        StartCoroutine(IntroCo());
    }

    IEnumerator IntroCo()
    {
        yield return new WaitForSeconds(2f);

        m_Logo.transform.DOMoveY(600, 0.4f);

        yield return new WaitForSeconds(0.5f);

        m_Logo.transform.DOPunchRotation(Vector3.one * 5f, 0.5f);
        m_Logo.transform.DOPunchScale(Vector3.one * 0.25f, 0.5f);

        yield return new WaitForSeconds(2);

        m_Logo.GetComponent<Image>().CrossFadeAlpha(0, 0.5f, true);

        yield return new WaitForSeconds(0.5f);

        m_SceneLoader.LoadLevel("MainMenu");
    }
}
