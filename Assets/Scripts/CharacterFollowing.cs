using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFollowing : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed;
    [SerializeField] private float z_offset;
    private void Start()
    {
        transform.position = player.position;
        transform.Translate(Vector3.back * z_offset);
    }

    private void Update()
    {
        Vector3 target = player.position + Vector3.back * z_offset;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
    }
}
