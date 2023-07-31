namespace D07
{
    internal class Program
    {
        static string program = "";
        static void Main(string[] args)
        {
            program = File.ReadAllText(@"..\..\..\input.txt");
            int best = 0;
            int[] data = new int[] { 0, 1, 2, 3, 4 };
            Back(ref best, data, part2: false);
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(best);
            data = new int[] { 5, 6, 7, 8, 9 };
            Back(ref best, data, part2: true);
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(best);
        }
        static void Back(ref int best, int[] data, bool part2)
        {
            int[] ints = data;
            int[] output = new int[ints.Length];
            int[] selected = new int[ints.Length];
            Back(0, ints.Length, ints, output, selected, ref best, part2);
        }
        static void Back(int k, int n, int[] data, int[] output, int[] selected, ref int best, bool part2)
        {
            if(k >= n)
            {
                int result = 0;
                IntCode[] amps = new IntCode[5];
                for(int i = 0; i < 5; i++)
                {
                    amps[i] = new IntCode(program);
                    amps[i].inputs.Enqueue(output[i]);
                }
                amps[0].inputs.Enqueue(0);
                bool ok = false;
            RunAgain: for (int i = 0; i < 5; i++)
                {                    
                    IntCode amp = amps[i];
                    ok = amp.Run(); // ok becomes false if program is waiting for input instead of halting
                    while (amp.outputs.Any())
                    {
                        amps[(i + 1) % 5].inputs.Enqueue(amp.outputs.First());
                        amp.outputs.RemoveAt(0);
                    }
                }
                while(part2 && !ok)
                {
                    goto RunAgain;
                }

                result = amps[0].inputs.First();
                if(result > best)
                {
                    best = result;
                }
            }
            else
            {
                for(int i = 0; i < data.Length; i++)
                {
                    if (selected[i] == 0)
                    {
                        selected[i] = 1;
                        output[k] = data[i];
                        Back(k + 1, n, data, output, selected, ref best, part2);
                        selected[i] = 0;
                    }
                }
            }
        }
    }
    public class IntCode
    {
        public int[] program;
        int driver = 0;
        public List<int> outputs = new List<int>();
        public Queue<int> inputs = new Queue<int>();
        int definput = 0;
        public IntCode(string ops)
        {
            //definput = defaultinput;
            string[] tokens = ops.Split(',');
            program = new int[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                program[i] = int.Parse(tokens[i]);
            }

            this.inputs = inputs;
        }
        public int Output()
        {
            return outputs.Last();
        }
        public bool Run()
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
            //Console.WriteLine(outputs.Last());
        }
        void Dissect(int opcode)
        {
            int[] op = new int[5];
            int idx = 4;
            while (opcode > 0)
            {
                op[idx] = opcode % 10;
                opcode /= 10;
                idx--;
            }
            bool firstimm = false;
            bool secondimm = false;
            bool thirdimm = false;
            if (op[0] == 1)
                thirdimm = true;
            if (op[1] == 1)
                secondimm = true;
            if (op[2] == 1)
                firstimm = true;
            int actualcode = 10 * op[3] + op[4];
            switch (actualcode)
            {
                case 1:
                    Addition(firstimm, secondimm);
                    driver += 4;
                    break;
                case 2:
                    Multiply(firstimm, secondimm);
                    driver += 4;
                    break;
                case 3:
                    Input(firstimm);
                    driver += 2;
                    break;
                case 4:
                    Output(firstimm);
                    driver += 2;
                    break;
                case 5:
                    if (!JumpIfTrue(firstimm, secondimm))
                        driver += 3;
                    break;
                case 6:
                    if (!JumpIfFalse(firstimm, secondimm))
                        driver += 3;
                    break;
                case 7:
                    LessThan(firstimm, secondimm);
                    driver += 4;
                    break;
                case 8:
                    Equality(firstimm, secondimm);
                    driver += 4;
                    break;
                default:
                    throw new Exception(actualcode + " unknown opcode!");

            }
        }
        bool JumpIfTrue(bool firstimm, bool secondimm)
        {
            if (program[firstimm ? driver + 1 : program[driver + 1]] != 0)
            {
                driver = program[secondimm ? driver + 2 : program[driver + 2]];
                return true;
            }
            return false;
        }
        bool JumpIfFalse(bool firstimm, bool secondimm)
        {
            if (program[firstimm ? driver + 1 : program[driver + 1]] == 0)
            {
                driver = program[secondimm ? driver + 2 : program[driver + 2]];
                return true;
            }
            return false;
        }
        void LessThan(bool firstimm, bool secondimm)
        {
            if (program[firstimm ? driver + 1 : program[driver + 1]] < program[secondimm ? driver + 2 : program[driver + 2]])
            {
                program[program[driver + 3]] = 1;
            }
            else
                program[program[driver + 3]] = 0;
        }
        void Equality(bool firstimm, bool secondimm)
        {
            if (program[firstimm ? driver + 1 : program[driver + 1]] == program[secondimm ? driver + 2 : program[driver + 2]])
            {
                program[program[driver + 3]] = 1;
            }
            else
                program[program[driver + 3]] = 0;
        }
        void Addition(bool firstimm, bool secondimm)
        {
            program[program[driver + 3]] = program[firstimm ? driver + 1 : program[driver + 1]] + program[secondimm ? driver + 2 : program[driver + 2]];
        }
        void Multiply(bool firstimm, bool secondimm)
        {
            program[program[driver + 3]] = program[firstimm ? driver + 1 : program[driver + 1]] * program[secondimm ? driver + 2 : program[driver + 2]];
        }
        void Input(bool firstimm)
        {
            program[firstimm ? driver + 1 : program[driver + 1]] = inputs.Dequeue();
        }
        void Output(bool firstimm)
        {
            outputs.Add(program[firstimm ? driver + 1 : program[driver + 1]]);
        }
        public void OverridePosition(int position, int value)
        {
            program[position] = value;
        }
        public int Position(int i)
        {
            return program[i];
        }
    }
}