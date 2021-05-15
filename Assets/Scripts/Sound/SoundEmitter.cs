using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    public UnityAction<SoundEmitter> OnPlayingEnd;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        //OnPlayingEnd += Inform;
    }

    private void Inform(SoundEmitter emitter)
    {
        Debug.Log(emitter.name + " has ended playing his sound");
    }

    public void PlayClip(AudioClip clip, AudioCueConfiguration config, Vector2 position)
    {
        config.ApplyTo(_source);
        _source.clip = clip;
        _source.transform.position = position;
        _source.Play();

        if (!_source.loop)
            StartCoroutine(nameof(WaitTilTheEnd));
    }

    private IEnumerator WaitTilTheEnd()
    {
        yield return new WaitForSeconds(_source.clip.length);
        OnPlayingEnd?.Invoke(this);
    }
}