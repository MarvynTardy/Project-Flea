using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TriggerCinemachineSwitcher : MonoBehaviour
{
    private CinemachineSwitcher m_CinemachineSwitcher;
    private CameraChecker m_CameraChecker;

    [SerializeField]
    private CinemachineVirtualCameraBase m_FromCamera;


    [SerializeField]
    private CinemachineVirtualCameraBase m_TargetCamera;
    

    private void Start()
    {
        m_CameraChecker = FindObjectOfType<CameraChecker>();
        m_CinemachineSwitcher = FindObjectOfType<CinemachineSwitcher>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ok");
        m_CinemachineSwitcher.SwitchCamera(m_FromCamera, m_TargetCamera);
        m_CameraChecker.enabled = false;
    }
    private void OnTriggerExit(Collider other) 
    {
        m_CinemachineSwitcher.ReSwitchCamera(m_FromCamera, m_TargetCamera);
        m_CameraChecker.enabled = true;
        
    }

    

}
