using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumAfterMovement : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_WalkEndVelocity = null;
    [SerializeField] private AnimationCurve m_GlideEndVelocity = null;
    private bool m_EndingWalk = false;
    private bool m_EndingGlide = false;
    private float m_ETimer = 0f;

    private Walking m_PlayerWalking;
    private Gliding m_PlayerGliding;

    private bool m_IsGliding = false;
    private bool m_IsWalking = false;
    private bool m_IsMoving = false;

    private float m_Speed = 0f;

    private void Awake()
    {
        m_PlayerWalking = GetComponent<Walking>();
        m_PlayerGliding = GetComponent<Gliding>();
    }

    private void Update()
    {
        if (!m_IsGliding && !m_IsWalking)
            m_IsMoving = false;
        else
            m_IsMoving = true;
    }

    private void EndWalking(CharacterController p_Controller)
    {
        if (m_PlayerWalking.WalkingSpeed == 0)
        {
            m_EndingWalk = false;
            m_ETimer = 0;
        }
        if (m_EndingWalk)
        {
            m_ETimer += 1 * Time.deltaTime;
            m_Speed = m_PlayerWalking.WalkingTrueSpeed * m_WalkEndVelocity.Evaluate(m_ETimer);
            p_Controller.Move(m_PlayerWalking.WalkingPastDirection.normalized * m_Speed * Time.deltaTime);
        }
    }

    private void EndGliding(CharacterController p_Controller)
    {
        if (m_Speed == 0)
        {
            m_EndingGlide = false;
            m_ETimer = 0;
        }
        if (m_EndingGlide)
        {
            m_ETimer += 1 * Time.deltaTime;
            m_Speed = m_PlayerGliding.GlidingTrueSpeed * m_GlideEndVelocity.Evaluate(m_ETimer);
            p_Controller.Move(m_PlayerGliding.GlidingPastDirection.normalized * m_Speed * Time.deltaTime);
        }
    }

    public bool IsWalking
    {
        get { return m_IsWalking; }
        set { m_IsWalking = value; }
    }

    public bool IsGliding
    {
        get { return m_IsGliding; }
        set { m_IsGliding = value; }
    }
}