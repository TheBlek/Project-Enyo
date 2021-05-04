using System;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioCueEventChannel _musicCueEventChannel;
    [SerializeField] private AudioCueEventChannel _sfxCueEventChannel;
    [SerializeField] private SoundEmitterFactory _factory;

    private void Start()
    {
        _musicCueEventChannel.OnEventRaised += OnSoundPlay;
        _sfxCueEventChannel.OnEventRaised += OnSoundPlay;
    }

    private void OnSoundPlay(AudioClip clip, AudioCueConfiguration config, Vector2 position)
    {
        SoundEmitter emitter = _factory.Create();
        emitter.PlayClip(clip, config, position);
        emitter.OnPlayingEnd += OnPlayingEnd;
    }

    private void OnPlayingEnd(SoundEmitter emitter)
    {
        Destroy(emitter);
    }
}