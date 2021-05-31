using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnterTriggerCinemachine : MonoBehaviour
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
        m_CinemachineSwitcher.SwitchCamera(m_FromCamera, m_TargetCamera);
    }
}
