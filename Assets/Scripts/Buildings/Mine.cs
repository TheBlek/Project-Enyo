using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Mine : Building
{
    [SerializeField] private int money_per_mineral_tile;

    private float time_since_last_addition = 0;
    private int MpS = 0;
    private GameManager _gameManager;

    private void Awake()
    {
        var d = transform.GetComponent<Damagable>();
        d.death_offset = .5f;
        d.onKill += PlayBlowAnimation;

        _gameManager = FindObjectOfType<GameManager>();
    }

    public override void SelfUpdate()
    {
        time_since_last_addition += Time.deltaTime;
        if (time_since_last_addition >= 1)
        {
            time_since_last_addition -= 1;
            _gameManager.AddMetals(MpS);
        }
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

        return MpS != 0;
    }

    private void CountMinerals(Vector2 position, GameManager gameManager)
    {
        MapCell[] cells = gameManager.GetMapManager().GetCellsInRect(position, GetSize());
        MpS = 0;
        foreach (MapCell cell in cells)
        {
            if (cell.Tile == MapTiles.Minerals)
            {
                MpS += money_per_mineral_tile;
            }
        }
    }
}
