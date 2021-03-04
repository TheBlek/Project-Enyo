using UnityEngine;
using System.Collections.Generic;

public class Cannon : AttackBuilding
{
    private Animator _shoot_animator;

    private new void Start()
    {
        base.Start();
        _shoot_animator = _barrel.GetComponent<Animator>();
        _shooter.OnShoot += PlayShootAnimation;
    }

    private void PlayShootAnimation()
    {
        _shoot_animator.Play("Shoot");
    }
}
