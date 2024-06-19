namespace D19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string lines = File.ReadAllText(@"..\..\..\input.txt");
            
            int count = 0;
            for(int i = 0; i < 50; i++)
                for(int j = 0; j < 50; j++)
                    if (GetInfo(i, j, lines) == 1)
                        count++;
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(count);

            int line = 100;
            int startj = 0;
            while (true)
            {
                int result = GetInfo(line, startj, lines);
                if (result == 1)
                    break;
                startj++;
            }

            while (true)
            {
                int bottomleft = GetInfo(line, startj, lines);
                if(bottomleft == 1)
                {
                    bool ok = true;
                    int topleft = GetInfo(line - 99, startj, lines);                  
                    int topright = GetInfo(line - 99, startj + 99, lines);
                    int bottomright = GetInfo(line, startj + 99, lines);
                    if (topleft == 0 || topright == 0 || bottomright == 0)
                        ok = false;
                    if (ok)
                        break;
                }
                if (GetInfo(line + 1, startj, lines) == 0)
                    startj++;
                line++;
            }
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine((line - 99) * 10000 + startj);
        }
        static int GetInfo(int i, int j, string lines)
        {
            if (i < 0 || j < 0)
                return 0;
            IntCode prog = new IntCode(lines);
            prog.inputs.Enqueue(i);
            prog.inputs.Enqueue(j);
            prog.Run();
            int result = (int)prog.outputs.Last();
            return result;
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