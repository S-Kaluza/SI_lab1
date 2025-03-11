using UnityEngine;

public class Node
{
    public int iGridX;
    public int iGridY;
    public bool bIsWall;
    public Vector3 vPosition;

    public Node ParentNode;
    public int gCost;
    public int hCost;

    public int FCost { get { return gCost + hCost; } }

    public Node(bool _isWall, Vector3 _pos, int _gridX, int _gridY)
    {
        bIsWall = _isWall;
        vPosition = _pos;
        iGridX = _gridX;
        iGridY = _gridY;
    }
}
