using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private int m_NumberTutoID = 1;
    private TutorialScreenManager m_Manager;
    private void Awake()
    {
        m_Manager = FindObjectOfType<TutorialScreenManager>();
    }

    private void OnTriggerEnter(Collider p_Other)
    {
        if (m_Manager)
            m_Manager.ShowTutorialScreen(m_NumberTutoID);
    }

    private void OnTriggerExit(Collider p_Other)
    {
        if (m_Manager)
            m_Manager.HideTutorialScreen(m_NumberTutoID);
    }
}
