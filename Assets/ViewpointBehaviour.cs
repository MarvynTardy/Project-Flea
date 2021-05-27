using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewpointBehaviour : MonoBehaviour
{
    private bool m_IsTrigger;
    private bool m_IsExit;

    // References
    [SerializeField] private Transform m_Platform;
    [SerializeField] private float m_TimeToWait = 10;
    private bool m_MovePlayer = false;
    private bool m_RotatePlayer = false;
    private Animator m_Anim;
    private ParticleSystem m_Particle;
    private ViewpointManager m_VpManager;
    private ControllerFinal m_Controller;
    private float m_ActualTimeRotation = 0;
    private float m_ActualTimeTrigger = 0;

    void Awake()
    {
        m_Anim = GetComponentInChildren<Animator>();
        m_Particle = GetComponentInChildren<ParticleSystem>();
        m_VpManager = FindObjectOfType<ViewpointManager>();
    }

    // Gérer le fait de passer le player en enfant le temps de l'anim
    // Désactiver player
    // Le placer au centre du spot, bien orienté
    // Lancer la séquence d'animation joueur
    // Activer 

    private void Update()
    {
        if (!m_IsExit)
        { 
            if (m_MovePlayer)
            {
                SetPlayerPosition();
            }

            if (m_RotatePlayer)
            {
                SetPlayerRotation();
            }

            if (m_IsTrigger)
            {
                m_ActualTimeTrigger += Time.deltaTime;

                if (m_ActualTimeTrigger >= m_TimeToWait)
                {
                    ReleaseViewPoint();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider p_Other)
    {
        // S'il n'a pas encore été activé
        if (!m_IsTrigger && !m_IsExit)
        {
            // On récupère le script Controller sur le joueur
            m_Controller = p_Other.gameObject.GetComponent<ControllerFinal>();

            // Le POV est désormais activé
            m_IsTrigger = true;
            
            // On commence la séquence d'activation
            StartCoroutine(Activation(p_Other));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_IsExit)
        {
            float m_Horizontal = 0;
            float m_Vertical = 0;

            m_Horizontal = Input.GetAxisRaw("Horizontal");
            m_Vertical = Input.GetAxisRaw("Vertical");
            if (m_Horizontal > 0 || m_Vertical > 0)
            {
                m_Controller.m_PlayerAnim.SetBool("IsInactive", false);
            }
        }
    }

    IEnumerator Activation(Collider p_Other)
    {
        // Feedback visuel
        m_Anim.SetBool("IsTrigger", true);
        m_Particle.Play();

        // Active la fonction qui recentre le joueur sur le socle
        m_MovePlayer = true;

        // Passer le joueur inactif
        m_Controller.m_CanInteract = false;

        // Fais jouer son animation de marche
        m_Controller.m_PlayerAnim.SetBool("IsMoving", true);

        // Passer le joueur en enfant de la plateforme afin d'éviter l'effet de "tremblement"
        p_Other.transform.parent = m_Platform;

        // Passer la velocité du joueur à 0

        // Caméra à gérer ici

        // S'il y a bien, dans la scène, un ViewpointManager
        if (m_VpManager)
        {
            // Alors, on lui indique que le Viewpoint actuel est activé

        }

        yield return new WaitForSeconds(1);
    }

    private void SetPlayerPosition()
    {
        Vector3 m_Direction;

        m_Direction = Vector3.Lerp(m_Controller.transform.position, m_Platform.position, 0.1f);

        m_Direction.y = m_Controller.transform.position.y;

        m_Controller.transform.position = m_Direction;


        // Si le joueur a atteint le centre de la plateforme
        if (Vector3.Distance(m_Platform.position, m_Controller.transform.position) < 2.59f)
        {
            m_Controller.m_PlayerAnim.SetBool("IsMoving", false);
            m_Controller.m_PlayerAnim.SetBool("IsInactive", true);

            m_RotatePlayer = true;

            m_MovePlayer = false;
        }
    }

    private void SetPlayerRotation()
    {
        m_Controller.transform.rotation = Quaternion.Slerp(m_Controller.transform.rotation, this.transform.rotation, Time.deltaTime * 1);

        m_ActualTimeRotation += Time.deltaTime;

        if (m_ActualTimeRotation >= 3f)
        {
            m_ActualTimeRotation = 0;
            m_RotatePlayer = false;
        }
    }


    private void ReleaseViewPoint()
    {
        m_IsExit = true;
        m_ActualTimeRotation = 0;
        m_IsTrigger = false;
        m_Controller.transform.parent = null;
        m_Controller.m_PlayerAnim.SetBool("IsInactive", false);
        //m_Controller.m_CanInteract = true;
    }
}
