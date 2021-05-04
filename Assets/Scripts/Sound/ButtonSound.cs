using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioCueEventChannel _sfxChannel;
    [SerializeField] private AudioClip _onHover;
    [SerializeField] private AudioClip _onClick;

    public void PlaySoundOnHover()
    {
        _sfxChannel.RaiseEvent(_onHover, new AudioCueConfiguration { loop = false, pitch = 1f, volume = 1f }, Vector2.zero);
    }
    public void PlaySoundOnClick()
    {
        _sfxChannel.RaiseEvent(_onClick, new AudioCueConfiguration { loop = false, pitch = 1f, volume = 1f }, Vector2.zero);
    }
}