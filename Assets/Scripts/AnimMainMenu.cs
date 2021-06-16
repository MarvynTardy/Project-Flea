using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnimMainMenu : MonoBehaviour
{
    public Image m_FadeWhiteIn;

    private void Awake()
    {
        m_FadeWhiteIn.gameObject.SetActive(true);
    }

    private void Start()
    {
        StartCoroutine(IntroCo());
    }

    IEnumerator IntroCo()
    {
        yield return new WaitForSeconds(0.5f);

        m_FadeWhiteIn.CrossFadeAlpha(0, 2, true);

        yield return new WaitForSeconds(2f);

        m_FadeWhiteIn.gameObject.SetActive(false);
    }
}
