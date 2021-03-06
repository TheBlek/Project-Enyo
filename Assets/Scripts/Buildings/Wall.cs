using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{

    [SerializeField] private Sprite[] sprites;
    private int pattern;
    private Building[] _neighbourBuildings;
    protected GameManager gameManager;

    public class RendererSetUp
    {
        public bool Flip_X;
        public bool Flip_Y;
        public Sprite sprite;

        public RendererSetUp(Sprite _sprite, bool flip_X, bool flip_Y)
        {
            sprite = _sprite;
            Flip_X = flip_X;
            Flip_Y = flip_Y;
        }
    }

    private Dictionary<int, RendererSetUp> rendererSetUps;

    private void Awake()
    {
        //Всех соседей можно представитьь в виде набора нулей и едениц. (ноль - нет, один - есть)
        //Чтобы поставить соответствие ситуаций с этими чилами нужно идти от верхнего по часовой стрелке и перевести в десятичную сис счисл
        rendererSetUps = new Dictionary<int, RendererSetUp>
        {
            //{ 0, new RendererSetUp {sprite = sprites[0], Flip_X = false, Flip_Y = false } },
            { 0, new RendererSetUp(sprites[0], false, false) },
            { 1, new RendererSetUp(sprites[4], false, false) },
            { 2, new RendererSetUp(sprites[1], false, false) },
            { 3, new RendererSetUp(sprites[2], false, true) },
            { 4, new RendererSetUp(sprites[4], false, true) },
            { 5, new RendererSetUp(sprites[3], false, false) },
            { 6, new RendererSetUp(sprites[2], false, false) },
            { 7, new RendererSetUp(sprites[8], true, false) },
            { 8, new RendererSetUp(sprites[1], true, false) },
            { 9, new RendererSetUp(sprites[2], true, true) },
            { 10, new RendererSetUp(sprites[5], false, false) },
            { 11, new RendererSetUp(sprites[6], false, false) },
            { 12, new RendererSetUp(sprites[2], true, false) },
            { 13, new RendererSetUp(sprites[8], false, false) },
            { 14, new RendererSetUp(sprites[6], false, true) },
            { 15, new RendererSetUp(sprites[7], false, false) }
        };
        GetComponent<Damagable>().onKill += OnDeath;

        gameManager = FindObjectOfType<GameManager>();
    }

    public override void SetUp()
    {
        AdjustTexture();
        RecallNeighbours();
    }

    public override void AdjustTexture()
    { 
        SetUpNeighbourBuildings();
        pattern = GeneratePattern();
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();

        renderer.sprite = rendererSetUps[pattern].sprite;
        renderer.flipX = rendererSetUps[pattern].Flip_X;
        renderer.flipY = rendererSetUps[pattern].Flip_Y;
    }

    private int GeneratePattern()
    {
        int pattern = 0;
        for (int i = 0; i < 4; i++)
        {
            if (_neighbourBuildings[i] != null && CheckName(_neighbourBuildings[i].GetBuildingType()))
                pattern += (int)Mathf.Pow(2f, i);
        }
        return pattern;
    }

    private void SetUpNeighbourBuildings()
    {
        var grid = gameManager.GetMapManager();
        MapCell[] neighbourCells = grid.GetStraightNeighbours(grid.GetGridPositionFromGlobal(transform.position));
        _neighbourBuildings = new Building[neighbourCells.Length];

        for (int i = 0; i < _neighbourBuildings.Length; i++)
        {
            if (neighbourCells[i] == null)
            {
                _neighbourBuildings[i] = null;
                continue;
            }
            _neighbourBuildings[i] = neighbourCells[i].BuildingInCell;
        }
    }

    private void RecallNeighbours()
    {
        foreach (Building building in _neighbourBuildings)
        {
            try
            {
                if (CheckName(building.GetBuildingType()))
                    building.AdjustTexture();
            }
            catch { }
        }
    }

    private void OnDeath()
    {
        type = Buildings.AboutToDie;
        RecallNeighbours();
    }

    private bool CheckName(Buildings type)
    {
        return type == Buildings.Wall || type == Buildings.Gate;
    }
}
