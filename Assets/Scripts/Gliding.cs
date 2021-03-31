using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [Header("VariablesGlide")]
    [SerializeField]
    private float m_TrueSpeed = 15f;
    [SerializeField]
    private float m_TurnSmoothTime = 0.5f;
    private float m_TurnSmoothVelocity;
    //[SerializeField]
    //private float m_Gravity = -9.81f;
    [SerializeField]
    private AnimationCurve m_BeginVelocity = null;
    [SerializeField]
    private AnimationCurve m_EndVelocity = null;

    private Vector3 m_Velocity = Vector3.zero;

    private bool m_BeginGlide = false;

    private float m_BTimer = 0f;

    private float m_Speed = 0f;

    private Vector3 m_PastDirection = Vector3.zero;

    private bool m_IsGliding = false;

    private bool m_EndingGlide = false;

    private float m_ETimer = 0f;

    private WallGliding m_PlayerWallGliding = null;

    private void Awake()
    {
        m_PlayerWallGliding = GetComponent<WallGliding>();
    }

    private void Start()
    {
        m_Speed = m_TrueSpeed;
    }

    private void Update()
    {

    }

    public Vector3 Glide(Transform p_Camera, CharacterController p_Controller, Vector3 p_Direction)
    {
        Vector3 l_DirectionToReturn = Vector3.zero;

        if (p_Direction.magnitude >= 0.1f)
        {
            m_BeginGlide = true;
            float l_TargetAngle = Mathf.Atan2(p_Direction.x, p_Direction.z) * Mathf.Rad2Deg + p_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            if (!m_PlayerWallGliding.IsWallGliding()) transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 l_MoveDir;
            /*if (!m_PlayerWallGliding.IsWallGliding()) l_MoveDir = Quaternion.Euler(0f, l_Angle, 0f) * Vector3.forward;
            else l_MoveDir = Quaternion.Euler(0f, l_TargetAngle, 0f) * Vector3.forward;*/
            l_MoveDir = Quaternion.Euler(0f, l_Angle, 0f) * Vector3.forward;
            // p_Controller.Move(l_MoveDir.normalized * m_Speed * Time.deltaTime);
            l_DirectionToReturn = l_MoveDir.normalized * m_Speed;
            m_PastDirection = l_MoveDir;
            m_IsGliding = true;
        }
        else
        {
            m_BeginGlide = false;
            if (m_IsGliding)
            {
                m_EndingGlide = true;
                m_IsGliding = false;
            }
        }
        BeginGliding();
        EndGliding(p_Controller);

        m_PlayerWallGliding.WallGlidingUpdate(p_Controller);

        return l_DirectionToReturn;
    }

    private void BeginGliding()
    {
        if (m_BeginGlide)
        {
            m_EndingGlide = false;
            m_BTimer += 1 * Time.deltaTime;
            m_Speed = m_TrueSpeed * m_BeginVelocity.Evaluate(m_BTimer);
        }
        else
            m_BTimer = 0;
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
            m_Speed = m_TrueSpeed * m_EndVelocity.Evaluate(m_ETimer);
            p_Controller.Move(m_PastDirection.normalized * m_Speed * Time.deltaTime);
        }
    }

    public float GlidingTrueSpeed
    {
        get { return m_TrueSpeed; }
    }

    public float GlidingSpeed
    {
        get { return m_Speed; }
    }

    public Vector3 GlidingPastDirection
    {
        get { return m_PastDirection; }
    }

    public float GlideSpeed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }
}