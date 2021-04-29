using UnityEngine;
using System;

[RequireComponent(typeof(Walker))]
public class ContextSteerer : MonoBehaviour
{
    public bool ShowGizmos;

    [HideInInspector] public Func<Collider2D, bool> TargetIdentification { get; set; }
    [HideInInspector] public Func<Collider2D, bool> DangerIdentification { get; set; }
    [HideInInspector] public float VisionRadius { get; set; }

    [SerializeField] private int _directions_count;
    [SerializeField] private LayerMask _target_mask;
    [SerializeField] private LayerMask _danger_mask;

    private Walker _walker;
    private Looker _looker;
    private Steer _steer;

    private void Awake()
    {
        _steer = new Steer(_directions_count);
        _walker = GetComponent<Walker>();
        _looker = GetComponent<Looker>();
    }

    private ContextMap TargetPursuingMap()
    {
        ContextMap map = new ContextMap(_steer.DirectionsCount);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, VisionRadius, _target_mask);

        foreach (Collider2D collider in colliders)
        {
            if (TargetIdentification(collider))
            {
                RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, collider.transform.position);
                float distance = Vector2.Distance(transform.position, hits[hits.Length - 1].point);

                float weight = (VisionRadius - distance) / VisionRadius;
                if (weight < 0) continue;

                map.AddInterestToVector(collider.transform.position - transform.position, weight);
            }
        }

        return map;
    }

    private ContextMap ObstructionAvoidanceMap()
    {
        ContextMap map = new ContextMap(_steer.DirectionsCount);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, VisionRadius, _danger_mask);
        foreach (Collider2D collider in colliders)
        {
            if (DangerIdentification(collider))
            {
                RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, collider.transform.position);
                float distance = Vector2.Distance(transform.position, hits[hits.Length - 1].point);

                float weight = (VisionRadius - distance) / VisionRadius;
                if (weight < 0) continue;

                Vector2 danger = collider.transform.position - transform.position;
                map.AddInterestToVectorWithDesirableDot(-danger, weight, 0.65f);
                map.AddDangerToVectorWithThreshold(danger, weight, 0.65f);
            }
        }

        return map;
    }

    private ContextMap ForwardGoingMap()
    {
        ContextMap map = new ContextMap(_steer.DirectionsCount);

        float weight = _steer.GetTheBestDirection().magnitude * 0.1f;
        map.AddInterestToVector(_walker.CurrentDirection, weight);

        return map;
    }

    public void Act()
    {
        _steer.SetNewMapUp();
        _steer.ApplyContextMap(TargetPursuingMap());
        _steer.ApplyContextMap(ObstructionAvoidanceMap());
        _steer.ApplyContextMap(ForwardGoingMap());
        _steer.InterpolateWithOld(Time.deltaTime);

        Vector2 direction = _steer.GetTheBestDirection();

        float speed = direction.magnitude == 0 ? 0 : 1;
        if (direction.magnitude < 0.2f)
            speed = direction.magnitude * 2;

        direction = direction.normalized;

        _looker.Target = (Vector2)transform.position + direction;

        _walker.WalkForward(speed); // it is zero only if direction originally was zero
    }

    private void OnDrawGizmos()
    {
        if (_steer.DirectionsCount == 0)
            return;
        _steer.VisualizeInterests(transform.position);
        _steer.VisualizeDangers(transform.position);
        _steer.VisualizePickedDirection(transform.position);
    }
}