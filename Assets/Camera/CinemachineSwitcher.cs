using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
// using Cinemachine.Editor;


public class CinemachineSwitcher : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField]
    private CinemachineStateDrivenCamera m_CinemachineStateDrivenCamera;
    [SerializeField]
    private CinemachineTrackedDolly m_CinemachineTrackedDolly;
    [SerializeField]
    public ICinemachineCamera m_ActualCamera;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CinemachineStateDrivenCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        m_CinemachineTrackedDolly = GetComponentInChildren<CinemachineTrackedDolly>();
        
    }

    private void Update()
    {
        m_ActualCamera = m_CinemachineStateDrivenCamera.LiveChild;
        //Debug.Log(m_ActualCamera);
    }

 
    public void SwitchCamera(CinemachineVirtualCameraBase p_FromCamera = null, CinemachineVirtualCameraBase p_ToCamera = null)
    {
        if((object)m_CinemachineStateDrivenCamera.LiveChild == p_FromCamera)
        {
            if(m_CinemachineTrackedDolly != null)
            {
                m_CinemachineTrackedDolly.m_AutoDolly.m_Enabled = true;
            }
            
            m_Animator.Play(p_ToCamera.name);
        }

    }

    public void ReSwitchCamera(CinemachineVirtualCameraBase p_FromCamera = null, CinemachineVirtualCameraBase p_ToCamera = null)
    {
        if ((object)m_CinemachineStateDrivenCamera.LiveChild == p_ToCamera)
        {
            if(m_CinemachineTrackedDolly != null)
            {
                m_CinemachineTrackedDolly.m_AutoDolly.m_Enabled = false;
            }
            
            m_CinemachineTrackedDolly.m_PathPosition = 0.0f;
            m_Animator.Play(p_FromCamera.name);
        }

    }



}
