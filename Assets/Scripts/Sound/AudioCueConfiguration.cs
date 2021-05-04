using UnityEngine;
public struct AudioCueConfiguration
{
    public float pitch;
    public float volume;
    public bool loop;

    public void ApplyTo(AudioSource source)
    {
        source.pitch = pitch;
        source.volume = volume;
        source.loop = loop;
    }
}