using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFinal : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform m_Camera = null;
    private CharacterController m_Controller = null;
    private Gliding m_PlayerGliding = null;
    private Walking m_PlayerWalking = null;
    private HookPosDetection m_HookPosDetection = null;
    private bool m_CanInteract = false;

    [Header("Movement")]
    private Vector3 m_Direction;
    private bool m_SpiritMode = false;

    [Header("GroundCheck")]
    [SerializeField] [Range(0, 1)] private float m_GroundDistance = 0.4f;
    private bool m_IsGrounded = false;
    [SerializeField] private Transform m_GroundChecker = null;
    [SerializeField] private LayerMask m_GroundMask;

    [Header("Jump")]
    [SerializeField] [Range(0, 10)] private float m_JumpForce = 4;
    private bool m_CanJump;
    private bool m_IsJumping;

    [Header("Gravity")]
    [SerializeField] [Range(0, -60)] private int m_GravityScale = -30;
    [SerializeField] [Range(0, -60)] private int m_GravityScaleSpirit = -10;
    [SerializeField] [Range(0, -10)] private int m_GravityScaleReset = -4;
    private float m_VelocityY = 0;

    [Header("Hookshot")]
    [SerializeField] private Transform m_HookShotOrigin = null;
    [SerializeField] [Range(0, 1)] private float m_HookshotLag = 0.3f;
    [SerializeField] [Range(1, 10)] float hookshotSpeedMultiplier = 5f;
    [SerializeField] [Range(0, 10)] float m_MomentumBoost = 5f;
    float hookshotThrowSpeed = 500f; // probablement inutilisé à la fin
    private Vector3 m_HookshotPosition;
    [SerializeField] private Vector3 m_CharacterVelocityMomentum;
    private HookshotState m_HookshotState;
    private float m_HookshotSize;

    [Header("Feedback")]
    private Animator m_PlayerAnim = null;
    private Transform m_HookShotTarget = null;

    #region Initialisation
    void Awake()
    {
        m_CanInteract = true;
        m_PlayerAnim = GetComponentInChildren<Animator>();
        m_Controller = GetComponent<CharacterController>();
        m_PlayerGliding = GetComponent<Gliding>();
        m_PlayerWalking = GetComponent<Walking>();
        m_HookPosDetection = GetComponent<HookPosDetection>();
    }
    #endregion

    void Update()
    {
        if (m_CanInteract)
        {
            switch (m_HookshotState)
            {
                default:
                case HookshotState.Neutral:
                    // HandleCharacterLook();
                    CharacterMovement();
                    HandleHookshotStart();
                    break;
                case HookshotState.HookshotThrown:
                    // HookshotThrow();
                    // HandleCharacterLook();
                    CharacterMovement();
                    break;
                case HookshotState.HookshotFlyingPlayer:
                    // HandleCharacterLook();
                    HookshotMovement();
                    break;
            }
            // CharacterMovement();
            CheckGround();

            if (Input.GetButtonDown("Jump") && m_CanJump)
                Jump();
        }

        // HandleHookshotStart();

        // Récupère la cible du grappin désigné dans le script HookPosDetection
        m_HookShotTarget = m_HookPosDetection.m_HookTarget;
    }

    private void CharacterMovement()
    {
        float l_Horizontal = Input.GetAxisRaw("Horizontal");
        float l_Vertical = Input.GetAxisRaw("Vertical");

        m_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

        if (m_SpiritMode)
            m_Direction = m_PlayerGliding.Glide(m_Camera, m_Controller, m_Direction);
        else
            m_Direction = m_PlayerWalking.Walk(m_Camera, m_Controller, m_Direction);

        GravityUpload();
        AnimCondWalk(m_Direction);

        // Application du momentum du grappin
        m_Direction += m_CharacterVelocityMomentum;

        // Application de la direction finale au character controller du personnage
        m_Controller.Move(m_Direction * Time.deltaTime);


        // Réduction du momentum
        if (m_CharacterVelocityMomentum.magnitude > 0f)
        {
            float momentumDrag = 3f;
            m_CharacterVelocityMomentum -= m_CharacterVelocityMomentum * momentumDrag * Time.deltaTime;
            // Si l'on veut modifier le temps de skidding du personnage au sol, il suffit de changer la valeur (ici 5) à laquelle on compare la magnitude du momentum
            if (m_CharacterVelocityMomentum.magnitude < 5f)
            {
                m_CharacterVelocityMomentum = Vector3.zero;
            }
        }

        // Met à jour les animations du personnage
        AnimCondGeneral();
    }

    private void CheckGround()
    {
        if (!m_IsJumping)
        {
            // Permet de savoir si le joueur touche un objet de type "sol"
            m_IsGrounded = Physics.CheckSphere(m_GroundChecker.position, m_GroundDistance, m_GroundMask);

            if (m_IsGrounded)
            {
                m_CanJump = true;
                ResetGravityEffect();
            }
        }
    }
    
    private void Jump()
    {
        m_CanJump = false;        
        m_IsJumping = true;
        m_VelocityY = Mathf.Sqrt(m_JumpForce * -2f * m_GravityScale);
        StartCoroutine(JumpCO());
    }

    IEnumerator JumpCO()
    {
        yield return new WaitForSeconds(0.05f);

        m_IsJumping = false;
    }

    #region Gestion de la gravité
    private void GravityUpload()
    {
        // Applique la gravité à la velocité du personnage
        if (m_SpiritMode)
            m_VelocityY += (float)m_GravityScaleSpirit * Time.deltaTime;
        else
            m_VelocityY += (float)m_GravityScale * Time.deltaTime;

        // Applique la VelocityY à la direction de déplacement du personnage
        m_Direction.y = m_VelocityY;
    }

    private void ResetGravityEffect()
    {
        // Appellé pour éviter que la gravité ne s'incrémente en permanence
        m_VelocityY = m_GravityScaleReset;
    }
    #endregion

    #region Hookshot
    // Gestion d'état du grappin et fonctions adéquates

    private enum HookshotState
    {
        Neutral,
        HookshotLaunch,
        HookshotThrown,
        HookshotFlyingPlayer,
    }

    private void HandleHookshotStart()
    {
        if (Input.GetButtonDown("Hook"))
        {
            if (m_HookShotTarget && m_HookPosDetection.m_CanBeHooked)
            {
                StartCoroutine(HandleHookshotStartCO());
            }
        }
    }

    IEnumerator HandleHookshotStartCO()
    {
        m_HookshotState = HookshotState.HookshotLaunch;
        m_HookShotOrigin.gameObject.SetActive(true);
        m_CanInteract = false;

        yield return new WaitForSeconds(m_HookshotLag);

        m_CanInteract = true;
        m_HookshotPosition = m_HookShotTarget.position;
        m_HookshotSize = 0f;

        // Change d'état (permet d'amorcer la propulsion du grappin)
        m_HookshotState = HookshotState.HookshotFlyingPlayer;
    }

    private void HookshotThrow()
    {
        m_HookShotOrigin.LookAt(m_HookshotPosition);

        m_CanJump = true;

        m_HookshotSize += hookshotThrowSpeed * Time.deltaTime;

        if (m_HookshotSize >= Vector3.Distance(transform.position, m_HookshotPosition))
        {
            m_HookshotState = HookshotState.HookshotFlyingPlayer;
        }
    }

    private void HookshotMovement()
    {
        m_HookShotOrigin.LookAt(m_HookshotPosition);

        Vector3 hookshotDir = (m_HookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 20f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, m_HookshotPosition), hookshotSpeedMin, hookshotSpeedMax);

        m_Controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        float reachedHookshotPositionDistance = 1.5f;

        // Joueur a atteint le point de grappin
        if (Vector3.Distance(transform.position, m_HookshotPosition) < reachedHookshotPositionDistance)
        {
            // StopHookshot();
            MomentumLaunch(hookshotDir, hookshotSpeed);
        }

        //if (TestInputDownHookshot())
        //{
        //    StopHookshot();
        //}

        //if (TestInputJump())
        //{
        //    MomentumLaunch(hookshotDir, hookshotSpeed);
        //}
    }

    private void MomentumLaunch(Vector3 p_Direction, float p_Speed)
    {
        m_CharacterVelocityMomentum = p_Direction * p_Speed * m_MomentumBoost;
        m_CharacterVelocityMomentum += Vector3.up * m_JumpForce;
        StopHookshot();
    }

    private void StopHookshot()
    {
        m_HookshotState = HookshotState.Neutral;
        ResetGravityEffect();
        // m_HookShotOrigin.gameObject.SetActive(false);
    }

    #endregion

    #region Visual Feedback
    // Gestion des animations, particles, shaders

    private void AnimCondGeneral()
    {        
        // Gestion des conditions d'animation du mode esprit
        if (m_SpiritMode)
            m_PlayerAnim.SetBool("IsGliding", true);
        else
            m_PlayerAnim.SetBool("IsGliding", false);

        // Gestion des conditions d'animation du personnage au sol ou en l'air
        if (m_IsGrounded)
            m_PlayerAnim.SetBool("IsGrounded", true);
        else
            m_PlayerAnim.SetBool("IsGrounded", false);

        // Gestion des conditions d'animation de saut
        if (Input.GetButtonDown("Jump") && m_CanJump && !m_SpiritMode)
            m_PlayerAnim.SetTrigger("IsJumping");

        // Gestion des conditions de dérapage
        if (m_IsGrounded && m_CharacterVelocityMomentum.magnitude > 0)
            m_PlayerAnim.SetBool("IsSkidding", true);
        else
            m_PlayerAnim.SetBool("IsSkidding", false);

    }

    private void AnimCondWalk(Vector3 p_Direction)
    {
        // Gestion des conditions d'animation de déplacements
        if (p_Direction.x != 0 || p_Direction.x != 0)
            m_PlayerAnim.SetBool("IsMoving", true);
        else
            m_PlayerAnim.SetBool("IsMoving", false);
    }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_GroundChecker.position, m_GroundDistance);
    }
    #endregion
}

#region Pour un script aux petits...
//~
//                           /~
//                     \  \ /**
//                      \ ////
//                      // //
//                     // //
//                   ///&//
//                  / & /\ \
//                /  & .,,  \
//              /& %  :       \
//            /&  %   :  ;     `\
//           /&' &..%   !..    `.\
//          /&' : &''" !  ``. : `.\
//         /#' % :  "" * .   : : `.\
//        I# :& :  !"  *  `.  : ::  I
//        I &% : : !%.` '. . : : :  I
//        I && :%: .&.   . . : :  : I
//        I %&&&%%: WW. .%. : :     I
//         \&&&##%%%`W! & '  :   ,'/
//          \####ITO%% W &..'  #,'/
//            \W&&##%%&&&&### %./
//              \###j[\##//##}/
//                 ++///~~\//_
//                  \\ \ \ \  \_
//                  /  /    \
#endregion