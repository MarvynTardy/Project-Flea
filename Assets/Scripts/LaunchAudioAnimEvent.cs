using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAudioAnimEvent : MonoBehaviour
{
    [SerializeField]
    private string m_EnumName;

    public void LaunchAudio()
    {
        AudioManager.Sound l_EnumName = (AudioManager.Sound)System.Enum.Parse(typeof(AudioManager.Sound), m_EnumName);
        AudioManager.PlaySound(l_EnumName);
    }
}
