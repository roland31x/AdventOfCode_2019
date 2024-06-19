namespace D02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IntCode program = new IntCode(File.ReadAllText(@"..\..\..\input.txt"));
            program.OverridePosition(1, 12);
            program.OverridePosition(2, 2);
            program.Run();
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(program.Position(0));
            for(int noun = 0; noun < 100; noun++)
            {
                for(int verb = 0; verb < 100; verb++)
                {
                    program = new IntCode(File.ReadAllText(@"..\..\..\input.txt"));
                    program.OverridePosition(1, noun);
                    program.OverridePosition(2, verb);
                    program.Run();
                    if(program.Position(0) == 19690720)
                    {
                        Console.WriteLine("Part 2 solution:");
                        Console.WriteLine(100 * noun + verb);
                        return;
                    }
                }
            }                        
        }
    }
    public class IntCode
    {
        public int[] program;
        public IntCode(string ops)
        {
            string[] tokens = ops.Split(',');
            program = new int[tokens.Length];
            for(int i = 0; i < tokens.Length; i++)
            {
                program[i] = int.Parse(tokens[i]);
            }
        }
        public void Run()
        {
            int driver = 0;
            while (program[driver] != 99)
            {
                switch (program[driver])
                {
                    case 1:
                        program[program[driver + 3]] = program[program[driver + 1]] + program[program[driver + 2]];
                        driver += 4;
                        break;
                    case 2:
                        program[program[driver + 3]] = program[program[driver + 1]] * program[program[driver + 2]];
                        driver += 4;
                        break;
                    default:
                        throw new Exception(program[driver] + " opcode unkown");
                }
            }
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