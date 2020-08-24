using UnityEngine;

public class MoveCommand : Command
{
    private readonly Vector2Int _direction;
    private readonly int _length;
    
    public MoveCommand(Vector2Int direction, int length)
    {
        _direction = direction;
        _length = length;
    }
    
    public override void Execute()
    {
        var pos = GameManager.Instance.playerPos;
        for (var i = _length; i != -1; i--)
        {
            var current = GameManager.Instance.NextNPos(pos, _direction, i);
            var next = GameManager.Instance.NextPos(current, _direction);
            if (GameManager.Instance.board[current.y][current.x] == 'o')
            {
                Camera.main.transform.position -= new Vector3(_direction.x, _direction.y, 0);
                GameManager.Instance.recursivePos = next;
            }
            GameManager.Instance.movement.Invoke(current, next - current);
            GameManager.Instance.board[next.y][next.x] = GameManager.Instance.board[current.y][current.x];
        }
        GameManager.Instance.board[pos.y][pos.x] = '.';
        var newPos = GameManager.Instance.NextPos(pos, _direction);
        GameManager.Instance.playerPos = newPos;
    }

    public override void Undo()
    {
        var pos = GameManager.Instance.playerPos;
        for (var i = 0; i != _length + 1; i++)
        {
            var current = GameManager.Instance.NextNPos(pos, _direction, i);
            var next = GameManager.Instance.NextPos(current, -_direction);
            if (GameManager.Instance.board[current.y][current.x] == 'o')
            {
                Camera.main.transform.position += new Vector3(_direction.x, _direction.y, 0);
                GameManager.Instance.recursivePos = next;
            }
            GameManager.Instance.movement.Invoke(current, next - current);
            GameManager.Instance.board[next.y][next.x] = GameManager.Instance.board[current.y][current.x];
        }

        var last = GameManager.Instance.NextNPos(pos, _direction, _length);
        GameManager.Instance.board[last.y][last.x] = '.';
        var prevPos = GameManager.Instance.NextPos(pos, -_direction);
        GameManager.Instance.playerPos = prevPos;
    }
}