using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/AudioCueEventChannel")]
public class AudioCueEventChannel : ScriptableObject
{

    public UnityAction<AudioClip, AudioCueConfiguration, Vector2> OnEventRaised;

    public void RaiseEvent(AudioClip clip, AudioCueConfiguration config, Vector2 position)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(clip, config, position);
        else
            Debug.Log("An Audio Cue was requested but noone was listening");
    }

}