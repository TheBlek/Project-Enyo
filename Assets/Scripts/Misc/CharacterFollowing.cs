using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFollowing : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smooth_time = 0.08f;
    [SerializeField] private float z_offset = 10f;
    private void Start()
    {
        transform.position = player.position;
        transform.Translate(Vector3.back * z_offset);
    }

    private void FixedUpdate()
    {
        Vector3 target = player.position + Vector3.back * z_offset;
        Vector3 velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smooth_time);
    }
}
