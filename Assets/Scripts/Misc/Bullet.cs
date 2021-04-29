using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float _speed = 400;
    [SerializeField] protected float damage = 50;
    [SerializeField] protected float life_time = 1;

    public Vector2 Target;

    protected Animator _animator;
    private bool is_in_animation;

    public bool is_enemy;

    private void Start()
    {
        Destroy(gameObject, life_time);
        transform.GetComponent<Rigidbody2D>().AddForce(transform.right * _speed);
        _animator = transform.GetComponent<Animator>();
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
        if (is_in_animation)
            return;

        if (collider.TryGetComponent(out Damagable component) && component.is_enemy != is_enemy)
                component.TakeDamage(damage);

        HandleBlowAnimation();
        Destroy(gameObject, .5f);
    }

    protected void HandleBlowAnimation()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        Destroy(GetComponent<Collider2D>());
        
        is_in_animation = true;
        
        _animator.Play("Blow");
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
