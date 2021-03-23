using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Mine : Building
{
    [SerializeField] private int money_per_mineral_tile;

    private GameManager _gameManager;

    private void Awake()
    {
        var d = transform.GetComponent<Damagable>();
        d.death_offset = .5f;
        d.onKill += PlayBlowAnimation;

        _gameManager = FindObjectOfType<GameManager>();
    }

    public override void SetUp()
    {
        base.SetUp();

        CountMinerals(transform.position, _gameManager);
    }

    private void PlayBlowAnimation()
    {
        transform.GetComponent<Animator>().Play("Blow");
    }

    public override bool IsPositionAcceptable(GameManager gameManager, Vector2 position = default)
    {
        if (position == default)
            position = transform.position;

        CountMinerals(position, gameManager);

        return _maintenance_cost != 0;
    }

    private void CountMinerals(Vector2 position, GameManager gameManager)
    {
        MapCell[] cells = gameManager.GetMapManager().GetCellsInRect(position, GetSize());
        _maintenance_cost = 0;
        foreach (MapCell cell in cells)
        {
            if (cell.Tile == MapTiles.Minerals)
            {
                _maintenance_cost -= money_per_mineral_tile;
            }
        }
    }

    public float Earnings => -_maintenance_cost;
}
