using UnityEngine;
using System.Collections.Generic;

public class SteerTester : MonoBehaviour
{
    [SerializeField] GameObject _intereset_prefab;
    [SerializeField] GameObject _danger_prefab;

    [SerializeField] int _direction_number;
    [SerializeField] float _desirable_dot;
    [SerializeField] float _threshold;
    [SerializeField] float _vision_radius;

    private Steer _steer;
    private Camera _camera;

    private List<Transform> _interesets;
    private List<Transform> _dangers;

    private void Start()
    {
        _camera = Camera.main;
        _steer = new Steer(_direction_number);
        _interesets = new List<Transform>();
        _dangers = new List<Transform>();
    }

    private void Update()
    {
        Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(_intereset_prefab, pos, Quaternion.identity);
            _interesets.Add(obj.transform);
            obj.SetActive(true);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GameObject obj = Instantiate(_danger_prefab, pos, Quaternion.identity);
            _dangers.Add(obj.transform);
            obj.SetActive(true);
        }


        _steer.SetNewMapUp(); 

        ContextMap interest_map = new ContextMap(_direction_number);
        foreach (Transform danger in _dangers)
        {
            float weight = (_vision_radius - danger.position.magnitude) / _vision_radius;
            interest_map.AddInterestToVectorWithDesirableDot(-danger.position, weight, _desirable_dot);
            interest_map.AddDangerToVectorWithThreshold(danger.position, weight, _threshold);
        }
        _steer.ApplyContextMap(interest_map);

        ContextMap danger_map = new ContextMap(_direction_number);
        foreach (Transform interest in _interesets)
        {
            float weight = (_vision_radius - interest.position.magnitude) / _vision_radius;
            danger_map.AddInterestToVector(interest.position, weight);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector2.zero, 1f);
        Gizmos.DrawWireSphere(Vector2.zero, _vision_radius);
        _steer.VisualizeInterests(Vector2.zero);
        _steer.VisualizeDangers(Vector2.zero);
        _steer.VisualizePickedDirection(Vector2.zero);
    }

}