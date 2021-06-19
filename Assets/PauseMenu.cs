using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_PauseCanvas;
    [SerializeField] Image m_FadeWhiteOut;
    private bool m_IsPaused = false;
    private ControllerFinal m_Controller;

    private void Start()
    {
        m_Controller = FindObjectOfType<ControllerFinal>();
        m_PauseCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (m_IsPaused)
                UnpauseGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        m_IsPaused = true;
        m_PauseCanvas.SetActive(true);
        m_Controller.m_CanInteract = false;
    }
    
    public void UnpauseGame()
    {
        m_IsPaused = false;
        m_PauseCanvas.SetActive(false);
        m_Controller.m_CanInteract = true;
    }

    public void QuitGame()
    {
        StartCoroutine(ExitMenuCO());
    }

    IEnumerator ExitMenuCO()
    {
        yield return new WaitForSeconds(0.5f);

        m_FadeWhiteOut.gameObject.SetActive(true);
        m_FadeWhiteOut.CrossFadeAlpha(0, 0, true);
        m_FadeWhiteOut.CrossFadeAlpha(1, 4, true);

        yield return new WaitForSeconds(5);

        FindObjectOfType<SceneLoader>().LoadLevel("MainMenu");
    }
}
