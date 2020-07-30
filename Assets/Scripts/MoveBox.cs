using UnityEngine;

public class MoveBox : MonoBehaviour
{
    [HideInInspector] public Vector2Int pos;
    
    private void Start()
    {
        GameManager.Instance.movement.AddListener(Move);
    }

    private void Move(Vector2Int current, Vector2Int distance)
    {
        if (current != pos) return;
        pos += distance;
        var scale = transform.lossyScale;
        var position = transform.position;
        position += new Vector3(distance.x * scale.x, -distance.y * scale.y, 0);
        transform.position = position;
    }
}