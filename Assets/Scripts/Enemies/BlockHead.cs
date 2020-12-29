using System.Collections;
using UnityEngine;
public class BlockHead : Enemy
{
    [SerializeField] private float knock_back;
    [SerializeField] private float knock_back_cooldown;

    private Rigidbody2D rig;
    private bool stop;

    private void Start()
    {
        rig = transform.GetComponent<Rigidbody2D>();
    }

    public override void SelfUpdate(GameManager gameManager)
    {
        if (stop)
            return;

        Vector2 playerPosition = gameManager.GetPlayerPosition();
        Vector2 relativePos = (Vector2)transform.position - playerPosition;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        transform.eulerAngles = Vector3.forward * (angle + 180);
        if (relativePos.magnitude > 0.3f)
            rig.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
    }

    private void StartKnockBack()
    {
        StartCoroutine("KnockBack");
    }

    IEnumerator KnockBack()
    {
        rig.MovePosition(transform.position - transform.right * knock_back);
        stop = true;
        yield return new WaitForSeconds(knock_back_cooldown);
        stop = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject collider)
    {
        try
        {
            collider.GetComponent<Damagable>().TakeDamage(damage);
            StartKnockBack();
        }
        catch { };
    }

}