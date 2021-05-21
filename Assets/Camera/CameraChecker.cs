using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChecker : MonoBehaviour
{
    [SerializeField]
    private Transform m_PlayerTransform;
    [SerializeField]
    private CinemachineVirtualCamera m_CameraToGo;
    [SerializeField]
    private CinemachineFreeLook m_CameraToReturn;


    private CinemachineSwitcher m_CinemachineSwitcher;


    private bool m_isLayered = false;


    [SerializeField]
    private LayerMask m_TargetLayer;

    // Update is called once per frame
    void Update()
    {
        
        RaycastHit l_Hit;
        m_isLayered = Physics.Raycast(m_PlayerTransform.position, Vector3.down, out l_Hit, 1.4f,  m_TargetLayer);
        
        if (m_isLayered)
        {
            if (l_Hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
  
            {
                m_CinemachineSwitcher.SwitchFreelookCamera(m_CameraToReturn);
            }
            if (l_Hit.transform.gameObject.layer == LayerMask.NameToLayer("Props"))
            {
                m_CinemachineSwitcher.SwitchCamera(m_CameraToGo);
            }
            
        }
       
    }

    private void Start()
    {
        m_CinemachineSwitcher = FindObjectOfType<CinemachineSwitcher>();
    }

    private void OnDrawGizmos()
    {
       
    }
}
