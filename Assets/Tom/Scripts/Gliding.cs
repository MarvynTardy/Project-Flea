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
    [SerializeField]
    private float m_Gravity = -9.81f;
    [SerializeField]
    private AnimationCurve m_BeginVelocity = null;
    [SerializeField]
    private AnimationCurve m_EndVelocity = null;

    private Vector3 m_Velocity = Vector3.zero;

    private bool m_BeginGlide = false;

    private float m_BTimer = 0f;

    private float m_Speed = 0f;

    private void Awake()
    {
        m_Speed = m_TrueSpeed;
    }

    private void Update()
    {
        BeginGliding();
    }

    public void Glide(Transform p_Camera, CharacterController p_Controller)
    {
        float l_Horizontal = Input.GetAxisRaw("Horizontal");
        float l_Vertical = Input.GetAxisRaw("Vertical");
        Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

        if (l_Direction.magnitude >= 0.1f)
        {
            m_BeginGlide = true;
            float l_TargetAngle = Mathf.Atan2(l_Direction.x, l_Direction.z) * Mathf.Rad2Deg + p_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 l_MoveDir = Quaternion.Euler(0f, l_Angle, 0f) * Vector3.forward;
            p_Controller.Move(l_MoveDir.normalized * m_Speed * Time.deltaTime);
        }
        else
            m_BeginGlide = false;
    }

    private void BeginGliding()
    {
        if (m_BeginGlide)
        {
            m_BTimer += 1 * Time.deltaTime;
            Debug.Log(m_Speed);
            m_Speed = m_TrueSpeed * m_BeginVelocity.Evaluate(m_BTimer);
            /*Debug.Log("Timer : " + m_BTimer);
            Debug.Log("Curve : " + m_BeginVelocity.Evaluate(m_BTimer));
            Debug.Log("Speed : " + m_Speed);*/
        }
        else
            m_BTimer = 0;
    }
}