using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected Buildings type;
    [SerializeField] protected Vector2 size;
    [SerializeField] protected int cost;

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
