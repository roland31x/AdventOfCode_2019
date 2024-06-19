namespace D12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input.txt");
            MoonSystem sys = new MoonSystem();
            foreach (string line in lines)
            {
                int[] vel = new int[3];
                int[] pos = new int[3];
                pos[0] = int.Parse(line.Split(',')[0].Replace("<x=","").Trim());
                pos[1] = int.Parse(line.Split(',')[1].Replace("y=", "").Trim());
                pos[2] = int.Parse(line.Split(',')[2].Replace("z=", "").Replace(">","").Trim());
                sys.moons.Add(new Moon(pos, vel));
            }
            sys.SetInitial();
            for(int t = 0; t < 1000; t++)
            {
                sys.Sim();
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(sys.Total());
            bool done = false;
            while (!done)
                done = sys.Sim();
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(sys.Repeat());
        }
    }
    public class MoonSystem
    {
        public List<Moon> moons = new List<Moon>();
        int[,] initialstate;
        int[] repeat = new int[3];
        int steps = 0;
        public MoonSystem() { }
        public void SetInitial()
        {
            initialstate = GetState();
        }
        public string Repeat()
        {
            long xr = repeat[0];
            long yr = repeat[1];
            long zr = repeat[2];

            return LCM(xr,LCM(yr,zr)).ToString();

        }
        static long GCD(long a, long b)
        {
            if (a == 0)
                return b;
            return GCD(b % a, a);
        }
        static long LCM(long a, long b)
        {
            return (a / GCD(a, b)) * b;
        }
        public bool Sim()
        {
            for (int i = 0; i < moons.Count; i++)
            {
                for (int j = i + 1; j < moons.Count; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (moons[i].pos[k] < moons[j].pos[k])
                        {
                            moons[i].vel[k]++;
                            moons[j].vel[k]--;
                        }
                        else if(moons[i].pos[k] > moons[j].pos[k])
                        {
                            moons[i].vel[k]--;
                            moons[j].vel[k]++;
                        }
                    }
                }
            }
            steps++;
            for(int i = 0; i < moons.Count; i++)
                for(int k = 0; k < 3; k++)
                    moons[i].pos[k] += moons[i].vel[k];

            int[,] current = GetState();

            for(int k = 0; k < 3; k++)
            {
                bool r = true;
                for(int i = 0; i < moons.Count; i++)
                {
                    if (current[i, k] != initialstate[i, k] || current[i, k + 3] != initialstate[i, k + 3])
                        r = false;
                }
                if(r && repeat[k] == 0)
                    repeat[k] = steps;
            }

            bool done = true;
            for(int i = 0; i < 3; i++)
                if (repeat[i] == 0)
                    done = false;
            return done;
        }
        public int Total()
        {
            int toreturn = 0;
            foreach (Moon m in moons)
                toreturn += m.Energy();
            return toreturn;
        }
        int[,] GetState()
        {
            int[,] tor = new int[moons.Count, 6];
            for (int i = 0; i < moons.Count; i++)
                for (int k = 0; k < 3; k++)
                {
                    tor[i, k] = moons[i].pos[k];
                    tor[i, k + 3] = moons[i].vel[k];
                }
            return tor;
                    
        }
    }
    public class Moon
    {
        public int[] pos;
        public int[] vel;
        public Moon(int[] pos, int[] vel)
        {
            this.pos = pos;
            this.vel = vel;
        }
        public int Energy()
        {
            int pot = 0;
            int kin = 0;
            foreach(int i in pos) 
                pot += Math.Abs(i);
            foreach(int i in vel)
                kin += Math.Abs(i);
            return pot * kin;
        }
    }
}