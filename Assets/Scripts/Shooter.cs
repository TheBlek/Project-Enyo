using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform shoot_pos;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject muzzleflash_prefab;

    public void Shoot()
    {
        Instantiate(bullet_prefab, shoot_pos.position, shoot_pos.rotation);
        var muzzleflash = Instantiate(muzzleflash_prefab, shoot_pos.position, Quaternion.Euler(shoot_pos.rotation.eulerAngles));
        Destroy(muzzleflash, 0.5f);
    }
}
