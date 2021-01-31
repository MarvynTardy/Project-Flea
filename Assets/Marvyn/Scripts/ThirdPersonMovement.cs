using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharacterController m_Controller;
    [SerializeField]
    private Transform m_Camera;
    [SerializeField]
    private Transform m_Groundcheck = null;
    [SerializeField]
    private LayerMask m_GroundMask;

    [Header("Variables")]
    [SerializeField]
    private float m_Speed = 6f;
    private float m_TurnSmoothTime = 0.5f;
    private float m_TurnSmoothVelocity;

    Vector3 m_Velocity = Vector3.zero;
    [SerializeField]
    private float m_Gravity = -9.81f;

    [SerializeField]
    private float m_GroundDistance = 0.4f;

    [SerializeField]
    private float m_JumpHeight = 3f;

    bool isGrounded;

    private Gliding m_PlayerGliding = null;
    private Walking m_PlayerWalking = null;
     
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_PlayerGliding = GetComponent<Gliding>();
        m_PlayerWalking = GetComponent<Walking>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(m_Groundcheck.position, m_GroundDistance, m_GroundMask);

        if (isGrounded && m_Velocity.y < 0)
        {
            m_Velocity.y = -2f;
        }

        Move();

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2f * m_Gravity);
        }
    }

    private void Move()
    {
        /*float l_Horizontal = Input.GetAxisRaw("Horizontal");
        float l_Vertical = Input.GetAxisRaw("Vertical");
        Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;
        
        if(l_Direction.magnitude >= 0.1f)
        {
            float l_TargetAngle = Mathf.Atan2(l_Direction.x, l_Direction.z) * Mathf.Rad2Deg + m_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 l_MoveDir = Quaternion.Euler(0f, l_Angle, 0f) * Vector3.forward;
            m_Controller.Move(l_MoveDir.normalized * m_Speed * Time.deltaTime);
        }*/
        if (Input.GetKey(KeyCode.LeftShift))
            m_PlayerGliding.Glide(m_Camera, m_Controller);
        else
            m_PlayerWalking.Walk(m_Camera, m_Controller);

        m_Velocity.y += m_Gravity * Time.deltaTime;

        m_Controller.Move(m_Velocity * Time.deltaTime);
    }
}