using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMusicVolume : MonoBehaviour
{
    public void UpdateVolume(float volume)
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetMusicVolume(volume);
        }
    }
}
