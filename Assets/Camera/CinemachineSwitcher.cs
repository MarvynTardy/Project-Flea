using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator m_Animator;
    private bool m_ThirdPersonCamera;
    //private CinemachineBrain m_CinemachineBrain;
    

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            SwitchCamera();
        }
        Debug.Log(Camera.current);

        
    }

    public void SwitchCamera()
    {
        
        m_Animator.Play("CameraRail");
    }
    public void ReSwitchCamera()
    {
        m_Animator.Play("Main Camera");
    }


   
}
