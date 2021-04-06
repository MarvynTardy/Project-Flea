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
}
