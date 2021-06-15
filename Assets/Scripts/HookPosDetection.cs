using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPosDetection : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float m_DetectionDistance = 20.0f;
    [SerializeField] private float m_HookDistance = 17.0f;

    float l_DotProductLimit = 0.5f;
    private Vector2 m_CenterScreenPos;

    [Header ("References")]
    [SerializeField] private GameObject m_UIFeedback;
    [SerializeField] private LayerMask m_LayerToDetect;
    [SerializeField] private LayerMask m_HookPointLayer;
    [HideInInspector] public Transform m_HookTarget = null;
    [HideInInspector] public bool m_CanBeHooked = false;
    private ControllerFinal m_Controller;
    private Camera m_Camera;
    private Transform m_LoadSprite = null;
    private Transform m_RefuseSprite = null;
    private Transform m_ValidSprite = null;


    private void Awake()
    {
        m_Camera = Camera.main;
        m_CenterScreenPos = new Vector2(m_Camera.pixelWidth/2, m_Camera.pixelHeight/2 );
        m_Controller = GetComponent<ControllerFinal>();
    }

    private void Update()
    {
        //Debug.Log(cam.pixelWidth/2 + cam.pixelHeight/2);
        //if (m_HookTarget)
        //{
        //    Debug.Log(cam.WorldToScreenPoint(m_HookTarget.transform.position));
        //}
        
        HookPointDetection();

        if (m_HookTarget)
        {
            if (!TestDotPositionValidity(m_HookTarget))
            {
                m_HookTarget = null;
            }
        }
    }

    private void HookPointDetection()
    {
        Transform l_PotentialTarget;
        Transform l_PotentialTargetBack;

        // Récupération de tous les objects de types grapPoint dans un tableau de collider
        Collider[] l_HookPositions = Physics.OverlapSphere(transform.position, m_DetectionDistance, m_HookPointLayer);
        if(l_HookPositions.Length > 0)
        {
            if (TestDotPositionValidity(l_HookPositions[0].transform))
            {
                // On initie la cible potentielle avec la première entrée de notre tableau
                l_PotentialTarget = l_HookPositions[0].transform;
                l_PotentialTargetBack = l_HookPositions[0].transform;
                
                // Pour chaque HookPoint à portée
                foreach (Collider l_HookPosition in l_HookPositions)
                {
                    if (TestDotPositionValidity(l_HookPosition.transform))
                    {
                        // On récupère le vecteur de la caméra jusqu'à la target testée
                        Vector3 l_VectorCamToTarget = Vector3.Normalize(l_HookPosition.transform.position - m_Controller.transform.position);

                        // On vérifie si il est bien devant le player
                        float l_DotProduct = Vector3.Dot(m_Controller.m_Camera.transform.forward, l_VectorCamToTarget);

                        // On crée une variable de sa position à l'écran
                        Vector2 l_HookPosToScreen = m_Camera.WorldToScreenPoint(l_HookPosition.transform.position);
                        Vector2 l_PotentialPosToScreen = m_Camera.WorldToScreenPoint(l_PotentialTarget.transform.position);

                        if (Vector2.Distance(l_HookPosToScreen, m_CenterScreenPos) <= Vector2.Distance(l_PotentialPosToScreen, m_CenterScreenPos))
                        {
                            if (l_DotProduct > 0)
                            {
                                l_PotentialTarget = l_HookPosition.transform;
                            }
                            else
                            {
                                l_PotentialTargetBack = l_HookPosition.transform;
                            }
                        }
                    }
                }
                // Debug.Log(Vector2.Distance(l_PotentialTarget.position, m_CenterScreenPos));

                if (l_PotentialTarget == null && l_PotentialTargetBack != null)
                {
                    l_PotentialTarget = l_PotentialTargetBack;
                }

                // Actualise le point de grappin si la cible potentiel n'est pas nulle et que la cible n'est pas déjà la cible actuelle
                if (l_PotentialTarget != null  && m_HookTarget != l_PotentialTarget)
                {
                    SetHookPoint(l_PotentialTarget);
                }
            }
        }
        
        if(m_HookTarget != null)
        {
            if (m_UIFeedback)
            {
                if (m_Controller.m_HookshotState == ControllerFinal.HookshotState.Neutral)
                {
                    m_UIFeedback.SetActive(true);
                    m_UIFeedback.transform.position = m_Camera.WorldToScreenPoint(m_HookTarget.transform.position);

                    ActualiseLoadingSprite();
                    ActualiseRefuseSprite();
                }
                else
                {
                    m_UIFeedback.SetActive(false);
                }
            }

            if (!Physics.Linecast(transform.position, m_HookTarget.position, m_LayerToDetect) && Vector3.Distance(this.transform.position, m_HookTarget.position) <= m_HookDistance)
                m_CanBeHooked = true;
            else
                m_CanBeHooked = false;

            // Si la distance entre le joueur et le point de grappin est supérieur à la distance de détection de point de grappin
            if (Vector3.Distance(transform.position, m_HookTarget.position) > m_DetectionDistance)
            {
                // On désactive le point de grappin
                CancelHookPoint();
            }
        }
        
        if (!m_HookTarget)
        {
            if (m_UIFeedback)
                m_UIFeedback.SetActive(false);
        }
    }

    private void SetHookPoint(Transform p_ActualiseHookPoint)
    {
        if(m_HookTarget != null)
        {
            // On désactive le feedback de ciblage de l'ancienne target
            // m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;

            m_LoadSprite.localScale = Vector3.zero;
            m_RefuseSprite.localScale = Vector3.zero;
        }

        m_CanBeHooked = false;

        // On actualise notre cible
        m_HookTarget = p_ActualiseHookPoint;
        // m_LoadSprite = m_HookTarget.Find("Sprite/Load");
        if (m_UIFeedback)
        {
            m_LoadSprite = m_UIFeedback.transform.Find("Image/Load");
            // m_RefuseSprite = m_HookTarget.Find("Sprite/Refuse");
            m_RefuseSprite = m_UIFeedback.transform.Find("Image/Refuse");
            m_ValidSprite = m_UIFeedback.transform.Find("Image/Valid");
        }

        // On active son feedback de ciblage
        // m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    private float TestDotPosition(Transform l_Target)
    {
        // On récupère le vecteur de la caméra jusqu'à la target testée
        Vector3 l_VectorCamToTarget = Vector3.Normalize(l_Target.position - m_Controller.m_Camera.transform.position);

        // On vérifie si il est bien devant la caméra
        float l_DotProduct = Vector3.Dot(m_Controller.m_Camera.transform.forward, l_VectorCamToTarget);

        return l_DotProduct;
    }

    private bool TestDotPositionValidity(Transform l_Target)
    {
        if (TestDotPosition(l_Target) >= l_DotProductLimit)
            return true;
        else
            return false;
    }

    private void CancelHookPoint()
    {
        // m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;
        m_HookTarget = null;
    }

    // Permet d'actualiser la taille du sprite qui "charge" le point de grappin
    private void ActualiseLoadingSprite()
    {
        if (Vector3.Distance(transform.position, m_HookTarget.position) > m_HookDistance)
        {
            m_ValidSprite.localScale = Vector3.zero;
            float MPT = Vector3.Distance(this.transform.position, m_HookTarget.position);
            MPT = Mathf.Clamp(MPT, m_HookDistance, m_DetectionDistance);
            m_LoadSprite.localScale = new Vector3(1 - ((MPT - m_HookDistance) / (m_DetectionDistance - m_HookDistance)), 1 - ((MPT - m_HookDistance) / (m_DetectionDistance - m_HookDistance)), 1 - ((MPT - m_HookDistance) / (m_DetectionDistance - m_HookDistance)));
        }
        else
        {
            m_LoadSprite.localScale = Vector3.zero;
            m_ValidSprite.localScale = Vector3.one;
        }
    }

    private void ActualiseRefuseSprite()
    {
        if (Physics.Linecast(transform.position, m_HookTarget.position, m_LayerToDetect))
        {
            // Debug.Log("ça touche");
            m_RefuseSprite.localScale = Vector3.one;
        }
        else
        {
            // Debug.Log("ça touche pas");
            m_RefuseSprite.localScale = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_DetectionDistance);

        Gizmos.color = Color.red;
        if(m_HookTarget)
        {
            Gizmos.DrawLine(transform.position, m_HookTarget.position);
        }
    }
}

