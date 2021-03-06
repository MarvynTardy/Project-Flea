﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAssets : MonoBehaviour
{
    private static AudioAssets _i;

    public static AudioAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<AudioAssets>("AudioAssets"));
            return _i;
        }
    }



    public SoundAudioClip[] m_SoundAudioClipArray;


    [System.Serializable]
    public class SoundAudioClip
    {
        public AudioManager.Sound m_Sound;
        public AudioClip[] m_AudioClip;
        [Range(0,100)]
        public float m_ProbabilityToLaunch = 100;
        
    }
}
