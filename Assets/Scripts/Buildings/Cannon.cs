using UnityEngine;
using System.Collections.Generic;

public class Cannon : AttackBuilding
{
    [SerializeField] private Animator _blow_animator;
    private Animator _shoot_animator;

    private new void Start()
    {
        base.Start();
        _shoot_animator = _barrel.GetComponent<Animator>();
        _shooter.OnShoot += PlayShootAnimation;

        var d = GetComponent<Damagable>();
        d.death_offset = .25f;
        d.onKill += PlayBlowAnimation;
    }

    private void PlayShootAnimation()
    {
        _shoot_animator.Play("Shoot");
    }

    private void PlayBlowAnimation()
    {
        _blow_animator.Play("Blow");
    }

}
