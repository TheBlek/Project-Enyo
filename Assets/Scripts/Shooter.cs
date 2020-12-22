using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform shoot_pos;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject muzzleflash_prefab;
    [SerializeField] private float fires_per_sec;

    private float delay;
    private float time_since_last_shot;

    private void Start()
    {
        delay = 1/fires_per_sec;
    }

    private void Update()
    {
        time_since_last_shot += Time.deltaTime;
    }

    public void Shoot()
    {
        if (time_since_last_shot < delay)
            return;

        Instantiate(bullet_prefab, shoot_pos.position, shoot_pos.rotation);

        var muzzleflash = Instantiate(muzzleflash_prefab, shoot_pos.position, Quaternion.Euler(shoot_pos.rotation.eulerAngles));
        Destroy(muzzleflash, 0.5f);

        time_since_last_shot -= delay;
        if (time_since_last_shot < 0)
            time_since_last_shot = 0;
    }
}
