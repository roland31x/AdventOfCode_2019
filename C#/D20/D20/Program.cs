using System.Drawing;

namespace D20
{
    internal class Program
    {
        static List<int[]> dirs = new List<int[]>() { new int[] { 1, 0 } , new int[] { 0, 1 } , new int[] { -1, 0 } , new int[] { 0, -1 } };
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input.txt");
            Node[,] map = new Node[lines.Length, lines[0].Length];
            MapBaseNodes(map, lines);
            MapPortalsToMap(map, lines, out Dictionary<string,Portal> portals);

            Portal start = portals["AA"];
            Portal target = portals["ZZ"];

            int starti = start.Outer.Value.X;
            int startj = start.Outer.Value.Y;

            int endi = target.Outer.Value.X;
            int endj = target.Outer.Value.Y;
            Node end = map[endi, endj];

            Queue<Node> q = new Queue<Node>();
            map[starti, startj].Value = 1;
            q.Enqueue(map[starti,startj]);
            while (true)
            {
                Node current = q.Dequeue();
                if (current == end)
                    break;
                foreach(Node neighbor in current.neighbors)
                {
                    if (neighbor.Value != 0)
                        continue;
                    neighbor.Value = current.Value + 1;
                    q.Enqueue(neighbor);
                }
                if(current.portal != null && current.portal.Value == 0)
                {
                    current.portal.Value = current.Value + 1;
                    q.Enqueue(current.portal);
                }
            }

            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(end.Value - 1);


            Queue<(Node,int)> q2 = new Queue<(Node,int)>();
            int[,,] marked = new int[map.GetLength(0),map.GetLength(1),1000];
            marked[starti, startj, 0] = 1;
            q2.Enqueue((map[starti, startj],0));
            while (q2.Any())
            {
                (Node current, int dim) = q2.Dequeue();
                if (current == end && dim == 0)
                    break;
                foreach (Node neighbor in current.neighbors)
                {
                    if (marked[neighbor.I, neighbor.J, dim] != 0)
                        continue;
                    marked[neighbor.I, neighbor.J, dim] = marked[current.I, current.J, dim] + 1;
                    q2.Enqueue((neighbor, dim));
                }
                if (current.portal != null)
                {
                    int whichdim = 1;
                    if (current.I == 2 || current.I == map.GetLength(0) - 3 || current.J == 2 || current.J == map.GetLength(1) - 3)
                        whichdim = -1;
                    if ((dim == 0 && whichdim == -1))
                        continue;
                    else if (marked[current.portal.I, current.portal.J, dim + whichdim] == 0)
                    {
                        marked[current.portal.I, current.portal.J, dim + whichdim] = marked[current.I, current.J, dim] + 1;
                        q2.Enqueue((current.portal, dim + whichdim));
                    }
                }
            }
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(marked[endi, endj, 0] - 1);
        }
        static void MapBaseNodes(Node[,] map, string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if (lines[i][j] != '.')
                        continue;
                    Node current = new Node(i, j);
                    map[i, j] = current;
                    SetNeighbors(current, i, j, map);
                }
            }
        }
        static void MapPortalsToMap(Node[,] map, string[] lines, out Dictionary<string,Portal> portals)
        {
            portals = new Dictionary<string, Portal>();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                for (int j = 0; j < lines[i].Length - 1; j++)
                {
                    if (lines[i][j] < 'A' || lines[i][j] > 'Z')
                        continue;
                    string portalname = GetName(i, j, lines);
                    if (!portals.ContainsKey(portalname))
                        portals.Add(portalname, new Portal(portalname));
                    Portal current = portals[portalname];

                    int[,] marked = new int[lines.Length, lines[i].Length];
                    Queue<(int, int)> q = new Queue<(int, int)>();
                    marked[i, j] = 1;
                    q.Enqueue((i, j));
                    while (true)
                    {
                        (int deqi, int deqj) = q.Dequeue();
                        if (map[deqi, deqj] != null)
                        {
                            Point toCheck = new Point(deqi, deqj);
                            if (current.Outer == null && current.Inner != toCheck)
                                current.Outer = toCheck;
                            else if (current.Outer != toCheck && current.Inner == null)
                                current.Inner = toCheck;
                            break;
                        }
                        foreach (int[] dir in dirs)
                        {
                            int nexti = dir[0] + deqi;
                            int nextj = dir[1] + deqj;
                            if (nexti < 0 || nexti >= lines.Length || nextj < 0 || nextj >= lines[i].Length || lines[i][j] == ' ' || marked[nexti, nextj] != 0)
                                continue;
                            marked[nexti, nextj] = 1;
                            q.Enqueue((nexti, nextj));
                        }
                    }

                }
            }

            foreach (Portal p in portals.Values.Where(x => x.Outer != null && x.Inner != null))
            {
                Point p1 = (Point)p.Outer!;
                Point p2 = (Point)p.Inner!;
                map[p1.X, p1.Y].portal = map[p2.X, p2.Y];
                map[p2.X, p2.Y].portal = map[p1.X, p1.Y];
            }
        }
        static string GetName(int i, int j, string[] lines)
        {
            char foundchar = lines[i][j];
            if (i - 1 >= 0 && (lines[i - 1][j] >= 'A' && lines[i - 1][j] <= 'Z'))
                return lines[i - 1][j].ToString() + foundchar;
            else if (j - 1 >= 0 && (lines[i][j - 1] >= 'A' && lines[i][j - 1] <= 'Z'))
                return lines[i][j - 1].ToString() + foundchar;
            else if (j + 1 >= 0 && (lines[i][j + 1] >= 'A' && lines[i][j + 1] <= 'Z'))
                return foundchar + lines[i][j + 1].ToString();
            else
                return foundchar + lines[i + 1][j].ToString();
        }
        static void SetNeighbors(Node current, int i, int j, Node[,] map)
        {
            TrySetNeighbor(current, i - 1, j, map);
            TrySetNeighbor(current, i + 1, j, map);
            TrySetNeighbor(current, i, j + 1, map);
            TrySetNeighbor(current, i, j - 1, map);
        }
        static void TrySetNeighbor(Node current, int i, int j, Node[,] map)
        {
            if (i < 0 || i >= map.GetLength(0) || j < 0 || j >= map.GetLength(1) || map[i, j] == null)
                return;

            current.neighbors.Add(map[i,j]);
            map[i, j].neighbors.Add(current);
        }
    }
    public class Portal
    {
        public string name;
        public Point? Outer;
        public Point? Inner;
        public Portal(string name)
        {
            this.name = name;
        }
    }
    public class Node
    {
        public int I;
        public int J;
        public int Value = 0;
        public HashSet<Node> neighbors = new HashSet<Node>();
        public Node? portal;
        public Node(int i, int j) 
        {
            I = i;
            J = j;
        }
    }
}