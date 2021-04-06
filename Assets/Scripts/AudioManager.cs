using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager 
{
    public enum Sound
    {
        Foley,
        Footstep,
        PlayerAttack,
    }
    private static Dictionary<Sound, float> probability;
    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject m_OneShotGameObject;
    private static AudioSource m_OneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        probability = new Dictionary<Sound, float>();
        probability[Sound.Foley] = 25f;
        soundTimerDictionary[Sound.Footstep] = 0f;
    }

    public static void PlaySound(Sound p_Sound)
    {
        if (CanPlaySound(p_Sound))
        {
            if(m_OneShotGameObject == null)
            {
                m_OneShotGameObject = new GameObject("One Shot Sound");
                m_OneShotAudioSource = m_OneShotGameObject.AddComponent<AudioSource>();
                m_OneShotAudioSource.PlayOneShot(GetAudioClip(p_Sound));
            }
            m_OneShotAudioSource.PlayOneShot(GetAudioClip(p_Sound));
            //Object.Destroy(m_OneShotGameObject, m_OneShotAudioSource.clip.length);


        }
        
    }

    private static bool CanPlaySound(Sound p_Sound)
    {
        switch (p_Sound)
        {
            default:
                return true;
            //case Sound.Footstep:
            //    if (soundtimerdictionary.containskey(p_sound))
            //    {
            //        float l_lasttimeplayed = soundtimerdictionary[p_sound];
            //        float l_footsteptimermax = 0.5f;
            //        if (l_lasttimeplayed + l_footsteptimermax < time.time)
            //        {
            //            soundtimerdictionary[p_sound] = time.time;
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }

            //    }
            //    else
            //    {
            //        return true;
            //    }
            case Sound.Foley:
                if (probability.ContainsKey(p_Sound))
                {
                    float l_RandomThrow = Random.Range(0, 100);
                    float l_ProbabilityToLaunch = probability[p_Sound];
                    if(l_RandomThrow <= l_ProbabilityToLaunch)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
                else
                {
                    return true;
                }

                
        }
    }

    private static AudioClip GetAudioClip(Sound p_Sound)
    {
        foreach (AudioAssets.SoundAudioClip soundAudioClip in AudioAssets.i.m_SoundAudioClipArray)
        {
            if(soundAudioClip.m_Sound == p_Sound )
            {
                foreach(AudioClip audioClip in soundAudioClip.m_AudioClip)
                {
                    return soundAudioClip.m_AudioClip[Random.Range(0, soundAudioClip.m_AudioClip.Length)];
                }
            }
        }
        return null;
    }
}
