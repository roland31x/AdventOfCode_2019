using System.Runtime.InteropServices;

namespace D14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input.txt");
            foreach (string line in lines)
            {
                string main = line.Split("=>")[1].Trim();
                string comps = line.Split("=>")[0].Trim();
                Chemical target = Chemical.GetChemical(main.Split(' ')[1]);
                long craft = long.Parse(main.Split(' ')[0]);
                target.TotalCrafted = craft;
                string[] components = comps.Split(',');
                foreach(string component in components)
                {
                    long amount = long.Parse(component.Trim().Split(' ')[0].Trim());
                    Chemical comp = Chemical.GetChemical(component.Trim().Split(' ')[1].Trim());
                    target.components.Add(comp);
                    target.amount.Add(amount);
                }
            }

            Chemical fuel = Chemical.GetChemical("FUEL");
            Dictionary<Chemical,long> inventory = new Dictionary<Chemical,long>();

            long ores = 0;
            Craft(fuel, inventory, ref ores);

            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(ores);

            Dictionary<Chemical, long> initialinv = new Dictionary<Chemical, long>();

            long cost = ores;
            long times = 1_000_000_000_000 / ores;
            ores *= times;
            
            foreach(Chemical c in inventory.Keys)
            {
                initialinv[c] = inventory[c];
                inventory[c] *= times;
            }       

            while (true)
            {
                DestroyInventory(inventory, out long orediff);
                if (orediff == 0)
                    break;
                ores -= orediff;

                long again = (1_000_000_000_000 - ores) / cost;

                times += again;
                ores += cost * again;

                foreach (Chemical c in inventory.Keys)
                    inventory[c] += initialinv[c] * again;
            }
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(times);

            
            
        }
        static void DestroyInventory(Dictionary<Chemical,long> inventory, out long oresrecovered)
        {
            bool ok;
            do
            {
                ok = false;
                foreach (Chemical c in inventory.Keys)
                {
                    if (c == Chemical.GetChemical("ORE") || c == Chemical.GetChemical("FUEL"))
                        continue;
                    long times = inventory[c] / c.TotalCrafted;
                    if (times == 0)
                        continue;
                    ok = true;
                    for (int i = 0; i < c.components.Count; i++)
                        inventory[c.components[i]] += times * c.amount[i];
                    inventory[c] -= times * c.TotalCrafted;
                }
            } while (ok);
            oresrecovered = inventory[Chemical.GetChemical("ORE")];
            inventory[Chemical.GetChemical("ORE")] = 0;
        }
        static void Craft(Chemical tocraft, Dictionary<Chemical,long> inventory, ref long ores) 
        {
            if(tocraft == Chemical.GetChemical("ORE"))
            {
                if (!inventory.ContainsKey(tocraft))
                    inventory.Add(tocraft, 1);
                else
                    inventory[tocraft] += 1;
                ores++;
            }

            for (int i = 0; i < tocraft.components.Count; i++)
            {
                if (!inventory.ContainsKey(tocraft.components[i]))
                    inventory.Add(tocraft.components[i], 0);
                while (inventory[tocraft.components[i]] < tocraft.amount[i])                   
                    Craft(tocraft.components[i], inventory, ref ores);
                inventory[tocraft.components[i]] -= tocraft.amount[i];
            }

            if (!inventory.ContainsKey(tocraft))
                inventory.Add(tocraft, 0);
            inventory[tocraft] += tocraft.TotalCrafted;
        }
    }
    public class Chemical
    {
        public static Dictionary<string,Chemical> Chemicals = new Dictionary<string,Chemical>();
        public List<Chemical> components = new List<Chemical>();
        public List<long> amount = new List<long>();
        public long TotalCrafted;
        public long? value;
        string name;
        public Chemical(string name)
        {
            this.name = name;
            if(name == "ORE")
                value = 1;
            if(!Chemicals.ContainsKey(name))
                Chemicals.Add(name, this);
        }
        public static Chemical GetChemical(string name)
        {
            if (Chemicals.ContainsKey(name))
                return Chemicals[name];
            else
                return new Chemical(name);
        }
        public override string ToString()
        {
            return name;
        }
    }
}