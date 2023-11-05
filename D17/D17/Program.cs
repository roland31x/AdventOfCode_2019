using System.Runtime.CompilerServices;
using System.Text;

namespace D17
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line = File.ReadAllText(@"..\..\..\input.txt");
            IntCode prog = new IntCode(line);
            prog.Run();

            List<List<char>> chars = new List<List<char>>();
            int dr = 0;
            int l = 0;
            chars.Add(new List<char>());
            while(dr < prog.outputs.Count)
            {
                char read = (char)prog.outputs[dr];
                if(read == 'M')
                    break;
                if (read == '\n')
                {
                    l++;
                    chars.Add(new List<char>());
                }
                else
                    chars[l].Add(read);
                dr++;
            }

            Map map = new Map(chars);
            int intersections = map.GetIntersectionsScore();
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(intersections);

            string path = map.GetPath();

            string MainFunc = "";

            EvaluatePath(path, out string A, out string B, out string C);
            MainFunc = path.Replace(A, "A,").Replace(B, "B,").Replace(C, "C,");

            List<int> main = MainFunc.ToAscii();
            List<int> Afunc = A.ToAscii();
            List<int> Bfunc = B.ToAscii();
            List<int> Cfunc = C.ToAscii();

            EnqList(prog, main);
            EnqList(prog, Afunc);
            EnqList(prog, Bfunc);
            EnqList(prog, Cfunc);
            int no = 'n';

            prog.inputs.Enqueue(no);
            prog.inputs.Enqueue(10);
            prog.Run();

            //chars = new List<List<char>>();
            //dr = 0;
            //l = 0;
            //chars.Add(new List<char>());
            //while (dr < prog.outputs.Count)
            //{
            //    char read = (char)prog.outputs[dr];
            //    if (read == '\n')
            //    {
            //        l++;
            //        chars.Add(new List<char>());
            //    }
            //    else
            //        chars[l].Add(read);
            //    dr++;
            //}
            //map = new Map(chars);
            //Console.WriteLine(map.ToString());
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(prog.outputs.Last());
        }
        static void EvaluatePath(string path, out string A, out string B, out string C)
        {
            string[] tokens = path.Split(',');
            A = "";
            B = "";
            C = "";
            for(int i = 0; i < tokens.Length / 2; i++)
            {
                if (A != "" && B != "" && C != "")
                    break;
                tokens = path.Split(',');
                for (int j = 20; j >= 4; j -= 2)
                {
                    string pattern = "";
                    bool toolong = false;
                    for(int k = i; k < i + j; k++)
                    {
                        try
                        {
                            if (tokens[k] == "S")
                                throw new InvalidDataException();
                            pattern += tokens[k];
                            
                            pattern += ',';
                        }
                        catch (Exception)
                        {
                            toolong = true;
                            break;
                        }
                        
                    }
                    if (toolong)
                        continue;
                    if (pattern == "A" || pattern == "B" || pattern == "C")
                        continue;
                    int timesok = 0;
                    for(int k = i + j; k + j < tokens.Length; k++)
                    {
                        try
                        {
                            string tocheck = "";
                            for (int l = k; l < k + j; l++)
                            {
                                if (tokens[l] == "S")
                                    throw new Exception();
                                tocheck += tokens[l];
                                tocheck += ',';
                            }
                            if (tocheck == pattern)
                                timesok++;
                        }
                        catch(Exception)
                        {
                            continue;
                        }
                    }

                    if(timesok > 0 && pattern != A && pattern != B && pattern != C)
                    {
                        if (A == "")
                            A = pattern;
                        else if (B == "")
                            B = pattern;
                        else if(C == "")
                            C = pattern;
                        path = path.Replace(pattern, "S,");
                        i = -1;
                        break;
                    }
                }
            }
        }
        static void EnqList(IntCode program, List<int> list)
        {
            foreach(int i in list)
                program.inputs.Enqueue(i);
        }
    }
    public class Map
    {
        public static readonly List<int[]> dirs = new List<int[]>() { new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 } };
        public static readonly List<int> trydir = new List<int>() { -1, 1 };
        List<List<char>> map;
        int cdir = -1;
        public Map(List<List<char>> tocopy)
        {
            map = tocopy.Where(x => x.Count > 0).ToList();
        }
        public string GetPath()
        {
            StringBuilder sb = new StringBuilder();

            int starti = -1;
            int startj = -1;
            for (int i = 0; i < map.Count; i++)
            {
                if (starti != -1)
                    break;
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (map[i][j] == '^' || map[i][j] == '>' || map[i][j] == '<' || map[i][j] == 'v')
                    {
                        starti = i;
                        startj = j;
                        switch (map[i][j])
                        {
                            case '^':
                                cdir = 0;
                                break;
                            case '<':
                                cdir = 1;
                                break;
                            case '>':
                                cdir = 2;
                                break;
                            case 'v':
                                cdir = 3;
                                break;
                        }
                        break;
                    }
                }
            }
            int[,] marked = new int[map.Count, map[0].Count];
            marked[starti, startj] = 1;

            while (true)
            {
                int okmove = 0;
                foreach (int[] dir in dirs)
                {
                    int nexti = starti + dir[0];
                    int nextj = startj + dir[1];
                    try
                    {
                        if (map[nexti][nextj] == '#' && marked[nexti,nextj] == 0)
                            okmove++; 
                    }
                    catch(Exception)
                    {
                        continue;
                    }
                }
                if (okmove == 0)
                    break;
                foreach(int tryd in trydir)
                {
                    bool ok = false;
                    int startdir = cdir;
                    startdir += tryd + 4;
                    startdir %= 4;
                    int nexti = starti + dirs[startdir][0];
                    int nextj = startj + dirs[startdir][1];

                    if (map[nexti][nextj] == '#')
                    {
                        cdir = startdir;
                        ok = true;
                        if(tryd == -1)
                        {
                            sb.Append('L');
                            sb.Append(',');
                        }
                        else if(tryd == 1)
                        {
                            sb.Append('R');
                            sb.Append(',');
                        }
                        int ct = 0;
                        while (nexti < map.Count && nextj < map[0].Count && nexti >= 0 && nextj >= 0 && map[nexti][nextj] == '#')
                        {
                            marked[nexti, nextj] = 1;
                            starti = nexti;
                            startj = nextj;
                            nexti = starti + dirs[cdir][0];
                            nextj = startj + dirs[cdir][1];
                            ct++;
                        }
                        sb.Append(ct);
                        sb.Append(',');
                    }
                    if (ok)
                        break;
                }              
            }

            return sb.ToString();
        }
        public int GetIntersectionsScore()
        {
            int score = 0;
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (map[i][j] != '#')
                        continue;
                    bool ok = true;
                    foreach(int[] dir in dirs)
                    {
                        try
                        {
                            if(map[i + dir[0]][j + dir[1]] != '#')
                            {
                                ok = false;
                                break;
                            }
                        }
                        catch(Exception)
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {
                        score += i * j;
                    }
                }
            }
            return score;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < map.Count; i++)
            {
                for(int j = 0; j < map[i].Count; j++)
                {
                    sb.Append(map[i][j]);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
    public static class StringExt
    {
        public static List<int> ToAscii(this string input)
        {
            List<int> toret = new List<int>();
            foreach(char c in input)
            {
                toret.Add(c);
            }
            if(toret.Last() == ',')
                toret.RemoveAt(toret.Count - 1);
            toret.Add(10);
            return toret;
        }
    }
    public class IntCode
    {
        public long[] program;
        long driver = 0;
        long relativebase = 0;
        int memsize = 5000; // please adjust this 
        public List<long> outputs = new List<long>();
        public Queue<long> inputs = new Queue<long>();
        int definput = 0;
        public IntCode(string ops)
        {
            string[] tokens = ops.Split(',');
            program = new long[memsize];
            for (int i = 0; i < tokens.Length; i++)
            {
                program[i] = long.Parse(tokens[i]);
            }
        }
        public long Output()
        {
            return outputs.Last();
        }
        public bool Run()
        {
            checked
            {
                while (program[driver] != 99)
                {
                    try
                    {
                        Dissect(program[driver]);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public long GetMode(long mode, long pos)
        {
            try
            {
                switch (mode)
                {
                    case 0:
                        return program[pos];
                    case 1:
                        return pos;
                    case 2:
                        return program[pos] + relativebase;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            throw new Exception("unkown mode");
        }
        void Dissect(long opcode)
        {
            long[] op = new long[5];
            int idx = 4;
            while (opcode > 0)
            {
                op[idx] = opcode % 10;
                opcode /= 10;
                idx--;
            }
            long firstparam = GetMode(op[2], driver + 1);
            long secondparam = GetMode(op[1], driver + 2);
            long thirdparam = GetMode(op[0], driver + 3);

            long actualcode = 10 * op[3] + op[4];
            switch (actualcode)
            {
                case 1:
                    Addition(firstparam, secondparam, thirdparam);
                    driver += 4;
                    break;
                case 2:
                    Multiply(firstparam, secondparam, thirdparam);
                    driver += 4;
                    break;
                case 3:
                    Input(firstparam);
                    driver += 2;
                    break;
                case 4:
                    Output(firstparam);
                    driver += 2;
                    break;
                case 5:
                    if (!JumpIfTrue(firstparam, secondparam))
                        driver += 3;
                    break;
                case 6:
                    if (!JumpIfFalse(firstparam, secondparam))
                        driver += 3;
                    break;
                case 7:
                    LessThan(firstparam, secondparam, thirdparam);
                    driver += 4;
                    break;
                case 8:
                    Equality(firstparam, secondparam, thirdparam);
                    driver += 4;
                    break;
                case 9:
                    SetRelativeBase(firstparam);
                    driver += 2;
                    break;
                default:
                    throw new Exception(actualcode + " unknown opcode!");

            }
        }
        void SetRelativeBase(long firstparam)
        {
            relativebase += program[firstparam];
        }
        bool JumpIfTrue(long firstparam, long secondparam)
        {
            if (program[firstparam] != 0)
            {
                driver = program[secondparam];
                return true;
            }
            return false;
        }
        bool JumpIfFalse(long firstparam, long secondparam)
        {
            if (program[firstparam] == 0)
            {
                driver = program[secondparam];
                return true;
            }
            return false;
        }
        void LessThan(long firstparam, long secondparam, long thirdparam)
        {
            if (program[firstparam] < program[secondparam])
            {
                program[thirdparam] = 1;
            }
            else
                program[thirdparam] = 0;
        }
        void Equality(long firstparam, long secondparam, long thirdparam)
        {
            if (program[firstparam] == program[secondparam])
            {
                program[thirdparam] = 1;
            }
            else
                program[thirdparam] = 0;
        }
        void Addition(long firstparam, long secondparam, long thirdparam)
        {
            program[thirdparam] = program[firstparam] + program[secondparam];
        }
        void Multiply(long firstparam, long secondparam, long thirdparam)
        {
            program[thirdparam] = program[firstparam] * program[secondparam];
        }
        void Input(long firstparam)
        {
            program[firstparam] = inputs.Dequeue();
        }
        void Output(long firstparam)
        {
            outputs.Add(program[firstparam]);
        }
        public void OverridePosition(int position, long value)
        {
            program[position] = value;
        }
        public long Position(int i)
        {
            return program[i];
        }
    }
}