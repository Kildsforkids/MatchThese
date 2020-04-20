using UnityEditor;

[CustomEditor(typeof(BoardController))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BoardController board = target as BoardController;

        board.GenerateBoard();
    }
}
