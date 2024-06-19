using System.Text;

namespace D24
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input.txt");
            BugMap bugmap = new BugMap(lines);
            while (true)
            {               
                int seen = bugmap.states.Count;
                bugmap.Sim();
                if (bugmap.states.Count == seen)
                    break;              
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(bugmap.GetState());

            BugMap recursive = new BugMap(lines);
            for(int t = 0; t < 200; t++)
                recursive.Sim2();
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(recursive.Score());
        }
    }
    public class BugMap
    {
        static List<int[]> dirs = new List<int[]>() { new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 1, 0 }, new int[] { -1, 0 } }; //, new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, 1 }, new int[] { -1, -1 }, };
        public HashSet<int> states = new HashSet<int>();      
        Dictionary<int,int[,]> dims = new Dictionary<int, int[,]>();
        public BugMap(string[] lines) 
        {
            dims.Add(0, new int[5, 5]);
            for(int i = 0; i < lines.Length; i++)
                for(int j = 0; j < lines[i].Length; j++)
                    if (lines[i][j] == '#')
                        dims[0][i, j] = 1;
           states.Add(GetState());
        }
        public int Score()
        {
            int count = 0;
            foreach (int[,] map in dims.Values)
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == 2 && j == 2)
                            continue;
                        count += map[i, j];
                    }                      
            return count;
        }
        public void Sim2()
        {
            Dictionary<int, int[,]> newdict = new Dictionary<int, int[,]>();
            int minkey = dims.Keys.Min() - 1;
            int maxkey = dims.Keys.Max() + 1;
            dims.Add(minkey, new int[5,5]);
            dims.Add(maxkey, new int[5,5]);

            for(int d = minkey; d <= maxkey; d++)
            {
                int[,] newmap = new int[5, 5];
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == 2 && j == 2)
                            continue;
                        newmap[i, j] = NextGen2(i, j, dims[d][i, j], d);
                    }
                }
                newdict.Add(d, newmap);
            }

            dims = newdict;

            int minkeycount = 0;
            int maxkeycount = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (i == 2 && j == 2)
                        continue;
                    minkeycount += dims[minkey][i, j];
                    maxkeycount += dims[maxkey][i, j];
                }                    
            }
            if (minkeycount == 0)
                dims.Remove(minkey);
            if (maxkeycount == 0)
                dims.Remove(maxkey);
        }
        int NextGen2(int i, int j, int isalive, int currentdim)
        {
            if (isalive == 1)
            {
                int count = 0;
                foreach (int[] dir in dirs)
                    count += IsBug(i + dir[0], j + dir[1], currentdim, i, j);
                if (count == 1)
                    return 1;
                return 0;
            }
            else
            {
                int count = 0;
                foreach (int[] dir in dirs)
                    count += IsBug(i + dir[0], j + dir[1], currentdim, i, j);
                if (count == 1 || count == 2)
                    return 1;
                return 0;
            }
        }
        public int GetState()
        {
            int toreturn = 0;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (dims[0][i, j] == 1)
                        toreturn += (int)Math.Pow(2, 5 * i + j);
            return toreturn;
        }
        public void Sim()
        {
            int[,] newmap = new int[5, 5];
            for(int i = 0; i < 5; i++)
                for(int j = 0; j < 5; j++)
                    newmap[i, j] = NextGen(i, j, dims[0][i,j]);
            dims[0] = newmap;
            states.Add(GetState());
        }
        int NextGen(int i, int j, int isalive)
        {
            if(isalive == 1)
            {
                int count = 0;
                foreach (int[] dir in dirs)
                    count += IsBug(i + dir[0], j + dir[1], 0, 0, 0);
                if (count == 1)
                    return 1;
                return 0;
            }
            else
            {
                int count = 0;
                foreach (int[] dir in dirs)
                    count += IsBug(i + dir[0], j + dir[1], 0, 0, 0);
                if (count == 1 || count == 2)
                    return 1;
                return 0;
            }
        }
        int IsBug(int i, int j, int dim, int senti, int sentj)
        {
            if (i < 0)
            {
                if (dims.ContainsKey(dim - 1))
                    return dims[dim - 1][1, 2];
                else
                    return 0;
            }
            else if (i >= 5)
            {
                if (dims.ContainsKey(dim - 1))
                    return dims[dim - 1][3, 2];
                else
                    return 0;
            }
            else if (j < 0)
            {
                if (dims.ContainsKey(dim - 1))
                    return dims[dim - 1][2, 1];
                else
                    return 0;
            }
            else if (j >= 5)
            {
                if (dims.ContainsKey(dim - 1))
                    return dims[dim - 1][2, 3];
                else
                    return 0;
            }

            if(i == 2 && j == 2)
            {
                if (senti == 2 && sentj == 1)
                {
                    int toreturn = 0;
                    if (dims.ContainsKey(dim + 1))
                        for (int k = 0; k < 5; k++)
                            toreturn += dims[dim + 1][k, 0];
                    return toreturn;
                }
                else if (senti == 2 && sentj == 3)
                {
                    int toreturn = 0;
                    if (dims.ContainsKey(dim + 1))
                        for (int k = 0; k < 5; k++)
                            toreturn += dims[dim + 1][k, 4];
                    return toreturn;
                }
                else if (senti == 1 && sentj == 2)
                {
                    int toreturn = 0;
                    if (dims.ContainsKey(dim + 1))
                        for (int k = 0; k < 5; k++)
                            toreturn += dims[dim + 1][0, k];
                    return toreturn;
                }
                else if (senti == 3 && sentj == 2)
                {
                    int toreturn = 0;
                    if (dims.ContainsKey(dim + 1))
                        for (int k = 0; k < 5; k++)
                            toreturn += dims[dim + 1][4, k];
                    return toreturn;
                }
            }
            

            return dims[dim][i, j];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int k in dims.Keys)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == 2 && j == 2)
                            sb.Append("?");
                        else
                            sb.Append(dims[k][i, j]);
                    }
                    sb.Append(Environment.NewLine);
                }
                sb.Append(k);
                sb.Append(Environment.NewLine);
            }
           
            return sb.ToString();
                    
        }
    }
}