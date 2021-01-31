using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPosDetection : MonoBehaviour
{
    [SerializeField]
    private float m_HookDistance = 10.0f;

    private Transform m_HookTarget = null;

    [SerializeField]
    private LayerMask m_LayerToDetect;

    [SerializeField]
    private Camera cam;

    Vector2 m_CenterScreenPos;

    private void Awake()
    {
        cam = Camera.main;
        m_CenterScreenPos = new Vector2(cam.pixelWidth/2, cam.pixelHeight/2 );
    }

    private void Update()
    {
        //Debug.Log(cam.pixelWidth/2 + cam.pixelHeight/2);
        //if (m_HookTarget)
        //{
        //    Debug.Log(cam.WorldToScreenPoint(m_HookTarget.transform.position));
        //}
        
        HookPointDetection();
    }
    private void HookPointDetection()
    {
        Transform l_PotentialTarget;

        // Récupération de tous les objects de types grapPoint dans un tableau de collider
        Collider[] l_HookPositions = Physics.OverlapSphere(transform.position, m_HookDistance, m_LayerToDetect);
        if(l_HookPositions.Length > 0)
        {
            // On initie la cible potentielle avec la première entrée de notre tableau
            l_PotentialTarget = l_HookPositions[0].transform;

            // Pour chaque HookPoint à portée
            foreach (Collider l_HookPosition in l_HookPositions)
            {
                // On crée une variable de sa position à l'écran
                Vector2 l_HookPosToScreen = cam.WorldToScreenPoint(l_HookPosition.transform.position);
                Vector2 l_PotentialPosToScreen = cam.WorldToScreenPoint(l_PotentialTarget.transform.position);

                if (Vector2.Distance(l_HookPosToScreen, m_CenterScreenPos) <= Vector2.Distance(l_PotentialPosToScreen, m_CenterScreenPos))
                {
                    l_PotentialTarget = l_HookPosition.transform;
                }
               
            }
            Debug.Log(Vector2.Distance(l_PotentialTarget.position, m_CenterScreenPos));

            // Actualise le point de grappin si la cible potentiel n'est pas nulle et que la cible n'est pas déjà la cible actuelle
            if (l_PotentialTarget != null && m_HookTarget != l_PotentialTarget)
            {
                SetHookPoint(l_PotentialTarget);
            }
        }
        
        if(m_HookTarget != null)
        {
            // Si la distance entre le joueur et le point de grappin est supérieur à la distance de détection de point de grappin
            if (Vector3.Distance(transform.position, m_HookTarget.position) > m_HookDistance)
            {
                // On désactive le point de grappin
                CancelHookPoint();
            }
        }
        
    }
    private void SetHookPoint(Transform p_ActualiseHookPoint)
    {
        if(m_HookTarget != null)
        {
            // On désactive le feedback de ciblage de l'ancienne target
            m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        

        // On actualise notre cible
        m_HookTarget = p_ActualiseHookPoint;

        // On active son feedback de ciblage
        m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    private void CancelHookPoint()
    {
        m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;
        m_HookTarget = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_HookDistance);
    }
}

