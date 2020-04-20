using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private Text stepsLeftText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text taskText;
    [SerializeField]
    private Text pauseButtonText;
    [SerializeField]
    private Text musicButtonText;
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private bool withTimer = false;
    [SerializeField]
    private Color taskColor = Color.white;
    [SerializeField]
    private int taskLeft = 5;
    [SerializeField]
    private int stepsLeft = 5;
    [SerializeField]
    private int helpTimer = 3;
    [SerializeField]
    private int scorePerTile = 10;
    [SerializeField]
    private int boardWidth = 10;
    [SerializeField]
    private int boardHeight = 6;
    [SerializeField]
    private Color[] tileColors;

    private CellController[,] cells;
    private float timer = 0;
    private int previousTime = 0;
    private int score = 0;
    private bool isMusicPlaying = false;
    private bool isGameOver = false;
    private bool isWin = false;
    private Scene scene;
    private bool can = false;
    private bool isSecondLeft = false;
    private bool helpControllerWasChecked = false;

    private const float moveDuration = .5f;

    public bool isPaused { get; set; } = false;
    public bool needToReturn { get; set; } = true;
    public GameObject previousTile { get; set; }
    public TileController helpTileController { get; set; }

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        instance = GetComponent<BoardController>();
        GenerateBoard();
        stepsLeftText.text = stepsLeft.ToString();
        taskText.text = taskLeft.ToString();
        taskColor = GetRandomTileColor();
        taskText.color = taskColor;
        panel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "AGAIN";
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (can)
            {
                helpControllerWasChecked = false;
                for (int y = 0; y < boardHeight; y++) CheckLine(y);
                if (helpControllerWasChecked)
                    if (helpTileController == null)
                        GenerateBoard();
            }
            if (isSecondLeft) isSecondLeft = false;
            timer += Time.deltaTime;
            if (timer > 1000) timer = 0;
            if (Mathf.RoundToInt(timer) > previousTime)
            {
                previousTime = Mathf.RoundToInt(timer);
                isSecondLeft = true;
            }
            if (withTimer)
            {
                if (isSecondLeft)
                {
                    stepsLeft--;
                    stepsLeftText.text = stepsLeft.ToString();
                    if (stepsLeft <= 10) stepsLeftText.color = Color.red;
                    if (stepsLeft <= 0) GameOver();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
                Restart();
        }
        if (Input.GetKeyDown(KeyCode.Space))
            SetPause(2);
        if (!can) can = true;
    }

    public void GenerateBoard()
    {
        cells = new CellController[boardWidth, boardHeight];
        string holderName = "Generated Board";
        GameObject board = transform.Find(holderName).gameObject;
        if (board != null)
            DestroyImmediate(board);

        board = new GameObject(holderName);
        board.transform.parent = transform;

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector2 cellPosition = new Vector2(-boardWidth / 2 + .5f + x, boardHeight / 2 - .5f - y);
                GameObject newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, board.transform);
                newCell.name = "cell(" + x + "," + y + ")";
                CellController cellController = newCell.GetComponent<CellController>();
                cellController.x = x;
                cellController.y = y;
                cells[x, y] = cellController;
            }
        }
    }

    private void Help(CellController cellController)
    {
        if (helpTileController == null)
        {
            if (cellController != null)
            {
                if (cellController.TilesCount > 0)
                {
                    CellController neighboringCellController1 = null;
                    CellController neighboringCellController2 = null;
                    neighboringCellController1 = GetCellController(cellController.x + 1, cellController.y + 1);
                    neighboringCellController2 = GetCellController(cellController.x + 1, cellController.y - 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x - 1, cellController.y + 1);
                    neighboringCellController2 = GetCellController(cellController.x - 1, cellController.y - 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x - 1, cellController.y + 1);
                    neighboringCellController2 = GetCellController(cellController.x + 1, cellController.y + 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x - 1, cellController.y - 1);
                    neighboringCellController2 = GetCellController(cellController.x + 1, cellController.y - 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x + 1, cellController.y + 1);
                    neighboringCellController2 = GetCellController(cellController.x + 2, cellController.y + 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x + 1, cellController.y - 1);
                    neighboringCellController2 = GetCellController(cellController.x + 2, cellController.y - 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x - 1, cellController.y + 1);
                    neighboringCellController2 = GetCellController(cellController.x - 2, cellController.y + 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x - 1, cellController.y - 1);
                    neighboringCellController2 = GetCellController(cellController.x - 2, cellController.y - 1);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x + 1, cellController.y);
                    neighboringCellController2 = GetCellController(cellController.x + 2, cellController.y);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x - 1, cellController.y);
                    neighboringCellController2 = GetCellController(cellController.x - 2, cellController.y);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x, cellController.y + 1);
                    neighboringCellController2 = GetCellController(cellController.x, cellController.y + 2);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    neighboringCellController1 = GetCellController(cellController.x, cellController.y - 1);
                    neighboringCellController2 = GetCellController(cellController.x, cellController.y - 2);
                    if (neighboringCellController1 != null && neighboringCellController2 != null)
                    {
                        if (neighboringCellController1.TilesCount > 0 && neighboringCellController2.TilesCount > 0)
                        {
                            if (neighboringCellController1.Tile.color == cellController.Tile.color &&
                                neighboringCellController2.Tile.color == cellController.Tile.color)
                            {
                                helpTileController = cellController.Tile;
                            }
                        }
                    }
                    if (helpTileController != null)
                    {
                        helpTileController.SetSelectColor();
                        helpControllerWasChecked = true;
                    }
                }
            }
        }
    }

    public Color GetRandomTileColor()
    {
        return tileColors[Random.Range(0, tileColors.Length)];
    }

    public CellController GetCellController(int x, int y)
    {
        if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight)
            return cells[x, y];
        else
            return null;
    }

    public void GetNoMatchingColor(TileController tileController, CellController cellController, bool isUpSide = false)
    {
        CellController neighboringCellController = null;
        if ((neighboringCellController = GetCellController(
            isUpSide ? cellController.x : cellController.x + 1,
            isUpSide ? cellController.y + 1 : cellController.y)) != null)
        {
            if (neighboringCellController.TilesCount > 0)
            {
                Color newColor = tileController.color;
                if (newColor == neighboringCellController.Tile.color)
                {
                    if ((neighboringCellController = GetCellController(
                        isUpSide ? cellController.x : cellController.x + 2,
                        isUpSide ? cellController.y + 2 : cellController.y)) != null)
                    {
                        if (neighboringCellController.TilesCount > 0)
                        {
                            Color neighboringCellColor = neighboringCellController.Tile.color;
                            while (newColor == neighboringCellColor)
                            {
                                newColor = GetRandomTileColor();
                            }
                            tileController.color = newColor;
                            tileController.SetDefaultColor();
                        }
                    }
                }
            }
        }
        if ((neighboringCellController = GetCellController(
            isUpSide ? cellController.x : cellController.x - 1,
            isUpSide ? cellController.y - 1 : cellController.y)) != null)
        {
            if (neighboringCellController.TilesCount > 0)
            {
                Color newColor = tileController.color;
                if (newColor == neighboringCellController.Tile.color)
                {
                    if ((neighboringCellController = GetCellController(
                        isUpSide ? cellController.x : cellController.x - 2,
                        isUpSide ? cellController.y - 2 : cellController.y)) != null)
                    {
                        if (neighboringCellController.TilesCount > 0)
                        {
                            Color neighboringCellColor = neighboringCellController.Tile.color;
                            while (newColor == neighboringCellColor)
                            {
                                newColor = GetRandomTileColor();
                            }
                            tileController.color = newColor;
                            tileController.SetDefaultColor();
                        }
                    }
                }
            }
        }
        if ((neighboringCellController = GetCellController(
            isUpSide ? cellController.x : cellController.x - 1,
            isUpSide ? cellController.y - 1 : cellController.y)) != null)
        {
            if (neighboringCellController.TilesCount > 0)
            {
                Color newColor = tileController.color;
                if (newColor == neighboringCellController.Tile.color)
                {
                    if ((neighboringCellController = GetCellController(
                        isUpSide ? cellController.x : cellController.x + 1,
                        isUpSide ? cellController.y + 1 : cellController.y)) != null)
                    {
                        if (neighboringCellController.TilesCount > 0)
                        {
                            Color neighboringCellColor = neighboringCellController.Tile.color;
                            while (newColor == neighboringCellColor)
                            {
                                newColor = GetRandomTileColor();
                            }
                            tileController.color = newColor;
                            tileController.SetDefaultColor();
                        }
                    }
                }
            }
        }
    }

    private void StartMoveFromUpperCellCoroutine(CellController cellController)
    {
        if (cellController.TilesCount == 0)
        {
            CellController upperCellController = null;
            upperCellController = GetUpperCellController(cellController);
            if (upperCellController != null)
            {
                if (upperCellController.TilesCount > 0)
                    StartCoroutine(MoveFromUpperCell(upperCellController.Tile.transform, cellController.transform, moveDuration));
            }
        }
    }

    private CellController GetLowerCellController(CellController cellController)
    {
        int k = 1;
        CellController lowerCellController = GetCellController(cellController.x, boardHeight - 1);
        while (cellController.y + k < boardHeight)
        {
            if (GetCellController(cellController.x, cellController.y + k) != null)
                if (GetCellController(cellController.x, cellController.y + k).TilesCount > 0)
                {
                    break;
                }
            lowerCellController = GetCellController(cellController.x, cellController.y + k);
            k++;
        }
        return lowerCellController;
    }

    private void CheckLine(int y)
    {
        CellController cellController = null;
        for (int x = 0; x < boardWidth; x++)
        {
            cellController = GetCellController(x, y);
            if (cellController.TilesCount == 0)
            {
                if (y == 0)
                {
                    cellController.CreateTile();
                    CellController lowerCellController = GetLowerCellController(cellController);
                    if (lowerCellController != null)
                        StartMoveFromUpperCellCoroutine(lowerCellController);
                }
                else
                {
                    StartMoveFromUpperCellCoroutine(cellController);
                }
            }
            if (isSecondLeft && previousTime % helpTimer == 0)
            {
                Help(cellController);
            }
        }
    }

    public void GetMatch(TileController tileController, bool isVertical = false)
    {
        if (tileController.transform.parent == null) return;
        List<CellController> cells = new List<CellController>();
        CellController cellController = tileController.transform.parent.GetComponent<CellController>();
        CellController neighboringCellController = null;
        CellController downCellController = cellController;
        Color tileColor = tileController.color;
        int k = 1;
        while ((neighboringCellController = GetCellController(
            isVertical ? cellController.x : cellController.x - k,
            isVertical ? cellController.y - k : cellController.y)) != null)
        {
            if (neighboringCellController.TilesCount == 0) break;
            if (tileController.color != neighboringCellController.Tile.color) break;
            cells.Add(neighboringCellController);
            k++;
        }
        k = 1;
        while ((neighboringCellController = GetCellController(
            isVertical ? cellController.x : cellController.x + k,
            isVertical ? cellController.y + k : cellController.y)) != null)
        {
            if (neighboringCellController.TilesCount == 0) break;
            if (tileController.color != neighboringCellController.Tile.color) break;
            cells.Add(neighboringCellController);
            k++;
        }
        if (cells.Count > 1)
        {
            foreach (CellController cell in cells)
            {
                Transform temp = cell.Tile.transform;
                Destroy(cell.Tile.gameObject);
                temp.parent = null;
                if (!isVertical)
                {
                    StartMoveFromUpperCellCoroutine(cell);
                }
                else
                {
                    if (cell.y >= downCellController.y) downCellController = cell;
                }
            }
            int i = 0;
            if (tileController.gameObject != null)
            {
                Transform temp = tileController.transform;
                Destroy(tileController.gameObject);
                temp.parent = null;
                i = 1;
            }
            if (!isVertical)
            {
                if (cells.Count > 2)
                {
                    cellController.CreateSpecialTile();
                    CellController upperCellController = GetUpperCellController(cellController);
                    if (upperCellController != null)
                        StartMoveFromUpperCellCoroutine(cellController);
                }
                else
                    StartMoveFromUpperCellCoroutine(cellController);
            }
            else
            {
                if (cells.Count > 2)
                {
                    downCellController.CreateSpecialTile();
                    CellController upperCellController = GetUpperCellController(downCellController);
                    if (upperCellController != null)
                        StartMoveFromUpperCellCoroutine(upperCellController);
                }
                else
                    StartMoveFromUpperCellCoroutine(downCellController);
            }
            if (!withTimer)
            {
                if (needToReturn)
                {
                    stepsLeft--;
                    stepsLeftText.text = stepsLeft.ToString();
                }
            }
            if (!GetComponent<AudioSource>().isPlaying && isMusicPlaying)
                GetComponent<AudioSource>().Play();
            if (helpTileController != null)
            {
                helpTileController.SetDefaultColor();
                helpTileController = null;
            }
            AddScore(scorePerTile * (cells.Count + i));
            if (tileColor == taskColor)
            {
                taskLeft -= (cells.Count + i);
                taskLeft = taskLeft < 0 ? 0 : taskLeft;
                taskText.text = taskLeft.ToString();
            }
            needToReturn = false;
        }
        cells.Clear();
        if (taskLeft <= 0)
        {
            Win();
        }
        if (!withTimer)
        {
            if (stepsLeft <= 0)
            {
                GameOver();
            }
        }
    }

    private CellController GetUpperCellController(CellController cellController)
    {
        int k = 1;
        CellController upperCellController = null;
        while (cellController.y - k >= 0)
        {
            upperCellController = GetCellController(cellController.x, cellController.y - k);
            if (upperCellController != null)
                if (upperCellController.TilesCount > 0)
                {
                    break;
                }
            k++;
        }
        return upperCellController;
    }

    private IEnumerator MoveFromUpperCell(Transform tile, Transform cell, float duration, bool withUpperContinue = true)
    {
        if (tile == null) yield break;
        tile.GetComponent<TileController>().isShifting = true;
        tile.parent = cell;
        float t = 0;
        Vector3 startPosition = tile.position;
        if (withUpperContinue)
        {
            CellController cellController = null;
            CellController upperCellController = GetCellController(cell.GetComponent<CellController>().x,
                cell.GetComponent<CellController>().y - 1);
            if (upperCellController == null) yield break;
            cellController = GetUpperCellController(upperCellController);
            if (cellController != null)
                if (cellController.TilesCount > 0)
                    StartCoroutine(MoveFromUpperCell(cellController.Tile.transform, upperCellController.transform, duration));
        }
        while (t <= 1.0f)
        {
            t += Time.deltaTime;
            tile.position = Vector3.Lerp(startPosition, cell.position, t / duration);
            yield return new WaitWhile(() => isPaused);
            if (tile == null) yield break;
        }
        tile.position = cell.position;
        GetMatch(tile.GetComponent<TileController>(), true);
        GetMatch(tile.GetComponent<TileController>());
        if (tile != null)
            tile.GetComponent<TileController>().isShifting = false;
    }

    public void ResetHelpTile(TileController tileController)
    {
        if (helpTileController != null)
        {
            if (helpTileController != tileController)
                helpTileController.SetDefaultColor();
            helpTileController = null;
        }
    }

    public void SwapTiles(Transform tile1, Transform tile2, float duration)
    {
        needToReturn = true;
        StartCoroutine(MoveToCell(tile1, tile1.parent, tile2.parent, duration));
        StartCoroutine(MoveToCell(tile2, tile2.parent, tile1.parent, duration));
    }

    private IEnumerator MoveToCell(Transform tile, Transform tileParent, Transform cell, float duration, bool withCheck = true)
    {
        tile.GetComponent<TileController>().isShifting = true;
        float t = 0;
        Vector3 startPosition = tile.position;
        while (t <= 1.0f)
        {
            t += Time.deltaTime;
            tile.position = Vector3.Lerp(startPosition, cell.position, t / duration);
            yield return new WaitWhile(() => isPaused);
        }
        tile.position = cell.position;
        if (withCheck)
        {
            if (tile.GetComponent<TileController>().isSpecial)
            {
                tile.parent = cell;
                int x = tile.parent.GetComponent<CellController>().x;
                int y = tile.parent.GetComponent<CellController>().y;
                DestroyTile(GetCellController(x - 1, y));
                DestroyTile(GetCellController(x, y - 1));
                DestroyTile(GetCellController(x + 1, y));
                DestroyTile(GetCellController(x, y + 1));
                DestroyTile(GetCellController(x, y));
                yield return new WaitWhile(() => isPaused);
                DestroyTile(tileParent.GetComponent<CellController>());
                if (taskLeft <= 0)
                {
                    Win();
                }
                if (!withTimer)
                {
                    stepsLeft--;
                    stepsLeftText.text = stepsLeft.ToString();
                    if (stepsLeft <= 0)
                    {
                        GameOver();
                    }
                }
                needToReturn = false;
            }
            else
            {
                tile.parent = cell;
                GetMatch(tile.GetComponent<TileController>());
                GetMatch(tile.GetComponent<TileController>(), true);
                yield return new WaitWhile(() => isPaused);
                if (needToReturn)
                {
                    if (tile != null)
                    {
                        tile.parent = tileParent;
                        StartCoroutine(MoveToCell(tile, tileParent, tile.parent, duration, false));
                    }
                }
            }
        }
        if (tile != null)
        {
            tile.GetComponent<TileController>().isShifting = false;
        }
    }

    private void AddScore(int value)
    {
        score += value;
        if (score <= 999)
            scoreText.text = "Score: " + score;
        else
            scoreText.text = "Score: 999+";
    }

    public void DestroyTile(CellController cellController)
    {
        if (cellController != null)
        {
            if (cellController.TilesCount > 0)
            {
                Transform temp = cellController.Tile.transform;
                if (cellController.Tile.color == taskColor)
                {
                    taskLeft -= 1;
                    taskLeft = taskLeft < 0 ? 0 : taskLeft;
                    taskText.text = taskLeft.ToString();
                }
                Destroy(cellController.Tile.gameObject);
                temp.parent = null;
                StartMoveFromUpperCellCoroutine(cellController);
                if (!GetComponent<AudioSource>().isPlaying && isMusicPlaying)
                    GetComponent<AudioSource>().Play();
                AddScore(scorePerTile);
            }
            
        }
    }

    public int GetBoardHeight()
    {
        return boardHeight;
    }

    private void Win()
    {
        if (!isPaused)
        {
            SetPause(1);
            panel.SetActive(true);
            panel.transform.GetChild(0).GetComponent<Text>().text = "CONGRATULATIONS";
            panel.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "NEXT";
            isGameOver = true;
            isWin = true;
        }
    }

    private void GameOver()
    {
        if (!isPaused)
        {
            SetPause(1);
            panel.SetActive(true);
            panel.transform.GetChild(0).GetComponent<Text>().text = "GAME OVER";
            isGameOver = true;
        }
    }

    public void SetPause(int value)
    {
        if (!isGameOver)
        {
            isPaused = value == 2 ? !isPaused : value == 1 ? true : false;
            pauseButtonText.color = isPaused ? Color.black : Color.white;
            panel.SetActive(!panel.activeSelf);
            panel.transform.GetChild(0).GetComponent<Text>().text = "PAUSED";
        }
    }

    public void SetMusic()
    {
        musicButtonText.color = isMusicPlaying ? Color.black : Color.white;
        isMusicPlaying = !isMusicPlaying;
        if (isMusicPlaying)
            Camera.main.GetComponent<AudioSource>().Play();
        else
            Camera.main.GetComponent<AudioSource>().Pause();
    }

    public void Restart()
    {
        if (!isWin)
            SceneManager.LoadScene(scene.buildIndex);
        else
        {
            if (scene.buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(scene.buildIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
