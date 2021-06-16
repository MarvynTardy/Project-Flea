using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnterTriggerCinemachine : MonoBehaviour
{

    private CinemachineSwitcher m_CinemachineSwitcher;
    private CameraChecker m_CameraChecker;
    [SerializeField]
    private float m_DurationTimer = 0.0f;

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
        Debug.Log("Yes");
        m_CinemachineSwitcher.SwitchCamera(m_FromCamera, m_TargetCamera);
        StartCoroutine(StartTimer());
    }

    public IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(m_DurationTimer);

        Debug.Log("Ok");
        m_CinemachineSwitcher.SwitchCamera(m_TargetCamera, m_FromCamera);
    }
}

