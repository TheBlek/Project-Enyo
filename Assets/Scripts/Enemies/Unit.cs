using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Unit")]
public class Unit : ScriptableObject
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _production_cost;
    [SerializeField] private float _production_time;

    public GameObject Prefab => _prefab;
    public int ProductionCost => _production_cost;
    public float ProductionTime => _production_time;
}