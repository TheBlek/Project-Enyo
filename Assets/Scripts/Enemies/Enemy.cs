using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float ApS;
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected LayerMask player_mask;

    protected EnemyTargets target;
    protected Vector3 target_pos;

    public bool IsTargetEleminated;

    public virtual void SelfUpdate()
    {

    }

    public EnemyTargets GetTarget()
    {
        return target;
    }

    public void SetTarget(Vector3 _target_pos)
    {
        target_pos = _target_pos;
    }

}
