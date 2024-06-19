using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace D15
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line = File.ReadAllText(@"..\..\..\input.txt");
            IntCode prog = new IntCode(line);
            Map map = new Map(prog);
            (int part1, int part2) = map.Result();
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(part1);
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(part2);
        }
    }
    public class Map
    {
        static readonly List<int[]> dirs = new List<int[]>() { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } };
        int[,] map = new int[1000,1000];
        int starti = 500;
        int startj = 500;
        int playeri;
        int playerj;
        IntCode player;
        public Map(IntCode brain)
        {
            playeri = starti;
            playerj = startj;
            player = brain;
        }
        void MapMap()
        {
            Stack<int> dirstack = new Stack<int>();
            for(int i = 1; i <= 4; i++)
            {
                dirstack.Push(i);
                player.inputs.Enqueue(i);
                player.Run();
                int result = (int)player.outputs.First();
                player.outputs.Clear();
                if(result > 0)
                {
                    map[playeri,playerj] = result;
                    playeri += dirs[i - 1][0];
                    playerj += dirs[i - 1][1];
                    KeepGoing(dirstack);

                    int inversedir = -1;
                    if (i == 1)
                        inversedir = 2;
                    else if (i == 2)
                        inversedir = 1;
                    else if (i == 3)
                        inversedir = 4;
                    else if (i == 4)
                        inversedir = 3;
                    player.inputs.Enqueue(inversedir);
                    player.Run();
                    player.outputs.Clear();
                    playeri -= dirs[i - 1][0];
                    playerj -= dirs[i - 1][1];
                }               
                dirstack.Pop();
            }
        }
        void KeepGoing(Stack<int> dirstack)
        {
            int top = dirstack.Peek();
            for (int i = 1; i <= 4; i++)
            {
                if (top == 1 && i == 2)
                    continue;
                if (top == 2 && i == 1)
                    continue;
                if (top == 3 && i == 4)
                    continue;
                if (top == 4 && i == 3)
                    continue;

                dirstack.Push(i);
                player.inputs.Enqueue(i);
                player.Run();
                int result = (int)player.outputs.First();
                player.outputs.Clear();
                if (result > 0)
                {
                    map[playeri, playerj] = result;
                    playeri += dirs[i - 1][0];
                    playerj += dirs[i - 1][1];

                    KeepGoing(dirstack);

                    int inversedir = -1;
                    if (i == 1)
                        inversedir = 2;
                    else if (i == 2)
                        inversedir = 1;
                    else if (i == 3)
                        inversedir = 4;
                    else if (i == 4)
                        inversedir = 3;

                    player.inputs.Enqueue(inversedir);
                    player.Run();
                    player.outputs.Clear();

                    playeri -= dirs[i - 1][0];
                    playerj -= dirs[i - 1][1];
                }
                dirstack.Pop();
            }
        }
        public (int part1,int part2) Result()
        {
            MapMap();

            int targeti = -1;
            int targetj = -1;
            for(int i = 0; i < 1000; i++)
            {
                for(int j = 0; j < 1000; j++)
                {
                    if (map[i,j] == 2)
                    {
                        targeti = i;
                        targetj = j;
                        break;
                    }
                }
                if (targeti > 0)
                    break;
            }

            return BFS(targeti, targetj);
        }
        (int,int) BFS(int targeti, int targetj)
        {
            int[,] mark = new int[1000, 1000];
            Queue<(int, int)> q = new Queue<(int, int)>();
            mark[starti, startj] = 1;
            q.Enqueue((starti, startj));
            while (q.Any())
            {
                (int deqi, int deqj) = q.Dequeue();
                foreach (int[] dir in dirs)
                {
                    int nexti = deqi + dir[0];
                    int nextj = deqj + dir[1];
                    if (map[nexti,nextj] != 0 && mark[nexti,nextj] == 0)
                    {
                        mark[nexti, nextj] = mark[deqi, deqj] + 1;
                        q.Enqueue((nexti, nextj));
                    }
                }
            }

            int part1 = mark[targeti, targetj];

            q.Clear();
            mark = new int[1000, 1000];
            mark[targeti, targetj] = 1;
            int maxmark = 0;
            q.Enqueue((targeti, targetj));
            while (q.Any())
            {
                (int deqi, int deqj) = q.Dequeue();
                foreach (int[] dir in dirs)
                {
                    int nexti = deqi + dir[0];
                    int nextj = deqj + dir[1];
                    if (map[nexti, nextj] != 0 && mark[nexti, nextj] == 0)
                    {
                        mark[nexti, nextj] = mark[deqi, deqj] + 1;
                        q.Enqueue((nexti, nextj));
                    }
                }
            }

            for (int i = 0; i < 1000; i++)
                for (int j = 0; j < 1000; j++)
                    if (mark[i, j] > maxmark)
                        maxmark = mark[i, j];

            return (part1, maxmark + 1);
        }
        public void Show()
        {
            int mini = 1000;
            int maxi = 0;
            int minj = 1000;
            int maxj = 0;
            for (int ii = 0; ii < 1000; ii++)
            {
                for (int jj = 0; jj < 1000; jj++)
                {
                    if (map[ii, jj] == 1)
                    {
                        if (ii < mini)
                            mini = ii;
                        if (ii > maxi)
                            maxi = ii;
                        if (jj < minj)
                            minj = jj;
                        if (jj > maxj)
                            maxj = jj;
                    }
                }
            }

            for (int ii = mini - 2; ii <= maxi + 2; ii++)
            {
                for (int jj = minj - 2; jj <= maxj + 2; jj++)
                {
                    Console.Write(map[ii, jj] == 0 ? '#' : '.');
                }
                Console.WriteLine();
            }
        }

    }
    public class IntCode
    {
        public long[] program;
        long driver = 0;
        long relativebase = 0;
        int memsize = 3000; // please adjust this 
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