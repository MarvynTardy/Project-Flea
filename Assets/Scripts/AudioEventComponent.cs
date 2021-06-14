using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventComponent : MonoBehaviour
{
   private void PlayFootstep()
    {
        AudioManager.PlaySound(AudioManager.Sound.Footstep);
    }

    private void PlayFoley()
    {
        AudioManager.PlaySound(AudioManager.Sound.Foley);
    }

    private void PlayPOVSound()
    {
        AudioManager.PlaySound(AudioManager.Sound.POV);
    }

    private void HookShotLaunch()
    {
        AudioManager.PlaySound(AudioManager.Sound.HookShotLaunch);
    }
    private void HookShotReachPoint()
    {
        AudioManager.PlaySound(AudioManager.Sound.HookShotReachPoint);
    }

    private void PlayLanding()
    {
        AudioManager.PlaySound(AudioManager.Sound.Landing);
    }
}
