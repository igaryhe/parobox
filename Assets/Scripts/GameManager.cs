using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{

    public int level;
    public GameObject boardPrefab;
    
    [HideInInspector] public char[][] board;
    [HideInInspector] public int size;
    [HideInInspector] public Vector2Int pos;
    [HideInInspector] public MoveEvent movement = new MoveEvent();
    
    private Vector2Int _entranceDir;
    private Vector2Int _entrancePos;
    [HideInInspector] public Vector2Int recPos;

    private readonly Stack<Command> _commands = new Stack<Command>();
    private const string Dir = "Assets/Levels/";

    protected override void Awake()
    {
        base.Awake();
        var lines = System.IO.File.ReadAllLines(Dir + level + ".txt");
        var list = new List<char[]>();
        foreach (var s in lines) { list.Add(s.ToCharArray()); }
        
        _entranceDir = CharToVec2(list[0][0]);
        list.RemoveAt(0);
        
        board = list.ToArray();
        size = board[0].Length;
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                var curPos = new Vector2Int(j, i);
                switch (board[i][j])
                {
                    case 'p':
                        pos = curPos;
                        break;
                    case 'e':
                        _entrancePos = curPos;
                        board[i][j] = '.';
                        break;
                    case 'o':
                        recPos = curPos;
                        break;
                }
            }
        }
    }

    private void Start()
    {
        var b = Instantiate(boardPrefab, new Vector3(0, size, 0),
            quaternion.identity);
        b.transform.localScale = new Vector3(size, size, 1);
        Camera.main.transform.position = new Vector3(recPos.x + 0.5f, size - (recPos.y + 0.5f), -10);
    }
    
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        var tmp = ctx.ReadValue<Vector2>();
        var input = new Vector2Int((int) tmp.x, -(int) tmp.y);
        TryMove(input);
    }

    public void OnUndo(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (_commands.Count == 0) return;
        var cmd = _commands.Pop();
        cmd.Undo();
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        while (_commands.Count != 0)
        {
            var cmd = _commands.Pop();
            cmd.Undo();
        }
    }
    
    private void TryMove(Vector2Int direction)
    {
        var count = TryPush(pos, direction);
        if (count < 0) return;
        var move = new MoveCommand(direction, count);
        move.Execute();
        _commands.Push(move);
    }

    private int TryPush(Vector2Int current, Vector2Int direction)
    {
        var next = NextPos(current, direction);
        switch (board[next.y][next.x])
        {
            case 'x': return -10;
            case '.': return 0;
            default: return TryPush(next, direction) + 1;
        }
    }
    

    public Vector2Int NextPos(Vector2Int current, Vector2Int direction)
    {
        if (current == _entrancePos && direction == _entranceDir)
            return recPos + _entranceDir;
        if (current == recPos + _entranceDir && direction == -_entranceDir)
            return _entrancePos;
        return current + direction;
    }

    public Vector2Int NextNPos(Vector2Int current, Vector2Int direction, int n)
    {
        var pos = current;
        for (int i = 0; i != n; i++)
            pos = NextPos(pos, direction);
        return pos;
    }
    
    private static Vector2Int CharToVec2(char side)
    {
        switch (side)
        {
            case 'l':
                return Vector2Int.left;
            case 'r':
                return Vector2Int.right;
            case 'u':
                return Vector2Int.up;
            default:
                return Vector2Int.down;
        }
    }
}