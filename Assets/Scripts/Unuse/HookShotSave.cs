using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookShotSave : MonoBehaviour
{
    //[Header("References")]
    //[SerializeField] private CharacterController m_Controller;
    //[SerializeField] private Transform m_Camera;
    //[SerializeField] private Transform m_Groundcheck = null;
    //[SerializeField] private LayerMask m_GroundMask;
    //[SerializeField] private Transform m_HookShotOrigin;
    //private HookPosDetection m_HookPosDetection;
    //private Transform m_HookShotTarget = null;

    //[Header("Variables")]
    //[SerializeField] private float m_Gravity = -9.81f;
    //[SerializeField] private float m_GroundDistance = 0.4f;
    //[SerializeField] private float m_JumpHeight = 3f;
    //[SerializeField] float m_JumpForce = 15f;
    //[SerializeField] float m_MomentumBoost = 5f;

    //private Vector3 m_Velocity = Vector3.zero;
    //private bool isGrounded;
    //private float m_Speed = 6f;
    //private float m_TurnSmoothTime = 0.5f;
    //private float m_TurnSmoothVelocity;
    //private float characterVelocityY;
    //private Vector3 characterVelocityMomentum;
    //private State state;
    //private Vector3 hookshotPosition;
    //private float hookshotSize;

    //// Feedback
    //private const float NORMAL_FOV = 60f;
    //private const float HOOKSHOT_FOV = 100f;
    //// private ParticleSystem speedLinesParticleSystem;


    //// À extraire_________________
    //private enum State
    //{
    //    Normal,
    //    HookshotThrown,
    //    HookshotFlyingPlayer,
    //}

    //private void Awake()
    //{
    //    // speedLinesParticleSystem = transform.Find("Camera").Find("SpeedLinesParticleSystem").GetComponent<ParticleSystem>();
    //    // Cursor.lockState = CursorLockMode.Locked;


    //    // À extraire_________________
    //    state = State.Normal;

    //    m_HookShotOrigin.gameObject.SetActive(false);
    //    m_HookPosDetection = GetComponent<HookPosDetection>();
    //}

    //private void Update()
    //{
    //    m_HookShotTarget = m_HookPosDetection.m_HookTarget;

    //    //isGrounded = Physics.CheckSphere(m_Groundcheck.position, m_GroundDistance, m_GroundMask);

    //    //// Gestion du saut et de la gravité
    //    //if (isGrounded && m_Velocity.y < 0)
    //    //{
    //    //    m_Velocity.y = -2f;
    //    //}

    //    //if (TestInputJump() && isGrounded)
    //    //{
    //    //    m_Velocity.y = Mathf.Sqrt(m_JumpHeight * -2f * m_Gravity);
    //    //}

    //    // À extraire
    //    switch (state)
    //    {
    //        default:
    //        case State.Normal:
    //            // HandleCharacterLook();
    //            CharacterMovement();
    //            HandleHookshotStart();
    //            break;
    //        case State.HookshotThrown:
    //            HookshotThrow();
    //            // HandleCharacterLook();
    //            CharacterMovement();
    //            break;
    //        case State.HookshotFlyingPlayer:
    //            // HandleCharacterLook();
    //            HookshotMovement();
    //            break;
    //    }
    //}


    //// À extraire_________________
    //private void CharacterMovement()
    //{
    //    float l_Horizontal = Input.GetAxisRaw("Horizontal");
    //    float l_Vertical = Input.GetAxisRaw("Vertical");

    //    // Vector3 characterVelocity = transform.right * l_Horizontal * moveSpeed + transform.forward * l_Vertical * moveSpeed;
    //    Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

    //    // Vérifie si le vecteur est en train de se déplacer
    //    if (l_Direction.magnitude >= 0.1f)
    //    {
    //        // Atan2 permet de retourner l'angle qu'il y a entre le joueur et sa direction
    //        // Rad2Deg permet de convertir l'angle (retourné en Radian) en degree
    //        // En additionant la rotation de la caméra, le déplacement devient relatif à 
    //        float l_TargetAngle = Mathf.Atan2(l_Direction.x, l_Direction.z) * Mathf.Rad2Deg + m_Camera.eulerAngles.y;

    //        // Smooth la transition entre l'angle actuel et l'angle désiré à une vélocité et une durée prédéfini
    //        float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
    //        transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

    //        Vector3 l_MoveDir = Quaternion.Euler(0f, l_Angle, 0f) * Vector3.forward;

    //        m_Controller.Move(l_MoveDir * m_Speed * Time.deltaTime);
    //    }

    //    // Apply gravity to the velocity
    //    float gravityDownForce = -60f;
    //    characterVelocityY += gravityDownForce * Time.deltaTime;

    //    // Apply Y velocity to move vector
    //    l_Direction.y = characterVelocityY;

    //    // Apply momentum
    //    l_Direction += characterVelocityMomentum;

    //    // Move Character Controller
    //    m_Controller.Move(l_Direction * Time.deltaTime);

    //    // Dampen momentum
    //    if (characterVelocityMomentum.magnitude > 0f)
    //    {
    //        float momentumDrag = 3f;
    //        characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
    //        if (characterVelocityMomentum.magnitude < .0f)
    //        {
    //            characterVelocityMomentum = Vector3.zero;
    //        }
    //    }
    //}


    //// À extraire_________________
    //private void ResetGravityEffect()
    //{
    //    characterVelocityY = 0f;
    //}

    //private void HandleHookshotStart()
    //{
    //    if (TestInputDownHookshot())
    //    {
    //        if (m_HookShotTarget)
    //        {
    //            // debugHitPointTransform.position = m_HookPoint.position;
    //            hookshotPosition = m_HookShotTarget.position;
    //            hookshotSize = 0f;
    //            m_HookShotOrigin.gameObject.SetActive(true);
    //            m_HookShotOrigin.localScale = Vector3.zero;
    //            state = State.HookshotThrown;
    //        }
    //    }
    //}

    //private void HookshotThrow()
    //{
    //    m_HookShotOrigin.LookAt(hookshotPosition);

    //    float hookshotThrowSpeed = 500f;
    //    hookshotSize += hookshotThrowSpeed * Time.deltaTime;
    //    m_HookShotOrigin.localScale = new Vector3(1, 1, hookshotSize);

    //    if (hookshotSize >= Vector3.Distance(transform.position, hookshotPosition))
    //    {
    //        state = State.HookshotFlyingPlayer;
    //        // speedLinesParticleSystem.Play();
    //    }
    //}

    //private void HookshotMovement()
    //{
    //    m_HookShotOrigin.LookAt(hookshotPosition);

    //    Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

    //    float hookshotSpeedMin = 10f;
    //    float hookshotSpeedMax = 40f;
    //    float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
    //    float hookshotSpeedMultiplier = 5f;

    //    // Move Character Controller
    //    // characterController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);
    //    m_Controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

    //    float reachedHookshotPositionDistance = 1.5f;
    //    if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
    //    {
    //        MomentumLaunch(hookshotDir, hookshotSpeed);
    //    }

    //    if (TestInputDownHookshot())
    //    {
    //        // Cancel Hookshot
    //        StopHookshot();
    //    }

    //    if (TestInputJump())
    //    {
    //        // Cancelled with Jump
    //        MomentumLaunch(hookshotDir, hookshotSpeed);
    //    }
    //}

    //private void MomentumLaunch(Vector3 p_Direction, float p_Speed)
    //{
    //    characterVelocityMomentum = p_Direction * p_Speed * m_MomentumBoost;
    //    characterVelocityMomentum += Vector3.up * m_JumpForce;
    //    StopHookshot();
    //}

    //private void StopHookshot()
    //{
    //    state = State.Normal;
    //    ResetGravityEffect();
    //    m_HookShotOrigin.gameObject.SetActive(false);
    //    // speedLinesParticleSystem.Stop();
    //}

    //private bool TestInputDownHookshot()
    //{
    //    return Input.GetKeyDown(KeyCode.E);
    //}

    //private bool TestInputJump()
    //{
    //    return Input.GetKeyDown(KeyCode.Space);
    //}
}
