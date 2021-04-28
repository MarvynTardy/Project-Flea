using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] private Vector3 m_Checkpoint = new Vector3(0, 1, 0);
    private ControllerFinal m_ControllerPlayer;

    [Header("Feedback")]
    [SerializeField] private Image m_BlackScreen;
    private Animator m_PlayerAnim = null;

    private void Awake()
    {
        m_ControllerPlayer = GetComponent<ControllerFinal>();
        m_PlayerAnim = GetComponentInChildren<Animator>();

        if (m_BlackScreen)
        {
            m_BlackScreen.gameObject.SetActive(true);
            m_BlackScreen.CrossFadeAlpha(0, 0.5f, false);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //    RespawnBegin();
    }

    public void RespawnBegin()
    {
        m_ControllerPlayer.m_CanInteract = false;

        if (m_BlackScreen)
            m_BlackScreen.CrossFadeAlpha(1, 1, false);

        StartCoroutine(RespawnCO());
    }

    IEnumerator RespawnCO()
    {
        yield return new WaitForSeconds(1);

        this.gameObject.transform.position = m_Checkpoint;
        m_PlayerAnim.SetBool("IsRespawn", true);

        yield return new WaitForSeconds(0.2f);

        if (m_BlackScreen)
            m_BlackScreen.CrossFadeAlpha(0, 3, false);

        yield return new WaitForSeconds(2);

        RespawnExit();
    }

    private void RespawnExit()
    {
        m_ControllerPlayer.m_CanInteract = true;
        m_PlayerAnim.SetBool("IsRespawn", false);
    }
}
