using System.Drawing;
using System.Reflection;

namespace D11
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            
            PaintingRobot robot1 = new PaintingRobot();
            robot1.Paint();
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(robot1.points.Count);
            PaintingRobot robot2 = new PaintingRobot();
            robot2.map[robot2.i, robot2.j] = 1;
            robot2.Paint();
            Console.WriteLine("Part 2 solution");
            robot2.Show();

        }
    }
    public class PaintingRobot
    {
        public readonly static List<int[]> dirs = new List<int[]>() { new int[] { -1, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 } };

        public int[,] map = new int[3000, 3000];
        public int i = 1500;
        public int j = 1500;
        public int dir = 0;
        bool finished = false;
        IntCode program;
        public HashSet<Point> points = new HashSet<Point>();
        public PaintingRobot()
        {
            string lines = File.ReadAllText(@"..\..\..\input.txt");
            program = new IntCode(lines);
        }
        public void Show()
        {
            int mini = 3000;
            int maxi = 0;
            int minj = 3000;
            int maxj = 0;
            for(int ii = 0; ii < 3000; ii++)
            {
                for(int jj = 0; jj < 3000; jj++)
                {
                    if (map[ii,jj] == 1)
                    {
                        if (ii < mini)
                            mini = ii;
                        if(ii > maxi)
                            maxi = ii;
                        if(jj < minj)
                            minj = jj;
                        if(jj > maxj)
                            maxj = jj;
                    }
                }
            }

            for (int ii = mini; ii <= maxi; ii++)
            {
                for (int jj = minj; jj <= maxj; jj++)
                {
                    Console.Write(map[ii, jj] == 0 ? '.' : '#');
                }
                Console.WriteLine();
            }
        }
        public void Paint()
        {
            while (!finished)
            {
                program.inputs.Enqueue(map[i, j]);
                finished = program.Run();
                int paint = (int)program.outputs.First();
                int nextdir = (int)program.outputs.Last();
                program.outputs.Clear();
                map[i, j] = paint;
                points.Add(new Point(i, j));
                if (nextdir == 0)
                    dir = (4 + dir - 1) % 4;
                else
                    dir = (4 + dir + 1) % 4;
                i += dirs[dir][0];
                j += dirs[dir][1];
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