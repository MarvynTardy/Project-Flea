using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator m_Animator;
    private CinemachineStateDrivenCamera m_CinemachineStateDrivenCamera;
    
   
   

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CinemachineStateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        
    }

    private void Update()
    {

        Debug.Log(m_CinemachineStateDrivenCamera.LiveChild);

        
    }

    public void SwitchCamera(CinemachineVirtualCamera p_CameraToGo)
    {
        m_Animator.Play(p_CameraToGo.name);
        
    }
    public void ReSwitchCamera(CinemachineVirtualCameraBase p_CameraToReturn)
    {
        m_Animator.Play(p_CameraToReturn.name);
    }


   
}
