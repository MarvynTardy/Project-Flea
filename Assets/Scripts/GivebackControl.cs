using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivebackControl : MonoBehaviour
{
    private ControllerFinal m_Controller;

    private void Awake()
    {
        m_Controller = GetComponentInParent<ControllerFinal>();
    }

    public void EnabledControl()
    {
        m_Controller.m_CanInteract = true;
        m_Controller.m_IsAfk = false;
    }

    public void StopLookAroundLoop()
    {
        m_Controller.m_PlayerAnim.SetBool("IsLookAround", false);
    }
}
