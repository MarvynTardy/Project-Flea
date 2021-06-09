using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenManager : MonoBehaviour
{
    public GameObject[] m_TutoScreens;

    private void Awake()
    {
        foreach (var l_TutoScreen in m_TutoScreens)
        {
            l_TutoScreen.SetActive(true);
            l_TutoScreen.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void ShowTutorialScreen(int p_ID)
    {
        // Juste une sécurité pour prévenir d'un potentiel bug
        if (p_ID <= m_TutoScreens.Length)
        {
            // Le p_ID - 1 permet de renvoyer l'élément correspondant à la bonne valeur (ex : p_ID = 1 renverra le premier élément du tableau)
            CanvasGroup l_ImageToFade = m_TutoScreens[p_ID - 1].GetComponent<CanvasGroup>();

            StartCoroutine(FadeAlpha(l_ImageToFade, l_ImageToFade.alpha, 1));
        }
        else
            Debug.Log("L'index " + p_ID + " n'est pas référencé dans la liste des écrans de tutoriels");
    }
    
    public void HideTutorialScreen(int p_ID)
    {
        if (p_ID <= m_TutoScreens.Length)
        {
            CanvasGroup l_ImageToFade = m_TutoScreens[p_ID - 1].GetComponent<CanvasGroup>();

            StartCoroutine(FadeAlpha(l_ImageToFade, l_ImageToFade.alpha, 0));
        }
    }

    IEnumerator FadeAlpha(CanvasGroup p_ImageRef, float p_StartAlpha, float p_EndAlpha, float p_LerpTime = 0.5f)
    {
        float l_TimeStartedLerping = Time.time;
        float l_TimeSinceStarted = Time.time - l_TimeStartedLerping;
        float l_PercentageComplete = l_TimeSinceStarted / p_LerpTime;

        while(true)
        {
            l_TimeSinceStarted = Time.time - l_TimeStartedLerping;
            l_PercentageComplete = l_TimeSinceStarted / p_LerpTime;

            float l_CurrentValue = Mathf.Lerp(p_StartAlpha, p_EndAlpha, l_PercentageComplete);

            p_ImageRef.alpha = l_CurrentValue;

            if (l_PercentageComplete >= 1) break;

            yield return null;
        }

        // Debug.Log("Fade done");
    }
}
