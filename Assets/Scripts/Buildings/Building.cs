using UnityEngine;

[RequireComponent(typeof(Damagable))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Building : MonoBehaviour
{
    [SerializeField] protected Buildings type;
    [SerializeField] protected Vector2 size;
    [SerializeField] protected int cost;
    [SerializeField] protected GameObject _explosion_prefab;
    public bool IsEnemy { get; set; }

    protected void Start()
    {
        var exp = Instantiate(_explosion_prefab, transform.position, Quaternion.identity);
        exp.TryGetComponent(out Explosion explosion);
        explosion.SetRadius(size.x);
        explosion.SetTarget(GetComponent<Damagable>());
    }

    public virtual void SelfUpdate()
    {

    }

    public virtual void SetUp()
    {

    }

    public virtual void AdjustTexture()
    {

    }

    public virtual bool IsPositionAcceptable(GameManager gameManager, Vector2 position=default)
    {
        return true;
    }

    public Vector2 GetSize() => size;

    public Buildings GetBuildingType() => type;

    public int GetCost() => cost;
 
}
