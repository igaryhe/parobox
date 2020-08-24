using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject wall;
    public GameObject board;
    public GameObject player;
    public GameObject box;
    public GameObject playerGoal;
    public GameObject boxGoal;

    private void Start()
    {
        for (var j = 0; j < GameManager.Instance.size; j++)
            for (var i = 0; i < GameManager.Instance.size; i++)
                DrawRect(GameManager.Instance.board[i][j], j, i);
    }

    private void DrawRect(char type, int x, int y)
    {
        var scale = transform.lossyScale.x / GameManager.Instance.size;
        if (scale > 0.001)
        {
            var pos = new Vector3(x * scale, -y * scale, 0) + transform.position;
            var newScale = new Vector3(scale, scale, 1);
            switch (type)
            {
                case 'x':
                    var w = Instantiate(wall, pos, Quaternion.identity);
                    w.transform.localScale = newScale;
                    w.transform.parent = transform;
                    break;
                case 'o':
                    var o = Instantiate(board, pos, Quaternion.identity);
                    o.GetComponent<MoveBox>().pos = new Vector2Int(x, y);
                    o.transform.localScale = newScale;
                    o.transform.parent = transform;
                    break;
                case 'p':
                    var p = Instantiate(player, pos, Quaternion.identity);
                    p.GetComponent<MoveBox>().pos = new Vector2Int(x, y);
                    p.transform.localScale = newScale;
                    p.transform.parent = transform;
                    break;
                case 'b':
                    var b = Instantiate(box, pos, Quaternion.identity);
                    b.GetComponent<MoveBox>().pos = new Vector2Int(x, y);
                    b.transform.localScale = newScale;
                    b.transform.parent = transform;
                    break;
            }

            if (new Vector2Int(x, y) == GameManager.Instance.boxGoal)
            {
                var g = Instantiate(boxGoal, pos, Quaternion.identity);
                g.transform.localScale = newScale;
                g.transform.parent = transform;
            } else if (new Vector2Int(x, y) == GameManager.Instance.playerGoal)
            {
                var g = Instantiate(playerGoal, pos, Quaternion.identity);
                g.transform.localScale = newScale;
                g.transform.parent = transform;
            }
        }
    }
}