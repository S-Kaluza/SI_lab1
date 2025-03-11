using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public Transform StartPosition;
    public Transform Target;
    public Grid grid;

    void Start()
    {
        Debug.Log("Rozpoczynam inicjalizację AStar...");

        // Automatyczne wykrywanie Grid
        if (grid == null)
        {
            grid = Object.FindFirstObjectByType<Grid>();
            if (grid != null)
            {
                Debug.Log("Grid znaleziony: " + grid.name);
            }
            else
            {
                Debug.LogError("ERROR: Grid nie został znaleziony! Dodaj go do sceny.");
            }
        }

        // Automatyczne wykrywanie obiektu Target (po nazwie i po tagu)
        if (Target == null)
        {
            Target = GameObject.Find("Target")?.transform;
            if (Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("Target")?.transform;
            }

            if (Target != null)
            {
                Debug.Log("Target przypisany automatycznie: " + Target.name);
            }
            else
            {
                Debug.LogError("ERROR: Nie znaleziono obiektu 'Target'! Sprawdź jego nazwę w Hierarchy.");
            }
        }
    }

    private void Update()
    {
        if (Target == null)
        {
            Debug.LogError("ERROR: Target nadal jest null w Update!");
            return;
        }

        if (StartPosition == null)
        {
            Debug.LogError("ERROR: StartPosition jest null! Upewnij się, że przypisałeś AI do pola w Inspectorze.");
            return;
        }

        Debug.Log("AI zmierza do: " + Target.position);
        FindPath(StartPosition.position, Target.position);
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (grid == null)
        {
            Debug.LogError("ERROR: Grid nadal jest null w FindPath!");
            return;
        }

        Debug.Log("Znaleziono Grid. Szukam ścieżki od " + startPos + " do " + targetPos);

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null)
        {
            Debug.LogError("ERROR: StartNode lub TargetNode są null w FindPath! Sprawdź, czy Grid poprawnie mapuje scenę.");
            return;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.NodeArray)
            {
                if (neighbor.bIsWall || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCost = currentNode.gCost + GetManhattanDistance(currentNode, neighbor);
                if (newMovementCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCost;
                    neighbor.hCost = GetManhattanDistance(neighbor, targetNode);
                    neighbor.ParentNode = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        path.Reverse();
        grid.FinalPath = path;
    }

    int GetManhattanDistance(Node nodeA, Node nodeB)
    {
        int x = Mathf.Abs(nodeA.iGridX - nodeB.iGridX);
        int y = Mathf.Abs(nodeA.iGridY - nodeB.iGridY);
        return x + y;
    }
}
