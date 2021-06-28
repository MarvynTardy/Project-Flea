using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewpointManager : MonoBehaviour
{
    public bool[] m_ActivationList = new bool[] { false, false, false };
    public MeshRenderer[] m_GlypheFeedback;
    private ControllerFinal m_Controller;
    [SerializeField] private Transform m_TargetPlayer;
    private bool m_MovePlayer;
    public Material m_GlowingMaterial;
    [SerializeField] private Image m_FadeToWhite;
    [SerializeField] private Text m_TextThanks;
    [SerializeField] private Text m_TextTeam;


    // public Dictionary<int, ViewpointBehaviour> m_ViewPoints = new Dictionary<int, ViewpointBehaviour>();
    // public ViewpointBehaviour[] m_ViewPointsList;


    //private void Start()
    //{
    //    m_ViewPointsList = FindObjectsOfType<ViewpointBehaviour>();
    //}
    private void Awake()
    {
        m_Controller = FindObjectOfType<ControllerFinal>();
    }

    //public void AddValidViewPoints()
    //{
    //    for (int i = 0; i < m_ViewPointsList.Length; i++)
    //    {
    //        if (m_ViewPointsList[i].m_IsExit)
    //            m_ActivationList[i] = true;
    //    }
    //}

    public void AddValidViewPointsOpti(int p_ViewPointID)
    {
        m_ActivationList[p_ViewPointID] = true;
        m_GlypheFeedback[p_ViewPointID].material = m_GlowingMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(SequenceFinal());
    }

    IEnumerator SequenceFinal()
    {
        m_Controller.m_CanInteract = false;

        m_MovePlayer = true;

        while (m_MovePlayer)
        {
            PlayerMoveForward();

            yield return null;
        }

        //m_FadeToWhite.CrossFadeAlpha(0, 0, true);
        //m_FadeToWhite.CrossFadeAlpha(1, 3, true);

        //yield return new WaitForSeconds(3);

        m_TextThanks.gameObject.SetActive(true);
        m_TextThanks.CrossFadeAlpha(0, 0, true);
        m_TextThanks.CrossFadeAlpha(1, 1, true);

        yield return new WaitForSeconds(2);

        m_TextTeam.gameObject.SetActive(true);
        m_TextTeam.CrossFadeAlpha(0, 0, true);
        m_TextTeam.CrossFadeAlpha(1, 1, true);

    }

    private void PlayerMoveForward()
    {
        Vector3 m_Direction;

        m_Direction = Vector3.Lerp(m_Controller.transform.position, m_TargetPlayer.position, 0.005f);

        m_Controller.m_PlayerAnim.SetBool("IsMoving", true);
        m_Controller.m_PlayerAnim.SetBool("IsGrounded", true);

        m_Direction.y = m_TargetPlayer.transform.position.y + 1.12f;

        m_Controller.transform.position = m_Direction;

        // Si le joueur a atteint la target
        if (Vector3.Distance(m_TargetPlayer.position, m_Controller.transform.position) < 2.59f)
        {
            m_Controller.m_PlayerAnim.SetBool("IsMoving", false);
            m_Controller.m_PlayerAnim.SetBool("IsInactive", true);
            m_MovePlayer = false;
        }
    }
}
