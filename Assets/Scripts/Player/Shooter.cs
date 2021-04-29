using UnityEngine;
using System;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform _shoot_point;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject muzzleflash_prefab;
    [SerializeField] private float fires_per_sec = 9;

    private float delay;
    private float time_since_last_shot;

    public Action OnShoot;

    private void Start()
    {
        delay = 1/fires_per_sec;
        if (muzzleflash_prefab != null)
            OnShoot += HandleMuzzleflash;
    }

    private void Update()
    {
        time_since_last_shot += Time.deltaTime;
    }

    public void Shoot(Vector2 target=default)
    {
        if (time_since_last_shot < delay)
            return;

        var bullet = Instantiate(bullet_prefab, _shoot_point.position, _shoot_point.rotation);

        if (target != default)
            bullet.GetComponent<Bullet>().Target = target;

        OnShoot();

        time_since_last_shot = 0;
    }

    public bool IsThereObstacleBeforeTarget(Vector2 target)
    {
        var hit = Physics2D.Linecast(_shoot_point.position, target);

        if (hit.transform == null || (Vector2)hit.transform.position == target)
            return false;

        return true;
    }

    private void HandleMuzzleflash()
    {
        var muzzleflash = Instantiate(muzzleflash_prefab, _shoot_point.position, Quaternion.Euler(_shoot_point.rotation.eulerAngles));
        Destroy(muzzleflash, 0.5f);
    }
}
