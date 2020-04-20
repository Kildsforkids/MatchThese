using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
    public Color color { get; set; } = Color.white;
    public bool isShifting { get; set; } = false;
    public bool isSpecial { get; set; } = false;

    private SpriteRenderer spriteRenderer;
    private BoardController boardController;
    private bool isSelected = false;
    private float selectColor = .5f;

    private const float moveDuration = .75f;

    private void Start()
    {
        boardController = BoardController.instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!isSpecial)
        {
            SetColor(boardController.GetRandomTileColor());
            boardController.GetNoMatchingColor(GetComponent<TileController>(), transform.parent.GetComponent<CellController>());
            boardController.GetNoMatchingColor(GetComponent<TileController>(), transform.parent.GetComponent<CellController>(), true);
        }
        else
            SetColor(transform.parent.GetComponent<CellController>().GetSpecialTileColor());
        
        StartCoroutine(ChangeScale(.5f, 1f));
    }

    private void OnMouseDown()
    {
        if (!isShifting && !boardController.isPaused)
            Select();
    }

    private IEnumerator ChangeScale(float lowerBound, float duration)
    {
        isShifting = true;
        float t = 0;
        Vector3 startScale = new Vector3(lowerBound, lowerBound, 1);
        Vector3 finalScale = transform.localScale;
        while (t <= 1.0f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, finalScale, t / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, t / duration);
            yield return new WaitWhile(() => boardController.isPaused);
        }
        transform.localScale = finalScale;
        SetDefaultColor();
        isShifting = false;
    }

    public void Deselect()
    {
        SetDefaultColor();
        boardController.previousTile = null;
        isSelected = false;
    }

    public void Select()
    {
        if (isSelected)
        {
            if (!isSpecial)
                Deselect();
            else
            {
                boardController.ResetHelpTile(this);
                int x = transform.parent.GetComponent<CellController>().x;
                int y = transform.parent.GetComponent<CellController>().y;
                boardController.DestroyTile(boardController.GetCellController(x - 1, y));
                boardController.DestroyTile(boardController.GetCellController(x, y - 1));
                boardController.DestroyTile(boardController.GetCellController(x + 1, y));
                boardController.DestroyTile(boardController.GetCellController(x, y + 1));
                if (y == boardController.GetBoardHeight() - 1)
                    boardController.DestroyTile(boardController.GetCellController(x, y));
                else
                    boardController.DestroyTile(boardController.GetCellController(x, y + 1));
            }
        }
        else
        {
            if (boardController.previousTile == null)
            {
                boardController.previousTile = gameObject;
                SetSelectColor();
                isSelected = true;
            }
            else
            {
                boardController.ResetHelpTile(this);
                int x = transform.parent.GetComponent<CellController>().x;
                int y = transform.parent.GetComponent<CellController>().y;
                int previousTileX = boardController.previousTile.transform.parent.GetComponent<CellController>().x;
                int previousTileY = boardController.previousTile.transform.parent.GetComponent<CellController>().y;
                if (Mathf.Abs(x - previousTileX) == 1 && y - previousTileY == 0 ||
                    Mathf.Abs(y - previousTileY) == 1 && x - previousTileX == 0)
                {
                    Transform previousTileTransform = boardController.previousTile.transform;
                    previousTileTransform.GetComponent<TileController>().Deselect();
                    boardController.SwapTiles(transform, previousTileTransform, moveDuration);
                    boardController.previousTile = null;
                }
                else
                {
                    boardController.previousTile.GetComponent<TileController>().Deselect();
                    boardController.previousTile = gameObject;
                    SetSelectColor();
                    isSelected = true;
                }
            }
        }
    }

    private void SetColor(Color newColor)
    {
        color = newColor;
        SetDefaultColor();
    }

    public void SetSelectColor()
    {
        spriteRenderer.color = new Color(color.r - selectColor, color.g - selectColor, color.b - selectColor);
    }

    public void SetDefaultColor()
    {
        spriteRenderer.color = color;
    }
}
