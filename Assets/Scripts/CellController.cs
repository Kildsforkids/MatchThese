using UnityEngine;

public class CellController : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Color specialTileColor = Color.black;

    private BoardController boardController;

    public int x { get; set; }
    public int y { get; set; }
    public TileController Tile
    {
        get
        {
            if (transform.childCount == 0) return null;
            else return transform.GetChild(0).GetComponent<TileController>();
        }
    }
    public int TilesCount { get { return transform.childCount; } }

    private void Start()
    {
        boardController = BoardController.instance;
        CreateTile();
    }

    public void CreateTile()
    {
        Instantiate(tilePrefab, transform.position, Quaternion.identity, transform);
    }

    public void CreateSpecialTile()
    {
        Instantiate(tilePrefab, transform.position, Quaternion.identity, transform);
        Tile.isSpecial = true;
    }

    public Color GetSpecialTileColor()
    {
        return specialTileColor;
    }
}
