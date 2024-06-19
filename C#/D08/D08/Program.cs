using System.IO;

namespace D08
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = File.ReadAllText(@"..\..\..\input.txt");
            LayeredImage img = new LayeredImage(6, 25, input);
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(img.P1Score());
            Console.WriteLine("Part 2 solution:");
            WriteMat(img.CalcImage());
        }
        static void WriteMat(int[,] mat)
        {
            Console.WriteLine();
            for(int i = 0; i < mat.GetLength(0); i++)
            {
                for(int j = 0;  j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 0)
                        Console.Write('.');
                    else
                        Console.Write("#");
                }
                Console.WriteLine();
            }
        }
    }
    public class LayeredImage
    {
        int H;
        int W;
        List<int[,]> layers = new List<int[,]>();
        List<int> layerzeros = new List<int>();
        public LayeredImage(int h, int w, string input)
        {
            this.H = h;
            this.W = w;
            int driver = 0;
            while (driver < input.Length)
            {
                int[,] layer = new int[H, W];
                int zeroes = 0;
                for(int i = 0; i < H; i++)
                {
                    for(int j = 0; j < W; j++)
                    {
                        layer[i, j] = input[driver] - '0';
                        if (layer[i, j] == 0)
                            zeroes++;
                        driver++;
                    }
                }
                layers.Add(layer);
                layerzeros.Add(zeroes);
            }
        }
        public int P1Score()
        {
            int ones = 0;
            int twos = 0;
            int[,] best = layers[layerzeros.IndexOf(layerzeros.Min())];
            for(int i = 0; i < H; i++)
            {
                for(int j = 0; j < W; j++)
                {
                    if (best[i, j] == 1)
                        ones++;
                    if (best[i, j] == 2)
                        twos++;
                }
            }
            return ones * twos;
        }
        public int[,] CalcImage()
        {
            int[,] final = new int[H, W];
            for(int i = 0; i < H; i++)
            {
                for(int j = 0; j < W; j++)
                {
                    int layer = 0;
                    while (layers[layer][i,j] == 2)
                    {
                        layer++;
                    }
                    final[i, j] = layers[layer][i, j];
                }
            }
            return final;
        }
    }
}