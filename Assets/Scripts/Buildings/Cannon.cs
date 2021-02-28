using UnityEngine;
using System.Collections.Generic;

public class Cannon : AttackBuilding
{
    private Animator _animator;

    private new void Start()
    {
        base.Start();
        _animator = _barrel.GetComponent<Animator>();
        _shooter.OnShoot += PlayShootAnimation;
    }

    private void PlayShootAnimation()
    {
        _animator.Play("Shoot");
    }

}
