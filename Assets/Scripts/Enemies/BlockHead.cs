using System.Collections;
using System;
using UnityEngine;
public class BlockHead : Enemy
{
    [SerializeField] private float knock_back = 0.19f;
    [SerializeField] private float knock_back_cooldown = 0.34f;

    private Rigidbody2D rig;
    private bool stunned;
    private Vector3[] waypoints;
    private int current_waypoint;
    private bool pathRequested;

    private GameManager gameManager;

    private void Start()
    {
        var _R = new System.Random();
        target = (EnemyTargets)_R.Next(Enum.GetValues(typeof(EnemyTargets)).Length);
        IsTargetEleminated = false;
        rig = transform.GetComponent<Rigidbody2D>();

        gameManager = FindObjectOfType<GameManager>();
        pathRequestManager = gameManager.GetPathRequestManager();

        pathRequested = true;
        pathRequestManager.RequestPath(transform.position, target_pos, OnPathFound);
    }

    public override void SelfUpdate()
    {

        if (stunned || pathRequested)
            return;

        Vector3 target_cell = gameManager.GetGridManager().SnapGlobalPositionToNearestCell(target_pos);
        if ((current_waypoint >= waypoints.Length || target_cell != waypoints[waypoints.Length - 1]) && !pathRequested)
        {
            pathRequested = true;
            pathRequestManager.RequestPath(transform.position, target_pos, OnPathFound);
        }

        Vector2 relativePos = (Vector2)(transform.position - waypoints[current_waypoint]);
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        transform.eulerAngles = Vector3.forward * (angle + 180);
        if (relativePos.magnitude > 0.01f)
        {
            rig.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
        }
        else
        {
            current_waypoint++;
            if (current_waypoint >= waypoints.Length)
                IsTargetEleminated = true;
        }

    }

    public void OnPathFound(Vector3[] _waypoints, bool success)
    {
        if (success)
        {
            waypoints = _waypoints;
            current_waypoint = 0;
            pathRequested = false;
        }
    }

    private void OnDrawGizmos()
    {
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