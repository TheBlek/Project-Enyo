using UnityEngine;

public enum BehaviourPattern
{
    Local,
    Path
}

[RequireComponent(typeof(Damagable))]
[RequireComponent(typeof(Walker))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MeleeDamager))]
[RequireComponent(typeof(ContextSteerer))]
public class Enemy : MonoBehaviour
{
    public bool ShowGizmos;

    [SerializeField] protected float _vision_radius;

    protected PathRequestManager _pathRequestManager;
    protected Walker _walker;
    protected ContextSteerer _steerer;
    protected PathFollower _path_follower;

    protected BehaviourPattern _behaviour_pattern = BehaviourPattern.Local;

    protected bool _stunned = false;
    protected virtual void Start()
    {
        _pathRequestManager = FindObjectOfType<GameManager>().GetPathRequestManager();
        _walker = GetComponent<Walker>();
        _steerer = GetComponent<ContextSteerer>();
        _path_follower = GetComponent<PathFollower>();
        _steerer.TargetIdentification = (collider) => collider.TryGetComponent(out Damagable damagable) && !damagable.is_enemy;
        _steerer.DangerIdentification = (collider) => collider.gameObject != gameObject;
        _steerer.VisionRadius = _vision_radius;
    }

    public virtual void SelfUpdate()
    {
        
    }

    public void Update()
    {
        if (_stunned)
            return;

        if (_behaviour_pattern == BehaviourPattern.Path)
            _path_follower.FollowPath();
        else
            _steerer.Act();
    }

    protected void KnockBack(float knock_back_distance, Vector2 direction)
    {
        transform.position = (Vector2)transform.position + direction.normalized * knock_back_distance;
    }

    protected void Stun(float duration)
    {
        _stunned = true;
        Invoke(nameof(ResetStun), duration);
    }

    private void ResetStun()
    {
        _stunned = false;
    }

    public void SetBehaviourPatter(BehaviourPattern pattern)
    {
        _behaviour_pattern = pattern;
    }
}
