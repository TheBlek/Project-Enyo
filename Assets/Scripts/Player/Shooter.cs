using UnityEngine;
using System;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform shoot_pos;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject muzzleflash_prefab;
    [SerializeField] private float fires_per_sec = 9;

    private float delay;
    private float time_since_last_shot;

    public Action OnShoot;

    private void Start()
    {
        delay = 1/fires_per_sec;
        OnShoot += HandleMuzzleflash;
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
        OnShoot();

        time_since_last_shot = 0;
    }

    private void HandleMuzzleflash()
    {
        var muzzleflash = Instantiate(muzzleflash_prefab, shoot_pos.position, Quaternion.Euler(shoot_pos.rotation.eulerAngles));
        Destroy(muzzleflash, 0.5f);
    }
}
