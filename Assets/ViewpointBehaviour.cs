using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewpointBehaviour : MonoBehaviour
{
    private bool m_IsTrigger;

    // References
    [SerializeField] private Transform m_Platform;
    private Animator m_Anim;
    private ParticleSystem m_Particle;
    private ViewpointManager m_VpManager;
    private ControllerFinal m_Controller;

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

    //private void Update()
    //{
    //    if ()
    //    {

    //    }
    //}

    private void OnTriggerEnter(Collider p_Other)
    {
        // S'il n'a pas encore été activé
        if (!m_IsTrigger)
        {
            // On récupère le script Controller sur le joueur
            m_Controller = p_Other.gameObject.GetComponent<ControllerFinal>();

            // Il est désormais activé
            m_IsTrigger = true;
            
            // On commence la séquence d'activation
            StartCoroutine(Activation(p_Other));
        }
    }

    IEnumerator Activation(Collider p_Other)
    {
        // Feedback visuel
        m_Anim.SetBool("IsTrigger", true);
        m_Particle.Play();
        SetPlayerPosition(p_Other.gameObject.transform.position);

        // Passer le joueur inactif
        p_Other.gameObject.GetComponent<ControllerFinal>().m_CanInteract = false;

        // Passer le joueur en enfant de la plateforme afin d'éviter l'effet de "tremblement"
        p_Other.transform.parent = m_Platform;

        // Caméra à gérer ici

        // S'il y a bien, dans la scène, un ViewpointManager
        if (m_VpManager)
        {
            // Alors, on lui indique que le Viewpoint actuel est activé

        }

        yield return new WaitForSeconds(1);
    }

    //private void OnTriggerStay(Collider p_Other)
    //{
    //    p_Other.transform.position = new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z);
    //}

    private void SetPlayerPosition(Vector3 p_PlayerPos)
    {
        //while (Vector3.Distance(m_Platform.position, m_Controller.transform.position) > 0.1f)
        //{
        //    Debug.Log("Ils sont trop loin");

        //    Vector3 m_Direction;

        //    m_Direction = Vector3.Lerp(m_Platform.position, m_Controller.transform.position, 0.5f);

        //    m_Controller.m_Controller.Move(m_Direction);
        //}

        Debug.Log("Ils sont trop loin");

        Vector3 m_Direction;

        m_Direction = Vector3.Lerp(m_Platform.position, m_Controller.transform.position, 0.5f);

        m_Controller.m_Controller.Move(m_Direction);
    }
}
