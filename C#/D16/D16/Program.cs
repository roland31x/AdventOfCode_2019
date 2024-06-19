using System.Text;

namespace D16
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputtext = File.ReadAllText(@"..\..\..\input.txt");
            int[] signal = new int[inputtext.Length];
            for (int i = 0; i < signal.Length; i++)
                signal[i] = int.Parse(inputtext[i].ToString());

            int offset = 0;
            for (int i = 0; i < 7; i++)
                offset += signal[i] * (int)Math.Pow(10, 6 - i);

            SignalAmplifier sa = new SignalAmplifier();
            for(int times = 0; times < 100; times++)
                signal = sa.Amplify(signal);

            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(signal.SubString(0,8));

            int initiallength = inputtext.Length;
            int totallength = 10000 * initiallength;
            int actuallength = totallength - offset;

            if(offset > totallength / 2)
            {
                // in this case the pattern is zeroes until the offset index, and positive +1s from it
                signal = new int[actuallength];
                for (int i = 0; i < signal.Length; i++)
                    signal[i] = int.Parse(inputtext[(offset + i) % inputtext.Length].ToString());

                for (int times = 0; times < 100; times++)
                    signal = sa.AmplifyEnd(signal);

                Console.WriteLine("Part 2 solution:");
                Console.WriteLine(signal.SubString(0, 8));
            }
            else
            {
                // get fucked?
            }            
        }
    }
    public class SignalAmplifier
    {
        int[] basepattern = new int[] { 0, 1, 0, -1 };
        public int[] Amplify(int[] input)
        {
            int[] toreturn = new int[input.Length];

            for(int i = 0; i < input.Length; i++)
                toreturn[i] = Calc2(input, i + 1);

            return toreturn;
        }
        public int[] AmplifyEnd(int[] input)
        {
            int[] toreturn = new int[input.Length];
            int lastmod = 0;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                toreturn[i] = (input[i] + lastmod) % 10;
                lastmod = toreturn[i];
            }               

            return toreturn;
        }
        public int Calc2(int[] input, int order)
        {
            int result = 0;

            for(int j = 1; j < (input.Length / order) + 1; j += 4)
                for (int i = j * order; i < j * order + order && i <= input.Length; i++)
                    result += input[i - 1];

            for (int j = 3; j < (input.Length / order) + 1; j += 4)
                for (int i = j * order; i < j * order + order && i <= input.Length; i++)
                    result -= input[i - 1];


            return Math.Abs(result) % 10;
        }
        public int Calc(int[] input, int order) 
        {
            int result = 0;
            int period = 1;
            int idx = 0;
            if(period == order)
            {
                period %= order;
                idx++;
            }
            for(int i = 0; i < input.Length; i++)
            {
                result += basepattern[idx] * input[i];
                period++;
                if (period == order)
                {
                    period %= order;
                    idx++;
                }
                idx %= 4;
            }

            return Math.Abs(result) % 10;
        }
    }
    public static class ArrayExtension
    {
        public static string SubString(this int[] array,int skip,int max)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0 + skip; i < skip + max && i < array.Length; i++)
                sb.Append(array[i].ToString());
            return sb.ToString();
        }
    }
}