using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace D18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Part1();
            Part2();
        }
        static void Part2()
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input2.txt");
            Map map = new Map(lines.Length, lines[0].Length, lines);

            NodeMap nodes = map.GetNodeMap(out List<Node> starts);
            
            List<Node> captured = new List<Node>();

            Dictionary<(int, int), int> states = new Dictionary<(int, int), int>();

            int bestdistx = int.MaxValue;
            for(int j = 0; j < starts.Count; j++)
            {
                int bestdist = int.MaxValue;
                Node start = starts[j];
                for (int i = 0; i < starts[j].neighbors.Count; i++)
                {
                    if (start.lockedby[i].Any())
                        continue;
                    starts[j] = starts[j].neighbors[i];
                    captured.Add(start.neighbors[i]);
                    int local = MinDist2(starts, captured, 0, start.DistToNode(start.neighbors[i]), nodes.Nodes, states);
                    captured.Remove(start.neighbors[i]);
                    starts[j] = start;
                    if (local < bestdist)
                        bestdist = local;
                }
                if(bestdist < bestdistx)
                    bestdistx = bestdist;
            }
            
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(bestdistx);
        }
        static int MinDist2(List<Node> currents, List<Node> captured, int currenttotal, int lastdist, List<Node> total, Dictionary<(int, int), int> states)
        {
            int capturedbits = 0;
            foreach (Node node in captured)
                capturedbits += (int)Math.Pow(2, total.IndexOf(node));
            int currentbit = 0;
            foreach(Node node in currents)
                currentbit += (int)Math.Pow(2, total.IndexOf(node));
            (int, int) currentstate = (capturedbits, currentbit);

            if (states.ContainsKey(currentstate))
                return lastdist + states[currentstate];

            if (captured.Count == total.Count)
                return lastdist;
            else
            {
                int bestx = int.MaxValue;             
                for (int j = 0; j < currents.Count; j++)
                {
                    int best = int.MaxValue;
                    Node current = currents[j];                    
                    for (int i = 0; i < current.neighbors.Count; i++)
                    {
                        if (captured.Contains(current.neighbors[i]) || current.lockedby[i].Except(captured).Any())
                            continue;
                        currents[j] = current.neighbors[i];
                        captured.Add(current.neighbors[i]);
                        int localbest = MinDist2(currents, captured, currenttotal + current.DistToNode(current.neighbors[i]), current.DistToNode(current.neighbors[i]), total, states);
                        captured.Remove(current.neighbors[i]);
                        currents[j] = current;
                        if (localbest < best)
                            best = localbest;
                    }
                    if(best < bestx)
                        bestx = best;
                }
                states.Add(currentstate, bestx);
                return lastdist + states[currentstate];
            }
        }
        static void Part1()
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input1.txt");
            Map map = new Map(lines.Length, lines[0].Length, lines);

            NodeMap nodes = map.GetNodeMap(out List<Node> start);
            int bestdist = int.MaxValue;
            List<Node> captured = new List<Node>();

            Dictionary<(int, int), int> states = new Dictionary<(int, int), int>();


            for (int i = 0; i < start[0].neighbors.Count; i++)
            {
                if (start[0].lockedby[i].Any())
                    continue;
                captured.Add(start[0].neighbors[i]);
                int local = MinDist(start[0].neighbors[i], captured, 0, start[0].DistToNode(start[0].neighbors[i]), nodes.Nodes, states);
                captured.Remove(start[0].neighbors[i]);
                if (local < bestdist)
                    bestdist = local;
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(bestdist);
        }
        static int MinDist(Node current, List<Node> captured, int currenttotal, int lastdist, List<Node> total, Dictionary<(int,int),int> states)
        {
            int capturedbits = 0;
            foreach(Node node in captured)
                capturedbits += (int)Math.Pow(2, total.IndexOf(node));
            int currentbit = (int)Math.Pow(2, total.IndexOf(current));
            (int,int) currentstate = (capturedbits, currentbit);

            if (states.ContainsKey(currentstate))
                return lastdist + states[currentstate];               
            
            if(captured.Count == total.Count)
                return lastdist;
            else
            {
                int best = int.MaxValue;
                for (int i = 0; i < current.neighbors.Count; i++)
                {
                    if (captured.Contains(current.neighbors[i]) || current.lockedby[i].Except(captured).Any())
                        continue;
                    captured.Add(current.neighbors[i]);
                    int localbest = MinDist(current.neighbors[i], captured, currenttotal + current.DistToNode(current.neighbors[i]), current.DistToNode(current.neighbors[i]), total, states);
                    captured.Remove(current.neighbors[i]);
                    if(localbest < best)
                        best = localbest;
                }
                states.Add(currentstate, best);
                return lastdist + states[currentstate];
            }
        }
    }
    public class NodeMap
    {
        public List<Node> Nodes = new List<Node>();
    }
    public class Node
    {
        public List<Node> neighbors = new List<Node>();
        public List<int> dists = new List<int>();
        public List<List<Node>> lockedby = new List<List<Node>>();
        public char Name;
        public Node(char c)
        {
            Name = c;
        }
        public int DistToNode(Node node)
        {
            return dists[neighbors.IndexOf(node)];
        }
        public void SortNode()
        {
            for(int i = 0; i < neighbors.Count; i++)
            {
                for (int j = i + 1; j < neighbors.Count; j++)
                {
                    if (dists[i] > dists[j])
                    {
                        (dists[i], dists[j]) = (dists[j], dists[i]);
                        (neighbors[i], neighbors[j]) = (neighbors[j], neighbors[i]);
                        (lockedby[i], lockedby[j]) = (lockedby[j], lockedby[i]);
                    }
                }
            }
        }
        public override string ToString()
        {
            return Name.ToString();
        }
    }
    public class Map
    {
        public static readonly List<int[]> dirs = new List<int[]>() { new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, };
        public int n { get { return map.GetLength(0); } }
        public int m { get { return map.GetLength(1); } }
        public int[,] map;
        public List<Player> players = new List<Player>();
        public List<Key> Keys = new List<Key>();
        public List<Door> Doors = new List<Door>();
        public Map(int n, int m, string[] lines)
        {
            map = new int[n, m];
            for(int i = 0; i < n; i++)
                for(int j = 0; j < m; j++)
                {
                    if (lines[i][j] == '#')
                        map[i, j] = 1;
                    else if (lines[i][j] == '@')
                        players.Add(new Player(i, j));
                    else if (lines[i][j] >= 'a' && lines[i][j] <= 'z')
                        Keys.Add(new Key(lines[i][j], i, j));
                    else if (lines[i][j] >= 'A' && lines[i][j] <= 'Z')
                        Doors.Add(new Door(lines[i][j].ToString().ToLower()[0], i, j));
                }
            foreach(Door d in Doors)
                map[d.I, d.J] = 2;
        }
        public NodeMap GetNodeMap(out List<Node> starts)
        {
            starts = new List<Node>();
            NodeMap main = new NodeMap();
            foreach(Key key in Keys)
                main.Nodes.Add(new Node(key.Char));

            foreach (Key key in Keys)
            {
                Node keynode = main.Nodes.First(x => x.Name == key.Char);
                foreach (Key other in Keys.Where(x => x != key))
                {
                    Node othernode = main.Nodes.First(x => x.Name == other.Char);
                    keynode.neighbors.Add(othernode);
                    int distnode = DistTo(key.I, key.J, other, main.Nodes, out List<Node> otherblocked);
                    keynode.dists.Add(distnode);
                    keynode.lockedby.Add(otherblocked);
                }
            }

            foreach (Player p in players)
            {
                Node start = new Node('@');
                foreach (Key key in Keys)
                {
                    Node keynode = main.Nodes.First(x => x.Name == key.Char);
                    start.neighbors.Add(keynode);
                    int dist = DistTo(p.I, p.J, key, main.Nodes, out List<Node> blockedby);
                    start.dists.Add(dist);
                    start.lockedby.Add(blockedby);
                }
                starts.Add(start);
            }           
            return main;
        }
        public int DistTo(int starti, int startj, Key k, List<Node> nodes, out List<Node> blockedby)
        {
            int[,] marked = new int[n, m];
            blockedby = new List<Node>();

            marked[starti, startj] = 1;
            int toreturn = -1;
            Queue<(int, int)> q = new Queue<(int, int)>();
            q.Enqueue((starti, startj));
            while (q.Any())
            {
                (int deqi, int deqj) = q.Dequeue();
                if (k.I == deqi && k.J == deqj)
                {
                    toreturn = marked[k.I, k.J];
                    break;
                }  
                foreach (int[] dir in dirs)
                {
                    int nexti = deqi + dir[0];
                    int nextj = deqj + dir[1];
                    if (nexti < 0 || nextj < 0 || nexti >= n || nextj >= m || marked[nexti, nextj] != 0 || map[nexti, nextj] == 1)
                        continue;
                    marked[nexti, nextj] = marked[deqi, deqj] + 1;
                    q.Enqueue((nexti, nextj));
                }
            }

            
            int backi = k.I;
            int backj = k.J;
            int currentmark = toreturn;
            if (currentmark == -1)
                blockedby.Add(new Node('&'));
            else
            {
                while (currentmark != 1)
                {
                    foreach (Key key in Keys)
                        if (backi == key.I && backj == key.J && key != k)
                            if (!blockedby.Contains(nodes.First(x => x.Name == key.Char)))
                                blockedby.Add(nodes.First(x => x.Name == key.Char));
                    if (map[backi, backj] == 2)
                        foreach (Door d in Doors)
                            if (d.I == backi && d.J == backj)
                                if (!blockedby.Contains(nodes.First(x => x.Name == d.Char)))
                                    blockedby.Add(nodes.First(x => x.Name == d.Char));
                    foreach (int[] dir in dirs)
                    {
                        int nexti = backi + dir[0];
                        int nextj = backj + dir[1];
                        if (marked[nexti, nextj] == currentmark - 1)
                        {
                            currentmark -= 1;
                            backi = nexti;
                            backj = nextj;
                            break;
                        }
                    }
                }
            }          

            return toreturn - 1;
        }
    }
    public class Door
    {
        public char Char;
        public int I;
        public int J;
        public bool collected = false;
        public Door(char c, int i, int j)
        {
            Char = c;
            I = i;
            J = j;
        }
    }
    public class Key
    {
        public char Char;
        public int I;
        public int J;
        public bool collected = false;
        public Key(char c, int i, int j) 
        {
            Char = c; 
            I = i;
            J = j;
        } 
    }
    public class Player
    {
        public int I;
        public int J;
        public Player(int i, int j)
        {
            this.I = i;
            this.J = j;
        }
    }
}