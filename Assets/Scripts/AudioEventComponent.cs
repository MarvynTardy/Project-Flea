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
        if(gameObject.name == ("PointOfView1"))
        {
            AudioManager.PlaySound(AudioManager.Sound.POV1);
        }
        if (gameObject.name == ("PointOfView2"))
        {
            AudioManager.PlaySound(AudioManager.Sound.POV2);
        }
        if (gameObject.name == ("PointOfView3"))
        {
            AudioManager.PlaySound(AudioManager.Sound.POV3);
        }
    }

    private void HookShotLaunch()
    {
        AudioManager.PlaySound(AudioManager.Sound.HookShotLaunch);
    }
    private void HookShotReachPoint()
    {
        AudioManager.PlaySound(AudioManager.Sound.HookShotReachPoint);
    }

    //private void PlayLanding()
    //{
    //    AudioManager.PlaySound(AudioManager.Sound.Landing);
    //}

    
}
