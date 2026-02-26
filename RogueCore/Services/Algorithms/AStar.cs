using RogueCore.Models;

namespace RogueCore.Services.Algorithms;

public static class AStar
{
    private class Node
    {
        public int X, Y;
        public int G, H;
        public int F => G + H;
        public Node Parent;

        public Node(int x, int y) { X = x; Y = y; }
    }

    public static List<Tile>? FindPath(Tile startTile, Tile targetTile, List<List<Tile>> map)
    {
        int width = map.Count;
        int height = map[0].Count;
        Node[,] nodes = new Node[width, height];
        PriorityQueue<Node, int> openSetPriorityQueue = new PriorityQueue<Node, int>();
        bool[,] closed = new bool[width, height];
        
        Node startNode = new Node(startTile.x, startTile.y) { G = 0 };
        startNode.H = GetDistance(startNode.X, startNode.Y, targetTile.x, targetTile.y);
    
        nodes[startNode.X, startNode.Y] = startNode;
        openSetPriorityQueue.Enqueue(startNode, startNode.F);
        
        while (openSetPriorityQueue.Count > 0)
        {
            Node currentNode = openSetPriorityQueue.Dequeue();

            // If we already visited this tile with a better path, skip this "stale" node
            if (closed[currentNode.X, currentNode.Y]) continue;

            if (currentNode.X == targetTile.x && currentNode.Y == targetTile.y)
                return RetracePath(startNode, currentNode, map);

            closed[currentNode.X, currentNode.Y] = true;

            foreach (var (nx, ny) in GetNeighborCoords(currentNode, width, height))
            {
                if (map[nx][ny].type == TileType.Empty || closed[nx, ny]) continue;

                int newG = currentNode.G + 1;
                Node neighbor = nodes[nx, ny];

                // If we haven't seen this node OR found a faster way to it
                if (neighbor == null || newG < neighbor.G)
                {
                    if (neighbor == null)
                    {
                        neighbor = new Node(nx, ny);
                        nodes[nx, ny] = neighbor;
                    }

                    neighbor.G = newG;
                    neighbor.H = GetDistance(nx, ny, targetTile.x, targetTile.y);
                    neighbor.Parent = currentNode;

                    // We just re-enqueue. The PriorityQueue handles the duplicates 
                    // because the one with the lower F-cost will be Dequeued first.
                    openSetPriorityQueue.Enqueue(neighbor, neighbor.F);
                }
            }
        }
        return null;
    
    }

    private static IEnumerable<(int, int)> GetNeighborCoords(Node n, int w, int h)
    {
        if (n.X > 0) yield return (n.X - 1, n.Y);
        if (n.X < w - 1) yield return (n.X + 1, n.Y);
        if (n.Y > 0) yield return (n.X, n.Y - 1);
        if (n.Y < h - 1) yield return (n.X, n.Y + 1);
    }
    
    private static int GetDistance(Node a, Node b)
    {
        // Manhattan distance: simple and fast for grid movement
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
    
    private static int GetDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    private static List<Node> GetNeighbors(Node node, List<List<Tile>> map)
    {
        List<Node> neighbors = new List<Node>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.X + dx[i];
            int ny = node.Y + dy[i];

            // Only add if within bounds and walkable
            if (nx >= 0 && nx < map.Count && ny >= 0 && ny < map[0].Count)
            {
                if (map[nx][ny].type != TileType.Empty) // Adjust to your wall logic
                {
                    neighbors.Add(new Node(nx, ny));
                }
            }
        }
        return neighbors;
    }

    private static List<Tile> RetracePath(Node start, Node end, List<List<Tile>> map)
    {
        List<Tile> path = new List<Tile>();
        Node curr = end;
        while (curr != start)
        {
            path.Add(map[curr.X][curr.Y]);
            curr = curr.Parent;
        }
        path.Reverse();
        return path;
    }
}