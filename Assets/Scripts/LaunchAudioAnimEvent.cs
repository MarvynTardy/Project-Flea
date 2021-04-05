using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAudioAnimEvent : MonoBehaviour
{
    [SerializeField]
    private string[] m_EnumName;
    private AudioManager.Sound[] m_EnumToLaunch;

    private void Awake()
    {
        m_EnumToLaunch = new AudioManager.Sound[m_EnumName.Length];

        // Debug.Log("EnumName Length = " + m_EnumName.Length + " and EnumToLaunch Length = " + m_EnumToLaunch.Length);

        for (int i = 0; i < m_EnumToLaunch.Length; i++)
        {
            m_EnumToLaunch[i] = (AudioManager.Sound)System.Enum.Parse(typeof(AudioManager.Sound), m_EnumName[i]);
        }
    }

    public void LaunchAudio()
    {
        foreach (var enumToPlay in m_EnumToLaunch)
        {
            // AudioManager.Sound l_EnumName = (AudioManager.Sound)System.Enum.Parse(typeof(AudioManager.Sound), enumToPlay);
            AudioManager.PlaySound(enumToPlay);
        }
        // AudioManager.Sound l_EnumName = (AudioManager.Sound)System.Enum.Parse(typeof(AudioManager.Sound), m_EnumName);
        // AudioManager.PlaySound(l_EnumName);
    }
}
