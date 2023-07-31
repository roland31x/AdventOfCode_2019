namespace D06
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using(StreamReader sr = new StreamReader(@"..\..\..\input.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string[] planets = sr.ReadLine()!.Split(')');
                    Planet parent = Planet.GetPlanet(planets[0]);
                    Planet orbit = Planet.GetPlanet(planets[1]);
                    parent.Children.Add(orbit);
                    orbit.Parent = parent;
                }
            }
            int count = 0;
            foreach(Planet p in Planet.Planets)
            {
                Planet curr = p;
                while(curr.Parent != null)
                {
                    curr = curr.Parent;
                    count++;
                }
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(count);

            count = 0;
            Planet me = Planet.GetPlanet("YOU").Parent!;
            Planet santa = Planet.GetPlanet("SAN").Parent!;
            List<Planet> parents = new List<Planet>();
            Planet current = me;
            while(current.Parent != null)
            {
                parents.Add(current.Parent);
                current = current.Parent;
            }
            current = santa;
            while (!parents.Contains(current)) // just gotta find the first common parent
            {
                count++;
                current = current.Parent;
            }
            count += parents.IndexOf(current) + 1;
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(count);
        }
    }
    public class Planet
    {
        public static List<Planet> Planets = new List<Planet>();
        public static Planet GetPlanet(string name)
        {
            foreach (Planet planet in Planets)
            {
                if (planet.Name == name)
                {
                    return planet;
                }
            }
            return new Planet(name);
        }
        public string Name { get; }
        public Planet? Parent { get; set; }
        public List<Planet> Children = new List<Planet>();
        public Planet(string name)
        {
            Name = name;
            Planets.Add(this);
        }
        public override string ToString()
        {
            return Name;
        }
    }
}