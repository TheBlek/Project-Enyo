using UnityEngine;
using System.Collections.Generic;

public class SteerTester : MonoBehaviour
{
    [SerializeField] GameObject _intereset_prefab;
    [SerializeField] GameObject _danger_prefab;

    [SerializeField] int _direction_number;
    [SerializeField] float _desirable_dot;
    [SerializeField] float _threshold;

    private Steer _steer;
    private Camera _camera;

    private List<Vector2> _interesets;
    private List<Vector2> _dangers;

    private void Start()
    {
        _camera = Camera.main;
        _steer = new Steer(_direction_number);
        _interesets = new List<Vector2>();
        _dangers = new List<Vector2>();
    }

    private void Update()
    {
        Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            _interesets.Add(pos);
            GameObject obj = Instantiate(_intereset_prefab, pos, Quaternion.identity);
            obj.SetActive(true);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _dangers.Add(pos);
            GameObject obj = Instantiate(_danger_prefab, pos, Quaternion.identity);
            obj.SetActive(true);
        }


        _steer.ResetSteerData();

        foreach (Vector2 danger in _dangers)
        {
            float weight = (3f - danger.magnitude) / 3f;
            _steer.AddInterestToVectorWithDesirableDot(-danger, weight, _desirable_dot);
            _steer.AddDangerToVectorWithThreshold(danger, weight, _threshold);
        }

        foreach (Vector2 interest in _interesets)
        {
            float weight = (3f - interest.magnitude) / 3f;
            _steer.AddInterestToVector(interest, weight);
        }

        if (_interesets.Count > 0)
            _steer.NormalizeInterests();
        if (_dangers.Count > 0)
            _steer.NormalizeDangers();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector2.zero, 1f);
        Gizmos.DrawWireSphere(Vector2.zero, 3f);
        _steer.VisualizeInterests(Vector2.zero);
        _steer.VisualizeDangers(Vector2.zero);
        _steer.VisualizePickedDirection(Vector2.zero);
    }

}