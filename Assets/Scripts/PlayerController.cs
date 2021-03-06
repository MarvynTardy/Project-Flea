﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController m_Controller = null;
    [SerializeField] private Transform m_Camera = null;
    [SerializeField] private Transform m_Groundcheck = null;
    [SerializeField] private LayerMask m_GroundMask;
    [SerializeField] private Transform m_HookShotOrigin = null;
    [SerializeField] private GameObject m_PlayerModel = null;
    [SerializeField] private CinemachineFreeLook CinemachineComponent = null;
    [SerializeField] private ParticleSystem[] m_GlideParticle = null;
    [SerializeField] private ParticleSystem m_HookshotParticle = null;
    [SerializeField] private SkinnedMeshRenderer m_Cloth = null;
    [SerializeField] private MeshRenderer m_Pagne = null;
    [SerializeField] private Material m_GlowMaterial = null;
    private Material m_ClothSavedMaterial;
    private Material m_PagneSavedMaterial;
    private HookPosDetection m_HookPosDetection;
    private Transform m_HookShotTarget = null;
    [SerializeField] private LineRenderer m_HookLine;
    private Gliding m_PlayerGliding = null;
    private Walking m_PlayerWalking = null;
    private WallGliding m_PlayerWallGliding = null;
    private Animator m_PlayerAnim = null;
    private Quaternion m_SavedRotation;
    private StaminaComponent m_StaminaComponent = null;

    [Header("Variables")]
    [SerializeField] private float m_GroundDistance = 0.4f;
    [SerializeField] float m_JumpForce = 15f;
    [SerializeField] float m_MomentumBoost = 5f;
    [SerializeField] float gravityDownForce = -40f;

    private bool m_IsGrounded;
    private bool m_CanJump;
    private float m_CharacterVelocityY;
    private Vector3 m_CharacterVelocityMomentum;
    private State m_State;
    private Vector3 m_HookshotPosition;
    private float m_HookshotSize;
    private bool m_FreezePlayer = false;

    // Feedback
    private const float m_NormalFOV = 40f;
    private const float m_HookshotFOV = 43f;
    // private ParticleSystem speedLinesParticleSystem;

    private enum State
    {
        Normal,
        HookshotLaunch,
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
        m_PlayerWallGliding = GetComponent<WallGliding>();
        m_PlayerAnim = GetComponentInChildren<Animator>();
        m_StaminaComponent = GetComponent<StaminaComponent>();
        //for (int i = 0; i < m_Cloth.Length; i++)
        //{
        //    m_SavedMaterial[i] = m_Cloth[i].materials[0];
        //}
        m_ClothSavedMaterial = m_Cloth.materials[0];
        m_PagneSavedMaterial = m_Pagne.materials[0];
        // m_HookLine = m_HookShotOrigin.GetComponent<LineRenderer>();
        // Debug.Log(CinemachineComponent.m_Lens.FieldOfView);
    }

    private void Update()
    {
        if(!m_FreezePlayer)
        {
            m_IsGrounded = Physics.CheckSphere(m_Groundcheck.position, m_GroundDistance, m_GroundMask);
        
            if (m_IsGrounded)
            {
                m_CanJump = true;
                m_PlayerAnim.SetBool("IsGrounded", true);
            }
            else
                m_PlayerAnim.SetBool("IsGrounded", false);

            m_HookShotTarget = m_HookPosDetection.m_HookTarget;

            if (m_IsGrounded && m_CharacterVelocityY < 0)
            {
                ResetGravityEffect();
            }

            if (TestInputJump() && m_CanJump)
            {
                Jump();

                if (!Input.GetButton("Glide"))
                    m_PlayerAnim.SetTrigger("IsJumping");
            }

            ApplyWallJump();

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
    }

    private void LateUpdate()
    {
        switch (m_State)
        {
            case State.HookshotLaunch:
            case State.HookshotFlyingPlayer:
                DrawLine();
                break;
            default:
                break;
        }
    }

    private void CharacterMovement()
    {
        float l_Horizontal = Input.GetAxisRaw("Horizontal");
        float l_Vertical = Input.GetAxisRaw("Vertical");

        Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

        if (l_Direction != Vector3.zero)
        {
            m_PlayerAnim.SetBool("IsMoving", true);
           
            
        }
        else
        {
            m_PlayerAnim.SetBool("IsMoving", false);
        }

        if (Input.GetButton("Glide") && m_StaminaComponent.CurrentStamina >= 0 /* Input.GetKey(KeyCode.LeftShift)*/)
        {
            l_Direction = m_PlayerGliding.Glide(m_Camera, m_Controller, l_Direction);
            m_PlayerAnim.SetBool("IsGliding", true);
            m_Cloth.material = m_GlowMaterial;
            m_Pagne.material = m_GlowMaterial;
            //foreach (var clothToReplace in m_Cloth)
            //{
            //    clothToReplace.material = m_GlowMaterial;
            //}
            m_StaminaComponent.UseStamina(10f * Time.deltaTime);
            foreach (ParticleSystem glideParticle in m_GlideParticle)
            {
                glideParticle.Play();
            }
        }
        else if (Input.GetButtonUp("Glide"))
        {
            m_PlayerGliding.GlideSpeed = 0;
            m_PlayerWallGliding.EndWallGlide();
            m_PlayerAnim.SetBool("IsGliding", false);
            //for (int i = 0; i < m_Cloth.Length; i++)
            //{
            //    m_Cloth[i].material = m_SavedMaterial[i];
            //}
            m_Cloth.material = m_ClothSavedMaterial;
            m_Pagne.material = m_PagneSavedMaterial;
           
            foreach (ParticleSystem glideParticle in m_GlideParticle)
            {
                glideParticle.Stop();
            }
        }
        else
        {
            l_Direction = m_PlayerWalking.Walk(m_Camera, m_Controller, l_Direction);
        }

        if (!m_PlayerWallGliding.IsWallGliding())
        {
            // Apply gravity to the velocity
            float gravityDownForce = 0f;
            if(!Input.GetButton("Glide"))
                gravityDownForce = -30f;
            else if(Input.GetButton("Glide"))
                gravityDownForce = -10f;
            m_CharacterVelocityY += gravityDownForce * Time.deltaTime;

            // Apply Y velocity to move vector
            l_Direction.y = m_CharacterVelocityY;
        }
        else
        {
            l_Direction += m_PlayerWallGliding.WallGlideGravity;
        }

        // Apply momentum
        l_Direction += m_CharacterVelocityMomentum;
        // Move Character Controller
        m_Controller.Move(l_Direction * Time.deltaTime);

        // Dampen momentum
        if (m_CharacterVelocityMomentum.magnitude > 0f)
        {
            float momentumDrag = 3f;
            m_CharacterVelocityMomentum -= m_CharacterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (m_CharacterVelocityMomentum.magnitude < 1f)
            {
                m_CharacterVelocityMomentum = Vector3.zero;
            }
        }

        

    }

    private void ResetGravityEffect()
    {
        m_CharacterVelocityY = -2;
    }

    private void HandleHookshotStart()
    {
        if (TestInputDownHookshot())
        {
            if (m_HookShotTarget && m_HookPosDetection.m_CanBeHooked)
            {
                StartCoroutine(HandleHookshotStartCO());
            }
        }
    }

    IEnumerator HandleHookshotStartCO()
    {
        //float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
        //m_PlayerModel.transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);
        m_State = State.HookshotLaunch;
        m_SavedRotation = m_PlayerModel.transform.rotation;
        m_PlayerModel.transform.LookAt(m_HookShotTarget);
        m_HookShotOrigin.gameObject.SetActive(true);
        m_HookshotParticle.Play();

        // DrawLine();

        m_PlayerAnim.SetTrigger("IsHookshot");
        m_FreezePlayer = true;
        yield return new WaitForSeconds(0.3f);
        m_PlayerAnim.SetBool("LaunchRotate", true);

        m_FreezePlayer = false;
        m_HookshotPosition = m_HookShotTarget.position;
        m_HookshotSize = 0f;
        // m_HookShotOrigin.localScale = Vector3.zero;
        m_State = State.HookshotThrown;
    }

    private void HookshotThrow()
    {
        m_HookShotOrigin.LookAt(m_HookshotPosition);

        m_CanJump = true;

        float hookshotThrowSpeed = 500f;
        m_HookshotSize += hookshotThrowSpeed * Time.deltaTime;
        // m_HookShotOrigin.localScale = new Vector3(1, 1, m_HookshotSize);

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
        float hookshotSpeedMax = 20f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, m_HookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 5f;

        m_Controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        float reachedHookshotPositionDistance = 1.5f;

        // joueur a atteint le point de grappin
        if (Vector3.Distance(transform.position, m_HookshotPosition) < reachedHookshotPositionDistance)
        {
            m_PlayerModel.transform.rotation = m_SavedRotation;
            m_PlayerAnim.SetTrigger("LaunchSalto");
            MomentumLaunch(hookshotDir, hookshotSpeed);
        }

        if (TestInputDownHookshot())
        {
            // Cancel Hookshot
            m_PlayerModel.transform.rotation = m_SavedRotation;
            StopHookshot();
        }

        if (TestInputJump())
        {
            // Cancelled with Jump
            m_PlayerModel.transform.rotation = m_SavedRotation;
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

    private void ApplyWallJump()
    {
        if(m_PlayerWallGliding.WallJumpPower != Vector3.zero)
        {
            m_CharacterVelocityMomentum += m_PlayerWallGliding.WallJumpPower;
            m_PlayerWallGliding.WallJumpPower = Vector3.zero;
        }
    }

    private void StopHookshot()
    {
        m_State = State.Normal;
        ResetGravityEffect();
        m_HookShotOrigin.gameObject.SetActive(false);
        CinemachineComponent.m_Lens.FieldOfView = m_NormalFOV;

        StopHookshotFeedback();
        // m_Camera.GetComponent<Camera>().fieldOfView = m_NormalFOV;
        // Camera.main.fieldOfView = m_NormalFOV;
        // speedLinesParticleSystem.Stop();
    }

    private void StopHookshotFeedback()
    {
        m_HookshotParticle.Clear();

        m_HookLine.SetPosition(0, Vector3.zero);
        m_HookLine.SetPosition(1, Vector3.zero);
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

    private void DrawLine()
    {
        // m_HookLine.positionCount = 2;
        // Debug.Log(m_HookShotOrigin.position);
        m_HookLine.SetPosition(0, m_HookShotOrigin.position);
        m_HookLine.SetPosition(1, m_HookShotTarget.position);
        //if (Vector3.Distance(m_HookShotOrigin.position, m_HookShotTarget.position) > 0.5f)
        //{
        //    m_HookLine.positionCount = 0;
        //}
        // Debug.Break();
    }

    private void OnDrawGizmos()
    {
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(m_Groundcheck.position, m_GroundDistance);
        /*Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), transform.right * 0.75f);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + -0.5f), transform.right * 0.75f);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), -transform.right * 0.75f);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + -0.5f), -transform.right * 0.75f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.right);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), -transform.right);*/
    }
}
