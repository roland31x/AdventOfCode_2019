using System.Xml;

namespace D05
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntCode program = new IntCode(File.ReadAllText(@"..\..\..\input.txt"), 1);
            Console.WriteLine("Part 1 solution:");
            program.Run();
            program = new IntCode(File.ReadAllText(@"..\..\..\input.txt"), 5);
            Console.WriteLine("Part 2 solution:");
            program.Run();
        }
    }
    public class IntCode
    {
        public int[] program;
        int driver = 0;
        List<int> outputs = new List<int>();
        int definput = 0;
        public IntCode(string ops, int defaultinput)
        {
            definput = defaultinput;
            string[] tokens = ops.Split(',');
            program = new int[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                program[i] = int.Parse(tokens[i]);
            }
        }
        public void Run()
        {
            while (program[driver] != 99)
            {
                Dissect(program[driver]);
            }
            Console.WriteLine(outputs.Last());
        }
        void Dissect(int opcode)
        {
            int[] op = new int[5];
            int idx = 4;
            while(opcode > 0)
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
                    if(!JumpIfTrue(firstimm, secondimm))
                        driver += 3;
                    break;
                case 6:
                    if(!JumpIfFalse(firstimm, secondimm))
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
            if(program[firstimm ? driver + 1 : program[driver + 1]] != 0)
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
            program[firstimm ? driver + 1 : program[driver + 1]] = definput;
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