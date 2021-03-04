using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _diameter;

    private float _length;
    private Damagable _target;
    private Animator _animator;

    private void Awake()
    {
        TryGetComponent(out _animator);
        SetLength();
    }

    private void HandleTarget()
    {
        _target.death_offset = _length;
        _target.onKill += Blow;
    }

    public void Blow()
    {
        _animator.Play("Blow");
        Destroy(gameObject, _length);
    }

    private void SetLength()
    {
        AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Explosion")
            {
                _length = clip.length;
            }
        }
    }

    public void SetRadius(float radius)
    {
        _diameter = radius;
        transform.localScale = Vector3.one * _diameter;
    }

    public void SetTarget(Damagable target)
    {
        _target = target;
        HandleTarget();
    }
}