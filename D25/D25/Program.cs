using System.Text;

namespace D25
{
    internal class Program
    {
        static List<Node> nodes = new List<Node>();
        static List<string> currentitems = new List<string>();
        static Node current = new Node("Hull breach");
        static void Main(string[] args)
        {
            string line = File.ReadAllText(@"..\..\..\input.txt");
            IntCode prog = new IntCode(line);
            IntCode clone = prog.Clone();
            prog.Run();

                //List<string> lines = prog.OutputASCII();
                //foreach (string l in lines)
                //    Console.WriteLine(l);
                //prog.InputASCII(Console.ReadLine());
                //prog.Run();
            DiscoverMap(prog);
            PickUpAllItems(ref prog);
            Node securitycp = nodes.First(x => x.IsSecurityCheckpoint);
            MoveTo(securitycp, prog);
            current = securitycp;
            Solve(prog);
        }
        static void Solve(IntCode prog)
        {
            string command = "north";
            List<string> outs = prog.OutputASCII();
            if (outs.Contains("- south") && current.S == null)
                command = "south";
            else if (outs.Contains("- east") && current.E == null)
                command = "east";
            else if (outs.Contains("- west") && current.W == null)
                command = "west";

            List<string> dropped = new List<string>();
            //while (outs.Contains("Command?"))
            //{
            //    prog.InputASCII(command);
            //    prog.Run();
            //    outs = prog.OutputASCII();
            //    for(int i = 0; i < currentitems.Count; i++)
            //    {
            //        dropped.Add(currentitems.First());
            //        prog.InputASCII("drop " + currentitems.First());
            //        prog.Run();
            //        prog.OutputASCII();
            //    }
            //}
            bool done = false;
            ItemCombine(currentitems, dropped,0, prog, ref done, command);
        }
        static void ItemCombine(List<string> items, List<string> result, int start, IntCode prog, ref bool done, string maincommand)
        {
            if (done)
                return;
            else
            {
                List<string> droppedhere = new List<string>();
                foreach (string s in items.Except(result))
                {
                    prog.InputASCII("drop " + s);
                    prog.Run();
                    prog.OutputASCII();
                    droppedhere.Add(s);
                }
                prog.InputASCII(maincommand);
                prog.Run();
                List<string> outs = prog.OutputASCII();
                if(!outs.Contains("Command?"))
                {
                    done = true;
                    foreach (string s in outs)
                        Console.WriteLine(s);
                }    
                foreach (string s in droppedhere)
                {
                    prog.InputASCII("take " + s);
                    prog.Run();
                    prog.OutputASCII();
                }
                
                
                   
                for (int i = start; i < items.Count; i++)
                {
                    result.Add(items[i]);
                    ItemCombine(items, result, i + 1, prog, ref done, maincommand);
                    result.Remove(items[i]);
                }
            }
            
        }
        static void PickUpAllItems(ref IntCode prog)
        {
            List<string> baditems = new List<string>();
            List<string> available = nodes.Where(x => x.items.Count > 0).SelectMany(x => x.items).ToList();
            while (available.Any())
            {
                Node target = nodes.First(x => x.items.Contains(available.First()));
                List<string> outs = prog.OutputASCII();

                MoveTo(target, prog);
                current = target;

                outs = prog.OutputASCII();

                IntCode clone = prog.Clone();
                string command = "take " + available.First();
                prog.InputASCII(command);
                prog.Run();

                outs = prog.OutputASCII();
                prog.InputASCII("inv");
                prog.Run();
                outs = prog.OutputASCII();
                bool OkMove = false;
                foreach(string line in outs)
                    if (line.Contains("Items"))
                        OkMove = true;

                if(!OkMove)
                {
                    prog = clone;
                    baditems.Add(available.First());
                    available = available.Except(baditems).ToList();
                }
                else
                {
                    currentitems.Add(available.First());
                    available.Remove(available.First());                   
                }
            }
        }
        static void DiscoverMap(IntCode prog)
        {
            current.visited = true;
            do
            {
                current.visited = true;
                List<string> outs = prog.OutputASCII();
                Analyze(outs, out bool CanMove);
                if (!nodes.Where(x => !x.visited).Any())
                    break;
                MoveTo(nodes.First(x => !x.visited), prog);
                current = nodes.First(x => !x.visited);
                
            } while(true);
        }
        static void MoveTo(Node target, IntCode program)
        {
            bool[] visited = new bool[nodes.Count];
            List<Node> dfs = new List<Node>();
            List<Node> result = new List<Node>();
            dfs.Add(current);
            visited[nodes.IndexOf(current)] = true;
            DFS(current, target, visited, dfs, result);

            for(int i = 0; i < result.Count - 1; i++)
            {
                List<string> outs = program.OutputASCII();
                string input = "north";
                if (result[i + 1] == result[i].S)
                    input = "south";
                if (result[i + 1] == result[i].E)
                    input = "east";
                if (result[i + 1] == result[i].W)
                    input = "west";
                program.InputASCII(input);
                program.Run();
            }
        }
        static void DFS(Node current, Node target, bool[] visited, List<Node> dfs, List<Node> res)
        {
            if (res.Any())
                return;
            if (current == target)
                foreach(Node n in dfs)
                    res.Add(n);
            else
            {
                foreach(Node n in current.GetNeighbors())
                {
                    if (visited[nodes.IndexOf(n)] == false)
                    {
                        dfs.Add(n);
                        visited[nodes.IndexOf(n)] = true;
                        DFS(n, target, visited, dfs, res);
                        dfs.Remove(n);
                        visited[nodes.IndexOf(n)] = false;
                    }
                }
            }
        }
        static void Analyze(List<string> outs, out bool CanMove)
        {
            CanMove = false;
            current.Name = outs[0];
            if(current.Name == "== Security Checkpoint ==")
            {
                CanMove = true;
                current.IsSecurityCheckpoint = true;
                return;
            }
            for(int i = 0; i < outs.Count; i++)
            {
                if (outs[i] == "Doors here lead:")
                {
                    CanMove = true;
                    while (i + 1 < outs.Count && outs[i + 1].Contains("-"))
                    {
                        i++;
                        string s = outs[i];
                        if (s.Contains("south"))
                            if (current.S == null)
                            {
                                if (!current.IsSecurityCheckpoint)
                                    current.S = new Node();
                                current.S.N = current;
                            }
                                
                        if (s.Contains("north"))
                            if (current.N == null)
                            {
                                if (!current.IsSecurityCheckpoint)
                                    current.N = new Node();
                                current.N.S = current;
                            }
                        if (s.Contains("west"))
                            if (current.W == null)
                            {
                                if (!current.IsSecurityCheckpoint)
                                    current.W = new Node();
                                current.W.E = current;
                            }
                        if (s.Contains("east"))
                            if (current.E == null)
                            {
                                if (!current.IsSecurityCheckpoint)
                                    current.E = new Node();
                                current.E.W = current;
                            }
                    }
                }
                if (outs[i] == "Items here:")
                {
                    CanMove = true;
                    while (i + 1 < outs.Count && outs[i + 1].Contains('-'))
                    {
                        i++;
                        if (outs[i].Contains("loop"))
                            continue;
                        current.items.Add(outs[i].Replace("-","").Trim());
                    }
                }
                if (outs[i] == "Command?")
                {
                    CanMove = true;
                }
            }             

        }
        internal class Node
        {
            public Node? N;
            public Node? S;
            public Node? W;
            public Node? E;
            public bool IsSecurityCheckpoint = false;
            public List<string> items = new List<string>();
            public bool visited = false;
            public string Name;
            public Node()
            {
                nodes.Add(this);
            }
            public Node(string name) 
            {
                Name = name;
                nodes.Add(this);
            }
            public List<Node> GetNeighbors()
            {
                List<Node> toreturn = new List<Node>();
                if(N != null)
                    toreturn.Add(N);
                if(S != null) 
                    toreturn.Add(S);
                if(W != null) 
                    toreturn.Add(W);
                if(E != null) 
                    toreturn.Add(E);
                return toreturn;
            }
            public override string ToString()
            {
                return Name;
            }

        }
    }
    public class IntCode
    {
        public long[] program;
        protected long driver = 0;
        protected long relativebase = 0;
        int memsize = 20000; // please adjust this 
        public List<long> outputs = new List<long>();
        public Queue<long> inputs = new Queue<long>();
        public IntCode(string ops)
        {
            string[] tokens = ops.Split(',');
            program = new long[memsize];
            for (int i = 0; i < tokens.Length; i++)
            {
                program[i] = long.Parse(tokens[i]);
            }
        }
        public IntCode Clone()
        {
            IntCode toreturn = new IntCode("1");
            toreturn.driver = driver;
            toreturn.relativebase = relativebase;
            for (int i = 0; i < memsize; i++)
                toreturn.program[i] = program[i];
            return toreturn;
        }
        public void InputASCII(string input)
        {
            for (int i = 0; i < input.Length; i++)
                inputs.Enqueue((int)input[i]);
            inputs.Enqueue(10);
        }
        public List<string> OutputASCII()
        {
            List<string> outs = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (int i in outputs)
            {
                if (i == 10)
                {
                    outs.Add(sb.ToString());
                    sb.Clear();
                }
                else if (i < 256)
                    sb.Append((char)i);
            }
            outputs.Clear();
            return outs.Where(x => x.Length > 1).ToList();
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