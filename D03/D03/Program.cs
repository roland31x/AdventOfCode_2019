using System.Security.AccessControl;

namespace D03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input.txt");
            CableMap map = new CableMap(lines);
            map.Solve();
            
        }
    }
    public class CableMap
    {
        public static List<int[]> directions = new List<int[]>() { new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 } };
        static int size = 25001;
        int[,] map = new int[size, size];
        int startx = size / 2;
        int starty = size / 2;
        Cable c1;
        Cable c2;
        public CableMap(string[] lines) 
        {
            c1 = new Cable(lines[0]);
            c2 = new Cable(lines[1]);
        }
        public void Solve()
        {
            int cablex = startx;
            int cabley = starty;
            for(int i = 0; i < c1.dirs.GetLength(0); i++)
            {
                for(int times = 0; times < c1.dirs[i,0]; times++)
                {
                    cablex += directions[c1.dirs[i, 1]][0];
                    cabley += directions[c1.dirs[i, 1]][1];
                    map[cablex, cabley] = 1;
                }
            }
            cablex = startx;
            cabley = starty;
            int best = int.MaxValue;
            for (int i = 0; i < c2.dirs.GetLength(0); i++)
            {
                for (int times = 0; times < c2.dirs[i, 0]; times++)
                {
                    cablex += directions[c2.dirs[i, 1]][0];
                    cabley += directions[c2.dirs[i, 1]][1];
                    if (map[cablex,cabley] > 0)
                    {
                        map[cablex, cabley] = 2;
                        int dist = Math.Abs(cablex - startx) + Math.Abs(cabley - starty);
                        if (dist < best)
                        {
                            best = dist;
                        }
                    }
                }
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(best);
            List<int[]> intersectionpairs = new List<int[]>();
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if (map[i, j] != 2)
                        map[i, j] = 0;
                    else
                    {
                        intersectionpairs.Add(new int[] { i, j });
                    }
                }
            }
            best = int.MaxValue;
            foreach (int[] pair in intersectionpairs)
            {
                int c1x = startx;
                int c1y = starty;
                int c1t = 0;
                bool c1ok = false;
                for (int i = 0; i < c1.dirs.GetLength(0); i++)
                {
                    if (c1ok)
                        break;
                    for (int times = 0; times < c1.dirs[i, 0]; times++, c1t++)
                    {
                        if (c1ok)
                            break;
                        c1x += directions[c1.dirs[i, 1]][0];
                        c1y += directions[c1.dirs[i, 1]][1];
                        if (c1x == pair[0] && c1y == pair[1])
                            c1ok = true;
                    }
                }
                int c2x = startx;
                int c2y = starty;
                int c2t = 0;
                bool c2ok = false;
                for (int i = 0; i < c2.dirs.GetLength(0); i++)
                {
                    if (c2ok)
                        break;
                    for (int times = 0; times < c2.dirs[i, 0]; times++, c2t++)
                    {
                        if (c2ok)
                            break;
                        c2x += directions[c2.dirs[i, 1]][0];
                        c2y += directions[c2.dirs[i, 1]][1];
                        if (c2x == pair[0] && c2y == pair[1])
                            c2ok = true;
                    }
                }
                int sum = c2t + c1t;
                if (sum < best)
                    best = sum;
            }
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(best);
        }
    }
    public class Cable
    {

        public int[,] dirs;
        public Cable(string coms)
        {
            string[] tokens = coms.Split(',');
            dirs = new int[tokens.Length, 2];
            for(int i = 0; i < tokens.Length; i++)
            {
                string command = tokens[i];
                switch (command[0])
                {
                    case 'U':
                        dirs[i, 1] = 0;
                        break;
                    case 'R':
                        dirs[i, 1] = 1;
                        break;
                    case 'D':
                        dirs[i, 1] = 2;
                        break;
                    case 'L':
                        dirs[i, 1] = 3;
                        break;
                }
                command = command.Replace(command[0].ToString(), "");
                dirs[i, 0] = int.Parse(command);
            }
        }
    }
}