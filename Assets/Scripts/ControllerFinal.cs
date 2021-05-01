using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFinal : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform m_Camera = null;
    [SerializeField] private GameObject m_PlayerModel = null;
    [HideInInspector] public bool m_CanInteract = false;
    private CharacterController m_Controller = null;
    private Gliding m_PlayerGliding = null;
    private Walking m_PlayerWalking = null;
    private StaminaComponent m_StaminaComponent = null;
    private HookPosDetection m_HookPosDetection = null;

    [Header("Movement")]
    private Vector3 m_Direction;
    [HideInInspector] public bool m_SpiritMode = false;

    [Header("GroundCheck")]
    [SerializeField] [Range(0, 1)] private float m_GroundDistance = 0.4f;
    [SerializeField] public Transform m_GroundChecker = null;
    [SerializeField] public LayerMask m_GroundMask;
    private bool m_IsGrounded = false;

    [Header("Jump")]
    [SerializeField] [Range(0, 10)] private float m_JumpForce = 4;
    [SerializeField] [Range(0, 10)] private float m_JumpForceSpirit = 3;
    private bool m_CanJump;
    private bool m_IsJumping;

    [Header("Gravity")]
    [SerializeField] [Range(0, -60)] private int m_GravityScale = -30;
    [SerializeField] [Range(0, -60)] private int m_GravityScaleSpirit = -10;
    [SerializeField] [Range(0, -10)] private int m_GravityScaleReset = -4;
    private float m_VelocityY = 0;

    [Header("Hookshot")]
    [SerializeField] private Transform m_HookshotOrigin = null;
    [SerializeField] private LineRenderer m_HookLine;
    [SerializeField] [Range(0, 1)] private float m_HookshotLag = 0.3f;
    [SerializeField] [Range(1, 10)] float m_HookshotSpeedMultiplier = 5f;
    [SerializeField] [Range(0, 10)] float m_MomentumBoost = 5f;
    private Vector3 m_HookshotPosition;
    private Vector3 m_CharacterVelocityMomentum;
    private HookshotState m_HookshotState;

    [Header("Feedback")]
    private Animator m_PlayerAnim = null;

    [Header("Hookshot Feedback")]
    [SerializeField] private ParticleSystem m_SpeedParticle = null;
    [SerializeField] private ParticleSystem m_ProjectionParticle = null;
    [SerializeField] private ParticleSystem m_ImpactParticle = null;
    [SerializeField] private Transform m_ProjectionPoint;
    private bool m_TargetReach = false;
    private bool m_PlayImpact = false;
    private float m_TimeElapsed = 0;
    private Transform m_HookshotTarget = null;
    private Quaternion m_SavedRotation;

    [Header ("Spirit Feedback")]
    [SerializeField] private ParticleSystem m_GlideParticle = null;
    [SerializeField] private Material m_GlowMaterial = null;
    [SerializeField] private SkinnedMeshRenderer m_Cloth = null;
    [SerializeField] private MeshRenderer m_Pagne = null;
    private Material m_ClothSavedMaterial;
    private Material m_PagneSavedMaterial;

    #region Initialisation
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        m_CanInteract = true;
        m_PlayerAnim = GetComponentInChildren<Animator>();
        m_Controller = GetComponent<CharacterController>();
        m_PlayerGliding = GetComponent<Gliding>();
        m_PlayerWalking = GetComponent<Walking>();
        m_StaminaComponent = GetComponent<StaminaComponent>();
        m_HookPosDetection = GetComponent<HookPosDetection>();

        // Permet de conserver en mémoire les materials originels des vêtements
        m_ClothSavedMaterial = m_Cloth.materials[0];
        m_PagneSavedMaterial = m_Pagne.materials[0];
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
                    HookshotStart();
                    break;
                case HookshotState.HookshotLaunch:
                    SlerpRotationPlayer();
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

            StaminaCondition();

        }

        if (Input.GetButtonUp("Glide"))
            SpiritRelease();

        // Feedback
        switch (m_HookshotState)
        {
            default:
                break;
            case HookshotState.HookshotLaunch:
                SlerpRotationPlayer();
                ProjectionPoint();
                DrawLine();
                break;
            case HookshotState.HookshotFlyingPlayer:
                SpeedStartFeedback();
                ProjectionPoint();
                DrawLine();
                break;
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

    #region Jump
    private void Jump()
    {
        m_CanJump = false;        
        m_IsJumping = true;

        if (m_SpiritMode)
            m_VelocityY = Mathf.Sqrt(m_JumpForceSpirit * -2f * m_GravityScale);
        else
            m_VelocityY = Mathf.Sqrt(m_JumpForce * -2f * m_GravityScale);

        // Permet d'éviter que le joueur collide toujours avec le sol et reset sa gravité
        StartCoroutine(JumpCO());
    }

    IEnumerator JumpCO()
    {
        yield return new WaitForSeconds(0.05f);

        m_IsJumping = false;
    }
    #endregion

    #region Gestion de la gravité
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
        // HookshotThrown,
        HookshotFlyingPlayer,
    }

    private void HookshotStart()
    {
        if (Input.GetButtonDown("Hook"))
        {
            if (m_HookPosDetection.m_HookTarget && m_HookPosDetection.m_CanBeHooked)
            {
                StartCoroutine(HookshotStartCO());
            }
        }
    }

    IEnumerator HookshotStartCO()
    {
        // Récupère la cible du grappin désigné dans le script HookPosDetection
        m_HookshotTarget = m_HookPosDetection.m_HookTarget;

        ResetProjectionPoint();

        m_HookshotState = HookshotState.HookshotLaunch;
        m_HookshotOrigin.gameObject.SetActive(true);
        m_CanInteract = false;
        HookshotStartFeedback();

        yield return new WaitForSeconds(m_HookshotLag);

        m_CanInteract = true;
        m_HookshotPosition = m_HookshotTarget.position;

        // Change d'état (permet d'amorcer la propulsion du grappin)
        m_HookshotState = HookshotState.HookshotFlyingPlayer;
    }

    private void HookshotMovement()
    {
        m_HookshotOrigin.LookAt(m_HookshotPosition);

        Vector3 hookshotDir = (m_HookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 20f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, m_HookshotPosition), hookshotSpeedMin, hookshotSpeedMax);

        m_Controller.Move(hookshotDir * hookshotSpeed * m_HookshotSpeedMultiplier * Time.deltaTime);

        float reachedHookshotPositionDistance = 1.5f;

        // Joueur a atteint le point de grappin
        if (Vector3.Distance(transform.position, m_HookshotPosition) < reachedHookshotPositionDistance)
        {
            m_CanJump = true;
            MomentumLaunch(hookshotDir, hookshotSpeed);
            HookshotReleaseFeedback();
        }

        //// Permet de cancel le hookshot
        //if (Input.GetButtonDown("Hook"))
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

    #region SpiritMode
    private void StaminaCondition()
    {
        if (m_StaminaComponent.CurrentStamina > 0)
        {
            if (Input.GetButton("Glide"))
                SpiritStart();
            //else if (Input.GetButtonUp("Glide"))
            //    SpiritRelease();                
        }
        else if (m_StaminaComponent.CurrentStamina <= 0 || (Input.GetButtonUp("Glide")))
            SpiritRelease();
    }

    private void SpiritStart()
    {
        m_SpiritMode = true;
        m_StaminaComponent.UseStamina(10f * Time.deltaTime);

        SpiritStartFeedback();
    }

    public void SpiritRelease()
    {
        m_SpiritMode = false;

        // Permet d'éviter que le player garde l'inertie de décélération de la précédente activation
        m_PlayerGliding.GlideSpeed = 0;

        SpiritReleaseFeedback();
    }
    #endregion

    #region Visual Feedback
    // Gestion des animations, particles, shaders

    private void AnimCondGeneral()
    {        
        // Gestion des conditions d'animation du mode esprit
        //if (m_SpiritMode)
        //    m_PlayerAnim.SetBool("IsSpirit", true);
        //else
        //    m_PlayerAnim.SetBool("IsSpirit", false);

        // Gestion des conditions d'animation du personnage au sol ou en l'air
        if (m_IsGrounded)
            m_PlayerAnim.SetBool("IsGrounded", true);
        else
            m_PlayerAnim.SetBool("IsGrounded", false);

        // Gestion des conditions d'animation de saut
        if (Input.GetButtonDown("Jump") && m_CanJump && !m_SpiritMode)
            m_PlayerAnim.SetTrigger("IsJumping");

        // Gestion des conditions de dérapage
        if (m_IsGrounded && m_CharacterVelocityMomentum.magnitude > 0 && !m_SpiritMode)
        {
            // Debug.Break();
            m_PlayerAnim.SetBool("IsSkidding", true);
        }
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

    private void HookshotStartFeedback()
    {
        m_PlayerAnim.SetTrigger("IsHookshot");
        // m_HookshotParticle.Play();
    }

    private void SlerpRotationPlayer()
    {
        m_SavedRotation = Quaternion.LookRotation((m_HookshotTarget.position - transform.position).normalized);
        if (m_SavedRotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, m_SavedRotation, Time.deltaTime * 10);
        }
    }

    private void ProjectionPoint()
    {
        if (m_TimeElapsed < m_HookshotLag)
        {
            m_ProjectionPoint.position = Vector3.Lerp(m_HookshotOrigin.position, m_HookshotTarget.position, m_TimeElapsed / m_HookshotLag * 2.5f);
            m_TimeElapsed += Time.deltaTime;
            m_ProjectionParticle.Play();
            m_PlayImpact = true;
            ImpactFeedback();
        }
        else
        {
            m_TargetReach = true;
            m_ProjectionPoint.position = m_HookshotTarget.position;
            m_ProjectionParticle.Stop();
            m_ProjectionParticle.Clear();
        }
    }

    private void ResetProjectionPoint()
    {
        if (m_TargetReach)
        {
            m_TimeElapsed = 0;
            m_TargetReach = false;
        }
    }

    private void ImpactFeedback()
    {
        if (m_PlayImpact)
        {
            // Debug.Break();
            m_ImpactParticle.transform.position = m_HookshotTarget.position;
            m_ImpactParticle.transform.SetParent(m_HookshotTarget);
            m_ImpactParticle.Play();
            m_PlayImpact = false;
        }
    }
    
    private void DrawLine()
    {
        // Set du LineRenderer
        m_HookLine.SetPosition(0, m_HookshotOrigin.position);
        if (m_TargetReach)
        {
            m_HookLine.SetPosition(1, m_HookshotTarget.position);
        }
        else
            m_HookLine.SetPosition(1, m_ProjectionPoint.position);
    }

    private void HookshotReleaseFeedback()
    {
        // Reset de la rotation du perso sur l'axe y
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        m_PlayerAnim.SetTrigger("LaunchSalto");

        // Reset du LineRenderer
        m_HookLine.SetPosition(0, Vector3.zero);
        m_HookLine.SetPosition(1, Vector3.zero);

        SpeedReleaseFeedback();
    }

    private void SpeedStartFeedback()
    {
        m_SpeedParticle.Play();
    }

    private void SpeedReleaseFeedback()
    {
        m_SpeedParticle.Clear();
        m_SpeedParticle.Stop();
    }

    private void SpiritStartFeedback()
    {
        m_PlayerAnim.SetBool("IsSpirit", true);
        m_GlideParticle.Play();

        // Permet d'appliquer le shader qui illumine les habits du personnage
        m_Cloth.material = m_GlowMaterial;
        m_Pagne.material = m_GlowMaterial;

        //if (m_Controller.velocity.x > 0 || m_Controller.velocity.z > 0)
        //    SpeedStartFeedback();
    }
    
    private void SpiritReleaseFeedback()
    {
        m_PlayerAnim.SetBool("IsSpirit", false);
        m_GlideParticle.Stop();

        // Permet de remettre le material d'origine au model du personnage 
        m_Cloth.material = m_ClothSavedMaterial;
        m_Pagne.material = m_PagneSavedMaterial;

        SpeedReleaseFeedback();
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