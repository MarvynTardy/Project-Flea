using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFinal : MonoBehaviour
{
    public float DebugFloat = 0;

    [Header("General")]
    [SerializeField] private Transform m_Camera = null;
    private CharacterController m_Controller = null;
    private Gliding m_PlayerGliding = null;
    private Walking m_PlayerWalking = null;
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

    [Header("Gravity")]
    [SerializeField] [Range(0, -60)] private int m_GravityScale = -30;
    [SerializeField] [Range(0, -60)] private int m_GravityScaleSpirit = -10;
    [SerializeField] [Range(0, -10)] private int m_GravityScaleReset = -4;
    private float m_VelocityY = 0;

    [Header("Hookshot")]
    private Vector3 m_CharacterVelocityMomentum;

    [Header("Feedback")]
    private Animator m_PlayerAnim = null;

    #region Initialisation
    void Awake()
    {
        m_CanInteract = true;
        m_PlayerAnim = GetComponentInChildren<Animator>();
        m_Controller = GetComponent<CharacterController>();
        m_PlayerGliding = GetComponent<Gliding>();
        m_PlayerWalking = GetComponent<Walking>();
    }
    #endregion

    void Update()
    {
        if (m_CanInteract)
        {
            CharacterMovement();
            CheckGround();

            if (Input.GetButtonDown("Jump") && m_CanJump)
                Jump();
        }
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

        // Application du momentum du grappin
        m_Direction += m_CharacterVelocityMomentum;

        // Application de la direction finale au character controller du personnage
        m_Controller.Move(m_Direction * Time.deltaTime);

        // Réduction du momentum
        if (m_CharacterVelocityMomentum.magnitude > 0f)
        {
            float momentumDrag = 3f;
            m_CharacterVelocityMomentum -= m_CharacterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (m_CharacterVelocityMomentum.magnitude < .0f)
            {
                m_CharacterVelocityMomentum = Vector3.zero;
            }
        }

        // Met à jour les animations du personnage
        AnimationCondition();
    }

    private void CheckGround()
    {
        // Permet de savoir si le joueur touche un objet de type "sol"
        m_IsGrounded = Physics.CheckSphere(m_GroundChecker.position, m_GroundDistance, m_GroundMask);

        if (m_IsGrounded)
        {
            m_CanJump = true;
            ResetGravityEffect();
        }
    }
    
    private void Jump()
    {
        m_CanJump = false;
        DebugFloat = Mathf.Sqrt(m_JumpForce * -2f * m_GravityScale);
        m_VelocityY = DebugFloat;
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

    private enum State
    {
        Neutral,
        HookshotLaunch,
        HookshotThrown,
        HookshotFlyingPlayer,
    }



    #endregion

    #region Visual Feedback
    // Gestion des animations, particles, shaders

    private void AnimationCondition()
    {
        // Gestion des conditions d'animation de déplacements
        if (m_Direction.x != 0 || m_Direction.x != 0)
            m_PlayerAnim.SetBool("IsMoving", true);
        else
            m_PlayerAnim.SetBool("IsMoving", false);
        
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