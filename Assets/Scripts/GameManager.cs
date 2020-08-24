using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{

    // 关卡编号
    public int level;
    // 背景 Prefab
    public GameObject boardPrefab;
    
    // 用于保存关卡状态的二维数组
    [HideInInspector] public char[][] board;
    // 关卡边长
    [HideInInspector] public int size;
    // 用于保存角色位置的变量
    [HideInInspector] public Vector2Int playerPos;
    // 用于保存递归方块位置的变量
    [HideInInspector] public Vector2Int recursivePos;
    // 用于保存箱子位置的变量
    [HideInInspector] public Vector2Int boxPos;
    // 移动事件
    [HideInInspector] public MoveEvent movement = new MoveEvent();

    // 箱子的胜利条件
    public Vector2Int boxGoal;
    // 玩家胜利条件
    public Vector2Int playerGoal;
    
    // 入口方向
    private Vector2Int _entranceDir;
    // 入口位置
    private Vector2Int _entrancePos;

    // 命令栈
    private readonly Stack<Command> _commands = new Stack<Command>();
    // 关卡文件路径前缀
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
                        playerPos = curPos;
                        break;
                    case 'b':
                        boxPos = curPos;
                        break;
                    case 'e':
                        _entrancePos = curPos;
                        board[i][j] = '.';
                        break;
                    case 'o':
                        recursivePos = curPos;
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
        Camera.main.transform.position = new Vector3(recursivePos.x + 0.5f, size - (recursivePos.y + 0.5f), -10);
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
        var count = TryPush(playerPos, direction);
        if (count < 0) return;
        var move = new MoveCommand(direction, count);
        move.Execute();
        _commands.Push(move);
        if (IsFinished()) Debug.Log("Win!");
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
            return recursivePos + _entranceDir;
        if (current == recursivePos + _entranceDir && direction == -_entranceDir)
            return _entrancePos;
        return current + direction;
    }

    public Vector2Int NextNPos(Vector2Int current, Vector2Int direction, int n)
    {
        var p = current;
        for (int i = 0; i != n; i++)
            p = NextPos(p, direction);
        return p;
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

    private bool IsFinished()
    {
        return playerPos == playerGoal && boxPos == boxGoal;
    }
}