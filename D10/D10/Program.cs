using System.Drawing;

namespace D10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Map asteroids = new Map(@"..\..\..\TextFile1.txt");
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(asteroids.Part1());
            Console.WriteLine("Part 2 solution:");
            asteroids.Part2();
        }
    }
    public class Map
    {
        static List<int[]> masks = new List<int[]>() { new int[] { -1, 1 }, new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, -1 } };
        static List<int[]> dirs8 = new List<int[]>() { new int[] { -1, 0 }, new int[] { -1, 1 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 1, 0 }, new int[] { 1, -1 }, new int[] { 0, -1 }, new int[] { -1, -1 } };
        int[,] map;
        int n { get { return map.GetLength(0); } }
        int m { get { return map.GetLength(1); } }
        int BestI;
        int BestJ;
        public Map(string file)
        {
            string[] lines = File.ReadAllLines(file);
            map = new int[lines.Length,lines[0].Length];
            for (int i = 0; i < lines.Length; i++)
                for(int j = 0; j < lines[i].Length; j++)
                    if (lines[i][j] == '#')
                        map[i, j] = 1;
        }
        public int Part1()
        {
            int bcount = 0;

            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if (map[i, j] == 0)
                        continue;
                    int seen = GetCount(i, j, map, out _);
                    if (seen > bcount)
                    {
                        bcount = seen;
                        BestI = i;
                        BestJ = j;
                    }                        
                }
            }
            
            return bcount;
        }
        public void Part2()
        {
            int asteroidskilled = 0;
            GetCount(BestI, BestJ, map, out int[,] visible);
            while(asteroidskilled < 200)
            {
                List<Point> closest = new List<Point>();
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        if (i == BestI && j == BestJ)
                            continue;
                        if (visible[i, j] == 1)
                            closest.Add(new Point(i, j));
                    }


                while (closest.Any())
                {
                    double bestangle = 361;
                    int idx = -1;
                    for (int i = 0; i < closest.Count; i++)
                    {
                        double angle = Anglebetween(new Point(0, BestJ), new Point(BestI, BestJ), closest[i]) * 180d / Math.PI;
                        if (closest[i].Y < BestJ)
                            angle = 360 - angle;
                        if (angle < bestangle)
                        {
                            bestangle = angle;
                            idx = i;
                        }
                    }
                    asteroidskilled++;
                    map[closest[idx].X, closest[idx].Y] = 0;
                    if(asteroidskilled == 200)
                        Console.WriteLine(closest[idx].Y * 100 + closest[idx].X);
                    closest.Remove(closest[idx]);
                }

                GetCount(BestI, BestJ, map, out visible);
            }
        }
        double Anglebetween(Point A, Point B, Point C)
        {
            // cos(B);
            double c = Dist(A, B);
            double a = Dist(B, C);
            double b = Dist(C, A);

            double cosb = (a*a - b*b + c*c) / (2*a*c);
            return Math.Acos(cosb);

        }
        double Dist(Point A, Point B)
        {
            return Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));
        }
        int GetCount(int starti, int startj, int[,] map, out int[,] mapclone)
        {
            int count = 0;
            mapclone = new int[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    mapclone[i, j] = map[i, j];
            mapclone[starti, startj] = 0;

            for (int i = 0; i < n; i++)
            {
                for(int j = 0; j < m; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    foreach (int[] mask in masks)
                    {
                        int diri = i * mask[0];
                        int dirj = j * mask[1];
                        int times = 1;
                        bool hit = false;
                        while (starti + diri * times < n && startj + dirj * times < m && starti + diri * times >= 0 && startj + dirj * times >= 0)
                        {
                            if (hit)
                                mapclone[starti + diri * times, startj + dirj * times] = 0;
                            else if (mapclone[starti + diri * times, startj + dirj * times] == 1)
                                hit = true;
                            times++;
                        }
                    }
                    
                }
            }
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    if (mapclone[i, j] == 1)
                        count++;
            return count;
        }
    }
}