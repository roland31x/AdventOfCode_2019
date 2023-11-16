using System;
using System.Linq.Expressions;

namespace D23
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line = File.ReadAllText(@"..\..\..\input.txt");
            List<IntCode> computers = new List<IntCode>();
            for(int i = 0; i < 50; i++)
            {
                IntCode computer = new IntCode(line);
                computer.inputs.Enqueue(i);
                computers.Add(computer);
                computer.boot = true;
                computer.Run();
                computer.boot = false;
                computer.block = false;
            }
            long lastsent = long.MinValue;
            bool[] idle = new bool[50];
            bool ok = false;
            long NATX = 0;
            long NATY = 0;
            int count = 0;
            while (!ok)
            {
                for (int k = 0; k < computers.Count; k++)
                {
                    computers[k].Step();
                    if (computers[k].block)
                        idle[k] = true;
                    else
                        idle[k] = false;
                    if (computers[k].outputs.Count == 3)
                    {
                        for(int j = 0; j < computers[k].outputs.Count; j += 3)
                        {
                            long a = computers[k].outputs[j];                          
                            long x = computers[k].outputs[j + 1];
                            long y = computers[k].outputs[j + 2];

                            if (a == 255)
                            {
                                if(count == 0)
                                {
                                    Console.WriteLine("Part 1 solution:");
                                    Console.WriteLine(y);
                                }
                                  
                                count++;
                                NATX = x;
                                NATY = y;
                                continue;
                            }
                            computers[(int)a].inputs.Enqueue(x);
                            computers[(int)a].inputs.Enqueue(y);
                        }
                        computers[k].outputs.Clear();
                    }
                }
                bool transmit = true;
                foreach (bool t in idle)
                    transmit &= t;
                if (transmit)
                {
                    //Console.WriteLine(NATY);
                    foreach(IntCode c in computers)
                        c.block = false;
                    computers[0].inputs.Enqueue(NATX);
                    computers[0].inputs.Enqueue(NATY);
                    if (lastsent == NATY)
                    {
                        ok = true;
                        Console.WriteLine("Part 2 solution:");
                        Console.WriteLine(NATY);
                    }                      
                    else
                        lastsent = NATY;
                }
                    
            }
        }
    }
    public class IntCode
    {
        public long[] program;
        long driver = 0;
        long relativebase = 0;
        int memsize = 10000; // please adjust this 
        public List<long> outputs = new List<long>();
        public Queue<long> inputs = new Queue<long>();
        public bool block = false;
        public bool boot = false;
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
        public void Step()
        {
            Dissect(program[driver]);
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
            try
            {
                block = false;
                program[firstparam] = inputs.Dequeue();
            }
            catch (Exception)
            {
                if (boot)
                    throw new Exception();
                block = true;
                program[firstparam] = -1;
            }
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