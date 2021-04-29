using UnityEngine;

[RequireComponent(typeof(Damagable))]
public class AutoHealer : MonoBehaviour
{
    [SerializeField] private float time_delay;
    [SerializeField] private float recovery_per_second;

    private Damagable subject;

    private float time_since_damage;

    private void Start()
    {
        subject = GetComponent<Damagable>();

        subject.onDamage += ResetCooldown;
    }

    private void Update()
    {
        time_since_damage += Time.deltaTime;

        if (time_since_damage >= time_delay)
            subject.Heal(recovery_per_second * Time.deltaTime);
    }

    private void ResetCooldown()
    {
        time_since_damage = 0;
    }
}
