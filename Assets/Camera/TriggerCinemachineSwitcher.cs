using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCinemachineSwitcher : MonoBehaviour
{
    private CinemachineSwitcher m_CinemachineSwitcher;


    private void OnTriggerEnter(Collider other)
    {
        m_CinemachineSwitcher.SwitchCamera();
    }
    private void OnTriggerExit(Collider other)
    {
        m_CinemachineSwitcher.ReSwitchCamera();
    }

}
