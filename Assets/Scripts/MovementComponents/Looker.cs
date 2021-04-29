using UnityEngine;

public class Looker : MonoBehaviour // Granting seemless looking on target
{
    private Vector2 _target;

    public Vector2 Target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
            IsPointedOnTarget = false;
        }
    }
    public bool IsPointedOnTarget { get; private set; }

    [SerializeField] private Transform _pointer;
    [SerializeField] private float _angular_speed;
    [SerializeField] private float _sight_offset;
    [SerializeField] private bool _instant_pointing;

    private void Update()
    {
        if (IsPointedOnTarget)
            return;

        Vector2 relative_pos = Target - (Vector2)transform.position;
        float target_angle = Mathf.Atan2(relative_pos.y, relative_pos.x) * Mathf.Rad2Deg + _sight_offset;

        if (_instant_pointing)
        {
            _pointer.eulerAngles = Vector3.forward * target_angle;
            IsPointedOnTarget = true;
            return;
        }


        float angle_diff = target_angle - _pointer.eulerAngles.z;

        while (Mathf.Abs(angle_diff) > 180)
        {
            target_angle += Mathf.Sign(_pointer.eulerAngles.z - target_angle) * 360;
            angle_diff = target_angle - _pointer.eulerAngles.z;
        }

        if (Mathf.Abs(angle_diff) > _angular_speed * Time.deltaTime)
            target_angle = _pointer.eulerAngles.z + _angular_speed * Time.deltaTime * Mathf.Sign(angle_diff);
        else
            IsPointedOnTarget = true;

        _pointer.eulerAngles = Vector3.forward * target_angle;
    }
}