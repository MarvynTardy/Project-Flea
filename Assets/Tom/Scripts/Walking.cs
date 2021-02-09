using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour
{
    [Header("VariablesWalk")]
    [SerializeField]
    private float m_TrueSpeed = 6f;
    [SerializeField]
    private float m_TurnSmoothTime = 0.5f;
    [SerializeField]
    private float m_TurnSmoothVelocity;
    //[SerializeField]
    //private float m_Gravity = -9.81f;
    [SerializeField]
    private AnimationCurve m_BeginVelocity = null;
    [SerializeField]
    private AnimationCurve m_EndVelocity = null;

    Vector3 m_Velocity = Vector3.zero;

    private bool m_BeginWalk = false;

    private float m_BTimer = 0f;

    private float m_Speed = 0f;

    private Vector3 m_PastDirection = Vector3.zero;

    private bool m_IsWalking = false;

    private bool m_EndingWalk = false;

    private float m_ETimer = 0f;

    private void Awake()
    {
        m_Speed = m_TrueSpeed;
    }

    private void Update()
    {
        // Debug.Log(m_IsWalking);
    }

    public Vector3 Walk(Transform p_Camera, CharacterController p_Controller, Vector3 p_Direction)
    {
        //float l_Horizontal = Input.GetAxisRaw("Horizontal");
        //float l_Vertical = Input.GetAxisRaw("Vertical");
        //Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

        if (p_Direction.magnitude >= 0.1f)
        {
            m_BeginWalk = true;
            float l_TargetAngle = Mathf.Atan2(p_Direction.x, p_Direction.z) * Mathf.Rad2Deg + p_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 l_MoveDir = Quaternion.Euler(0f, l_TargetAngle, 0f) * Vector3.forward;
            p_Controller.Move(l_MoveDir.normalized * m_Speed * Time.deltaTime);
            m_PastDirection = l_MoveDir;
            m_IsWalking = true;
        }
        else
        {
            m_BeginWalk = false;
            if (m_IsWalking)
            {
                m_EndingWalk = true;
                m_IsWalking = false;
            }
        }
        BeginWalking();
        EndWalking(p_Controller);

        return p_Direction;
    }

    private void BeginWalking()
    {
        if (m_BeginWalk)
        {
            m_EndingWalk = false;
            m_BTimer += 1 * Time.deltaTime;
            m_Speed = m_TrueSpeed * m_BeginVelocity.Evaluate(m_BTimer);
        }
        else
            m_BTimer = 0;
    }

    private void EndWalking(CharacterController p_Controller)
    {
        if (m_Speed == 0)
        {
            m_EndingWalk = false;
            m_ETimer = 0;
        }
        if (m_EndingWalk)
        {
            m_ETimer += 1 * Time.deltaTime;
            m_Speed = m_TrueSpeed * m_EndVelocity.Evaluate(m_ETimer);
            p_Controller.Move(m_PastDirection.normalized * m_Speed * Time.deltaTime);
        }
    }
}