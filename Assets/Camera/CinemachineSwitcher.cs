using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Editor;

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator m_Animator;
    private CinemachineStateDrivenCamera m_CinemachineStateDrivenCamera;
    [SerializeField]
    private CinemachineVirtualCamera[] m_CinemachineVirtualCameras;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CinemachineStateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        m_CinemachineVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        foreach (CinemachineVirtualCamera cinemachineVirtualCamera in m_CinemachineVirtualCameras)
        {
            
           
               
        }
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
    public void SwitchFreelookCamera(CinemachineFreeLook p_CameraToGo)
    {
        m_Animator.Play(p_CameraToGo.name);
    }


   
}
