using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnSystem : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private Vector3 m_Checkpoint = new Vector3(0, 1, 0);
    private ControllerFinal m_ControllerPlayer;
    private bool m_CanCheck = false;

    [Header("Feedback")]
    [SerializeField] private Image m_BlackScreen;
    private Animator m_PlayerAnim = null;

    private void Awake()
    {
        m_Checkpoint = transform.position;
        m_ControllerPlayer = GetComponent<ControllerFinal>();
        m_PlayerAnim = GetComponentInChildren<Animator>();

        if (m_BlackScreen)
        {
            m_BlackScreen.gameObject.SetActive(true);
            m_BlackScreen.CrossFadeAlpha(0, 0.5f, false);
        }

        m_CanCheck = true;
    }

    private void Update()
    {
    //    if (Input.GetKeyDown(KeyCode.A))
    //        RespawnBegin();

        if (m_CanCheck && m_ControllerPlayer.m_CanInteract)
        {
            CheckValidPoint();
        }
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

        // fade au noir à été executé
        m_ControllerPlayer.ResetGravityEffect();

        this.gameObject.transform.position = m_Checkpoint;
        // m_PlayerAnim.SetBool("IsInactive", true);
        m_PlayerAnim.SetTrigger("IsRespawn");

        yield return new WaitForSeconds(0.2f);

        // Respawn
        if (m_ControllerPlayer.m_SpiritMode)
            m_ControllerPlayer.SpiritRelease();
        
        if (m_BlackScreen)
            m_BlackScreen.CrossFadeAlpha(0, 3, false);

        yield return new WaitForSeconds(2);

        RespawnExit();
    }

    private void RespawnExit()
    {
        m_ControllerPlayer.m_CanInteract = true;
        // StartCoroutine(m_ControllerPlayer.IdleFeedbackAFKReleaseCO());
        m_PlayerAnim.SetBool("IsInactive", false);
    }

    private void CheckValidPoint()
    {
        m_CanCheck = false;

        RaycastHit l_HitPoint;

        if (Physics.Raycast(m_ControllerPlayer.transform.position, m_ControllerPlayer.transform.TransformDirection(Vector3.down), out l_HitPoint, 3, m_ControllerPlayer.m_GroundMask))
        {
            // Si le personnage est bien au sol
            if (m_ControllerPlayer.m_IsGrounded && !m_ControllerPlayer.m_IsSliding)
            {
                m_Checkpoint = new Vector3(l_HitPoint.point.x, l_HitPoint.point.y + 1.05f, l_HitPoint.point.z);
            }
        }

        StartCoroutine(CheckValidPointCO());
    }

    IEnumerator CheckValidPointCO()
    {
        yield return new WaitForSeconds(1);

        m_CanCheck = true;
    }
}
