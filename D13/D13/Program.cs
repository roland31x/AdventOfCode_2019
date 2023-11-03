namespace D13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string lines = File.ReadAllText(@"..\..\..\input.txt");
            IntCode program = new IntCode(lines);
            program.program[0] = 2;
            bool ok = program.Run();
           
            Map map = new Map();
            
            int idx = 0;
            while(idx < program.outputs.Count)
            {
                int i = (int)program.outputs[idx + 1];
                int j = (int)program.outputs[idx];
                int type = (int)program.outputs[idx + 2];
                idx += 3;
                if (j == -1 && i == 0)
                {
                    map.Score = type;
                    continue;
                }                   
                map.map[i, j] = type;
                
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(map.BlockCount());
            map.Normalize();
            //map.Show();

            while (map.BlockCount() > 0)
            {
                int toenq = 0;
                int ballj = map.BallJ();
                int tilej = map.TileJ();
                if (ballj < tilej)
                    toenq = -1;
                else if (ballj > tilej)
                    toenq = 1;
                program.inputs.Enqueue(toenq);
                program.outputs.Clear();
                program.Run();
                idx = 0;
                while (idx < program.outputs.Count)
                {
                    int i = (int)program.outputs[idx + 1];
                    int j = (int)program.outputs[idx];
                    int type = (int)program.outputs[idx + 2];
                    idx += 3;
                    if (j == -1 && i == 0)
                    {
                        map.Score = type;
                        continue;
                    }
                    map.map[i, j] = type;
                }
                //map.Show();
            }
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(map.Score);
        }
    }
    public class Map
    {
        int size = 1000;
        public int[,] map;
        public int Score;
        public Map()
        {
            map = new int[size, size];
        }
        public void Normalize()
        {
            int maxi = 0;
            int maxj = 0;
            for (int ii = 0; ii < size; ii++)
            {
                for (int jj = 0; jj < size; jj++)
                {
                    if (map[ii, jj] != 0)
                    {
                        if (ii > maxi)
                            maxi = ii;
                        if (jj > maxj)
                            maxj = jj;
                    }
                }
            }
            int[,] norm = new int[maxi + 1, maxj + 1];
            for (int ii = 0; ii <= maxi; ii++)
                for (int jj = 0; jj <= maxj; jj++)
                    norm[ii, jj] = map[ii, jj];

            map = norm;

        }
        public int BlockCount()
        {
            int ct = 0;
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(0); j++)
                    if (map[i, j] == 2)
                        ct++;
            return ct;
        }
        public int TileJ()
        {
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] == 3)
                        return j;
            throw new Exception();
        }
        public int BallJ()
        {
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] == 4)
                        return j;
            throw new Exception();
        }
        public void Show()
        {
            int mini = 3000;
            int maxi = 0;
            int minj = 3000;
            int maxj = 0;
            for (int ii = 0; ii < map.GetLength(0); ii++)
            {
                for (int jj = 0; jj < map.GetLength(1); jj++)
                {
                    if (map[ii, jj] != 0)
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

            for (int ii = mini; ii <= maxi; ii++)
            {
                for (int jj = minj; jj <= maxj; jj++)
                {
                    Console.Write(map[ii, jj]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
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