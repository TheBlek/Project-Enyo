using System.Collections;
using System;
using UnityEngine;
public class BlockHead : Enemy
{
    public bool ShowGizmos;

    [SerializeField] private float knock_back = 0.19f;
    [SerializeField] private float knock_back_cooldown = 0.34f;

    private Rigidbody2D rig;
    private bool stunned;
    private Vector3[] waypoints;
    private int current_waypoint;
    private bool pathRequested;

    private GameManager gameManager;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        RandomizeTarget();
        IsTargetEleminated = false;
        rig = transform.GetComponent<Rigidbody2D>();

        gameManager = FindObjectOfType<GameManager>();
        pathRequestManager = gameManager.GetPathRequestManager();

        pathRequested = true;
        pathRequestManager.RequestPath(transform.position, target_pos, OnPathFound);
    }

    public void RandomizeTarget()
    {
        var _R = new System.Random();
        target = (EnemyTargets)_R.Next(Enum.GetValues(typeof(EnemyTargets)).Length);
    }

    public override void SelfUpdate()
    {

        if (stunned || pathRequested || waypoints == null)
            return;

        if (current_waypoint >= waypoints.Length || target_pos != waypoints[waypoints.Length - 1])
        {
            pathRequested = true;
            pathRequestManager.RequestPath(transform.position, target_pos, OnPathFound);

            if (current_waypoint >= waypoints.Length)
                return;
        }


        Vector2 relativePos = (Vector2)(transform.position - waypoints[current_waypoint]);
        TurnToAngle(relativePos);
        
        if (relativePos.magnitude > 0.01f)
        {
            SetWalking(true);
            rig.MovePosition(transform.position - transform.right * speed * Time.deltaTime);
        }
        else
        {
            current_waypoint++;
            if (current_waypoint >= waypoints.Length)
                IsTargetEleminated = true;
        }

    }

    private void SetWalking(bool state)
    {
        if (animator != null)
            animator.SetBool("IsWalking", state);
    }

    private void TurnToAngle(Vector3 relative_pos)
    {
        float angle = Mathf.Atan2(relative_pos.y, relative_pos.x) * Mathf.Rad2Deg;

        transform.eulerAngles = Vector3.forward * angle;
    }

    public void OnPathFound(Vector3[] _waypoints, bool success)
    {
        if (success)
        {
            waypoints = _waypoints;
            current_waypoint = 1; // You don't need to go to where u stand
            waypoints[waypoints.Length - 1] = target_pos; // Just correcting pos in cell 
            pathRequested = false;
        }
        else
        {
            pathRequested = false;
            IsTargetEleminated = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos)
            return;

        Gizmos.color = Color.red;
        if (waypoints != null && current_waypoint < waypoints.Length)
        {
            Gizmos.DrawLine(transform.position, waypoints[current_waypoint]);
            Gizmos.DrawCube(waypoints[current_waypoint], Vector3.one * .1f);
            if (current_waypoint + 1 < waypoints.Length)
                for (int i = current_waypoint + 1; i < waypoints.Length; i++)
                {
                    Gizmos.DrawCube(waypoints[i], Vector3.one * .1f);
                    Gizmos.DrawLine(waypoints[i], waypoints[i - 1]);
                }
        }
    }

    #region Ailments
    private void KnockBack(float knock_back_distance)
    {
        rig.MovePosition(transform.position + transform.right * knock_back_distance);
    }

    private void Stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        stunned = true;
        SetWalking(false);
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
                animator.Play("Attack");
                KnockBack(knock_back);
                Stun(knock_back_cooldown);
            }
        }
        catch { };
    }
    #endregion

}