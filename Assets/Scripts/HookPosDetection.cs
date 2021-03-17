﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPosDetection : MonoBehaviour
{
    [SerializeField]
    private float m_DetectionDistance = 20.0f;
    private float m_HookDistance = 15.0f;

    public Transform m_HookTarget = null;
    [SerializeField] GameObject m_UIFeedback;
    Transform m_LoadSprite = null;
    Transform m_RefuseSprite = null;
    [SerializeField] LayerMask m_LayerToDetect;


    [SerializeField]
    private LayerMask m_HookPointLayer;

    [SerializeField]
    private Camera cam;

    Vector2 m_CenterScreenPos;
    public bool m_CanBeHooked = false;

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
        Collider[] l_HookPositions = Physics.OverlapSphere(transform.position, m_DetectionDistance, m_HookPointLayer);
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

            // Debug.Log(Vector2.Distance(l_PotentialTarget.position, m_CenterScreenPos));

            // Actualise le point de grappin si la cible potentiel n'est pas nulle et que la cible n'est pas déjà la cible actuelle
            if (l_PotentialTarget != null && m_HookTarget != l_PotentialTarget)
            {
                SetHookPoint(l_PotentialTarget);
            }
        }
        
        if(m_HookTarget != null)
        {
            m_UIFeedback.SetActive(true);
            m_UIFeedback.transform.position = cam.WorldToScreenPoint(m_HookTarget.transform.position);

            ActualiseLoadingSprite();

            ActualiseRefuseSprite();


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
        m_LoadSprite = m_UIFeedback.transform.Find("Image/Load");
        // m_RefuseSprite = m_HookTarget.Find("Sprite/Refuse");
        m_RefuseSprite = m_UIFeedback.transform.Find("Image/Refuse");

        // On active son feedback de ciblage
        // m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    private void CancelHookPoint()
    {
        // m_HookTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;
        m_HookTarget = null;
    }

    // Permet d'actualiser la taille du sprite qui "charge" le point de grappin
    private void ActualiseLoadingSprite()
    {
        float MPT = Vector3.Distance(this.transform.position, m_HookTarget.position);
        MPT = Mathf.Clamp(MPT, m_HookDistance, m_DetectionDistance);
        m_LoadSprite.localScale = new Vector3(1 - ((MPT - m_HookDistance) / (m_DetectionDistance - m_HookDistance)), 1 - ((MPT - m_HookDistance) / (m_DetectionDistance - m_HookDistance)), 1 - ((MPT - m_HookDistance) / (m_DetectionDistance - m_HookDistance)));
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

        if(m_HookTarget)
        {
            Gizmos.DrawLine(transform.position, m_HookTarget.position);
        }
    }
}
