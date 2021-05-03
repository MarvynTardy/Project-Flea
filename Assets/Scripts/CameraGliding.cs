using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraGliding : MonoBehaviour
{
    private CinemachineSwitcher m_CinemachineSwitcher;

    [SerializeField]
    private CinemachineVirtualCameraBase m_CameraToGo;


    [SerializeField]
    private CinemachineVirtualCameraBase m_CameraToReturn;

    private void Start()
    {
        m_CinemachineSwitcher = FindObjectOfType<CinemachineSwitcher>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_CinemachineSwitcher.SwitchCamera(m_CameraToGo);
        }
        else
        {
            m_CinemachineSwitcher.SwitchCamera(m_CameraToReturn);
        }
    }
}
