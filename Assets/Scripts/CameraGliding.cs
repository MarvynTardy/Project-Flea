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
        //if (Input.GetKey(KeyCode.LeftShift))
        //{ bhy
        //    m_CinemachineSwitcher.SwitchCamera(m_CameraToGo);
        //    m_CameraToReturn.ForceCameraPosition(m_CameraToGo.transform.position, Quaternion.identity);
        //}
        //else
        //{
        //    m_CinemachineSwitcher.SwitchCamera(m_CameraToReturn);
        //    m_CameraToGo.ForceCameraPosition(m_CameraToReturn.transform.position, Quaternion.identity);
        //}
    }
}
