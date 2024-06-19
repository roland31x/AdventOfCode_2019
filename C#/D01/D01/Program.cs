namespace D01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\..\input.txt");
            int sum1 = 0;
            int sum2 = 0;
            for(int i = 0; i < lines.Length; i++)
            {
                int modulefuel = int.Parse(lines[i]) / 3 - 2;
                sum1 += modulefuel;
                do
                {
                    sum2 += modulefuel;
                    modulefuel /= 3;
                    modulefuel -= 2;
                } while (modulefuel > 0);
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(sum1);
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(sum2);
        }
    }
}