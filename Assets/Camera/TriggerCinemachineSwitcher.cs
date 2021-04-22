using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCinemachineSwitcher : MonoBehaviour
{
    private CinemachineSwitcher m_CinemachineSwitcher;

    private void Start()
    {
        m_CinemachineSwitcher = FindObjectOfType<CinemachineSwitcher>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ok");
        m_CinemachineSwitcher.SwitchCamera();
    }
    private void OnTriggerExit(Collider other)
    {
        m_CinemachineSwitcher.ReSwitchCamera();
    }

}
