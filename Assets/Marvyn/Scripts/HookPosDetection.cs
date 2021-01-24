using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPosDetection : MonoBehaviour
{
    [SerializeField]
    private float m_GrapDistance = 10.0f;
    
    private Transform m_GrapTarget = null;

    [SerializeField]
    private LayerMask m_LayerToDetect;

    private void Update()
    {
        GrapPointDetection();
    }

    private void GrapPointDetection()
    {
        Collider[] l_GrapPositions = Physics.OverlapSphere(transform.position, m_GrapDistance, m_LayerToDetect);
        foreach (Collider grapPosition in l_GrapPositions)
        {
            if(m_GrapTarget == null)
            {
                m_GrapTarget = grapPosition.transform;
            }
            if (Vector3.Distance(transform.position, grapPosition.transform.position) < Vector3.Distance(transform.position, m_GrapTarget.position))
            {
                m_GrapTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;

                m_GrapTarget = grapPosition.transform;
                m_GrapTarget.GetComponentInChildren<SpriteRenderer>().enabled = true;

            }

        }

        if (Vector3.Distance(transform.position, m_GrapTarget.position) > m_GrapDistance)
        {
            m_GrapTarget.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_GrapDistance);
    }
}

