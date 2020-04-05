using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer audioPlayerInstance;

    public AudioClip[] audioClips;
    public AudioSource sourcePrefab;

    private void Awake()
    {
        if (audioPlayerInstance == null) { audioPlayerInstance = this; }
    }

    public void PlayClip(AudioClip sound, bool loop, float volume)
    {
        AudioSource temp;

        temp = Instantiate(sourcePrefab);

        if (loop)
        {
            temp.loop = true;
        }

        temp.clip = sound;
        temp.volume = volume;
        temp.Play();
    }
}
