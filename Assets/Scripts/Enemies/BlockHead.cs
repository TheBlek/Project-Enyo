using System.Collections;
using System;
using UnityEngine;
public class BlockHead : Enemy
{
    [SerializeField] private float knock_back = 0.19f;
    [SerializeField] private float knock_back_cooldown = 0.34f;

    private Rigidbody2D rig;
    private bool stunned;

    private void Start()
    {
        var _R = new System.Random();
        target = (EnemyTargets)_R.Next(Enum.GetValues(typeof(EnemyTargets)).Length);
        IsTargetEleminated = false;
        rig = transform.GetComponent<Rigidbody2D>();
    }

    public override void SelfUpdate()
    {
        if (stunned)
            return;

        Vector2 relativePos = (Vector2)(transform.position - target_pos);
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        transform.eulerAngles = Vector3.forward * (angle + 180);
        if (relativePos.magnitude > 0.3f)
            rig.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
        else
            IsTargetEleminated = true;
    }

    #region Ailments
    private void KnockBack(float knock_back_distance)
    {
        rig.MovePosition(transform.position - transform.right * knock_back_distance);
    }

    private void Stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        stunned = true;
        yield return new WaitForSeconds(duration);
        stunned = false;
    }
    #endregion

    #region Collision Handle
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject collider)
    {
        try
        {
            if (!collider.GetComponent<Damagable>().is_enemy)
            {
                collider.GetComponent<Damagable>().TakeDamage(damage);
                KnockBack(knock_back);
                Stun(knock_back_cooldown);
            }
        }
        catch { };
    }
    #endregion

}