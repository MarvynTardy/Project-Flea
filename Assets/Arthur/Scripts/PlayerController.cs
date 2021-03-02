using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController m_Controller;
    [SerializeField] private Transform m_Camera;
    [SerializeField] private Transform m_Groundcheck = null;
    [SerializeField] private LayerMask m_GroundMask;
    [SerializeField] private Transform m_HookShotOrigin;
    [SerializeField] private GameObject m_PlayerModel;
    [SerializeField] private CinemachineFreeLook CinemachineComponent;
    private HookPosDetection m_HookPosDetection;
    private Transform m_HookShotTarget = null;
    private Gliding m_PlayerGliding = null;
    private Walking m_PlayerWalking = null;

    [Header("Variables")]
    [SerializeField] private float m_GroundDistance = 0.4f;
    [SerializeField] float m_JumpForce = 15f;
    [SerializeField] float m_MomentumBoost = 5f;

    private bool m_IsGrounded;
    private bool m_CanJump;
    private float m_CharacterVelocityY;
    private Vector3 m_CharacterVelocityMomentum;
    private State m_State;
    private Vector3 m_HookshotPosition;
    private float m_HookshotSize;

    // Feedback
    private const float m_NormalFOV = 40f;
    private const float m_HookshotFOV = 43f;
    // private ParticleSystem speedLinesParticleSystem;

    private enum State
    {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,
    }

    private void Awake()
    {
        // speedLinesParticleSystem = transform.Find("Camera").Find("SpeedLinesParticleSystem").GetComponent<ParticleSystem>();
        Cursor.lockState = CursorLockMode.Locked;

        m_State = State.Normal;

        m_HookShotOrigin.gameObject.SetActive(false);
        m_HookPosDetection = GetComponent<HookPosDetection>();
        m_PlayerGliding = GetComponent<Gliding>();
        m_PlayerWalking = GetComponent<Walking>();

        // Debug.Log(CinemachineComponent.m_Lens.FieldOfView);
    }

    private void Update()
    {
        m_IsGrounded = Physics.CheckSphere(m_Groundcheck.position, m_GroundDistance, m_GroundMask);
        
        if (m_IsGrounded)
        {
            m_CanJump = true;
        }

        m_HookShotTarget = m_HookPosDetection.m_HookTarget;

        if (m_IsGrounded && m_CharacterVelocityY < 0)
        {
            ResetGravityEffect();
        }

        if (TestInputJump() && m_CanJump)
        {
            Jump();
        }

        switch (m_State)
        {
            default:
            case State.Normal:
                // HandleCharacterLook();
                CharacterMovement();
                HandleHookshotStart();
                break;
            case State.HookshotThrown:
                HookshotThrow();
                // HandleCharacterLook();
                CharacterMovement();
                break;
            case State.HookshotFlyingPlayer:
                // HandleCharacterLook();
                HookshotMovement();
                break;
        }
    }

    private void CharacterMovement()
    {
        float l_Horizontal = Input.GetAxisRaw("Horizontal");
        float l_Vertical = Input.GetAxisRaw("Vertical");

        Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

        if (Input.GetButton("Glide") /* Input.GetKey(KeyCode.LeftShift)*/)
            m_PlayerGliding.Glide(m_Camera, m_Controller, l_Direction);
        else
            m_PlayerWalking.Walk(m_Camera, m_Controller, l_Direction);

        // Apply gravity to the velocity
        float gravityDownForce = -60f;
        m_CharacterVelocityY += gravityDownForce * Time.deltaTime;

        // Apply Y velocity to move vector
        l_Direction.y = m_CharacterVelocityY;

        // Apply momentum
        l_Direction += m_CharacterVelocityMomentum;

        // Move Character Controller
        m_Controller.Move(l_Direction * Time.deltaTime);

        // Dampen momentum
        if (m_CharacterVelocityMomentum.magnitude > 0f)
        {
            float momentumDrag = 3f;
            m_CharacterVelocityMomentum -= m_CharacterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (m_CharacterVelocityMomentum.magnitude < .0f)
            {
                m_CharacterVelocityMomentum = Vector3.zero;
            }
        }
    }

    private void ResetGravityEffect()
    {
        m_CharacterVelocityY = -2f;
    }

    private void HandleHookshotStart()
    {
        if (TestInputDownHookshot())
        {
            if (m_HookShotTarget && m_HookPosDetection.m_CanBeHooked)
            {
                // debugHitPointTransform.position = m_HookPoint.position;
                m_HookshotPosition = m_HookShotTarget.position;
                m_HookshotSize = 0f;
                m_HookShotOrigin.gameObject.SetActive(true);
                m_HookShotOrigin.localScale = Vector3.zero;
                m_State = State.HookshotThrown;
            }
        }
    }

    private void HookshotThrow()
    {
        m_HookShotOrigin.LookAt(m_HookshotPosition);

        m_CanJump = true;

        float hookshotThrowSpeed = 500f;
        m_HookshotSize += hookshotThrowSpeed * Time.deltaTime;
        m_HookShotOrigin.localScale = new Vector3(1, 1, m_HookshotSize);

        if (m_HookshotSize >= Vector3.Distance(transform.position, m_HookshotPosition))
        {
            m_State = State.HookshotFlyingPlayer;
            CinemachineComponent.m_Lens.FieldOfView = m_HookshotFOV;
            // m_Camera.GetComponent<Camera>().fieldOfView = m_HookshotFOV;
            // Camera.main.fieldOfView = m_HookshotFOV;
            // speedLinesParticleSystem.Play();
        }
    }

    private void HookshotMovement()
    {
        m_HookShotOrigin.LookAt(m_HookshotPosition);

        Vector3 hookshotDir = (m_HookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, m_HookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 5f;

        m_Controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        float reachedHookshotPositionDistance = 1.5f;
        if (Vector3.Distance(transform.position, m_HookshotPosition) < reachedHookshotPositionDistance)
        {
            MomentumLaunch(hookshotDir, hookshotSpeed);
        }

        if (TestInputDownHookshot())
        {
            // Cancel Hookshot
            StopHookshot();
        }

        if (TestInputJump())
        {
            // Cancelled with Jump
            MomentumLaunch(hookshotDir, hookshotSpeed);
        }
    }

    private void MomentumLaunch(Vector3 p_Direction, float p_Speed)
    {
        m_CharacterVelocityMomentum = p_Direction * p_Speed * m_MomentumBoost;
        m_CharacterVelocityMomentum += Vector3.up * m_JumpForce;
        StopHookshot();
    }

    private void Jump()
    {
        // Debug.Log("saut");
        m_CanJump = false;
        ResetGravityEffect();
        m_CharacterVelocityMomentum += Vector3.up * m_JumpForce * 3;
        // ResetGravityEffect();
    }

    private void StopHookshot()
    {
        m_State = State.Normal;
        ResetGravityEffect();
        m_HookShotOrigin.gameObject.SetActive(false);
        CinemachineComponent.m_Lens.FieldOfView = m_NormalFOV;
        // m_Camera.GetComponent<Camera>().fieldOfView = m_NormalFOV;
        // Camera.main.fieldOfView = m_NormalFOV;
        // speedLinesParticleSystem.Stop();
    }

    private bool TestInputDownHookshot()
    {
        // return Input.GetKeyDown(KeyCode.E);
        return Input.GetButtonDown("Hook");
    }

    private bool TestInputJump()
    {
        //return Input.GetKeyDown(KeyCode.Space);
        return Input.GetButtonDown("Jump");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_Groundcheck.position, m_GroundDistance);
    }
}