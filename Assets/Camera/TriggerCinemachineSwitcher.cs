using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TriggerCinemachineSwitcher : MonoBehaviour
{
    private CinemachineSwitcher m_CinemachineSwitcher;

    [SerializeField]
    private CinemachineVirtualCamera m_CameraToGo;
  
    
    [SerializeField]
    private CinemachineVirtualCameraBase m_CameraToReturn;
    

    private void Start()
    {
        m_CinemachineSwitcher = FindObjectOfType<CinemachineSwitcher>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ok");
        m_CinemachineSwitcher.SwitchCamera(m_CameraToGo);
    }
    private void OnTriggerExit(Collider other)
    {
        m_CinemachineSwitcher.ReSwitchCamera(m_CameraToReturn);
    }

    

}
