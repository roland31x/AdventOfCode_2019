using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace D22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] commands = File.ReadAllLines(@"..\..\..\input.txt");
            CardStack cs = new CardStack(10007);
            foreach(string command in commands)
                cs.Evaluate(command);
            Console.WriteLine(cs.IndexOf(2019));

            long factorynumber =    119315717514047;
            long times =            101741582076661;
            CardStackBig cb = new CardStackBig(factorynumber);
            cb.BuildLinearFunction(commands, times);
            
            Console.WriteLine(cb.GetTargetPosition(2020));
        }
    }
    public static class BigIntExt
    {
        public static BigInteger Modulo(this BigInteger a, BigInteger m)
        {
            return (a % m + m) % m;
        }
        public static BigInteger Inv(this BigInteger a, BigInteger modulus)
        {
            return a.ModPow(modulus - 2, modulus);
        }
        public static BigInteger ModPow(this BigInteger b, BigInteger e, BigInteger m)
        {
            BigInteger r = 1;
            b = b % m;
            while(e > 0)
            {
                if((e % 2) == 1)
                    r = (r * b) % m;
                b = (b * b % m);
                e = e >> 1;
            }
            return r;
        }
    }
    public class CardStackBig
    {
        BigInteger count;
        BigInteger _a = 1;
        BigInteger _b = 0;
        public BigInteger A { get => _a; set { _a = value.Modulo(count); } }
        public BigInteger B { get => _b; set { _b = value.Modulo(count); } }
        public CardStackBig(BigInteger count)
        {
            this.count = count;
        }
        public void Evaluate(string command)
        {
            string com = command.Split(' ')[0];
            BigInteger newa;
            BigInteger newb;
            if (com == "cut")
                (newa, newb) = Cut(int.Parse(command.Split(' ')[1]));
            else if (int.TryParse(command.Split(' ').Last(), out int inc))
                (newa, newb) = DealInc(inc);
            else
                (newa, newb) = Deal();
            A = newa * A;
            B = newa * B + newb;
        }
        public (BigInteger, BigInteger) Cut(int cards) => (1, -cards);
        public (BigInteger, BigInteger) DealInc(int inc) => (inc, 0);
        public (BigInteger, BigInteger) Deal() => (-1, -1);
        public void BuildLinearFunction(string[] coms, BigInteger times)
        {
            foreach (string s in coms)
                Evaluate(s);
            BigInteger completeA = A.ModPow(times, count);
            BigInteger completeB = (B * (completeA - 1) * (A - 1).Inv(count)).Modulo(count);
            A = completeA;
            B = completeB;
        }
        public BigInteger GetTargetPosition(BigInteger targetpos)
        {
            return ((targetpos - B).Modulo(count) * A.Inv(count)).Modulo(count);
        }
    }
    public class CardStack
    {
        int count;
        int[] data;
        public int IndexOf(int card) => data.ToList().IndexOf(card);
        public int this[int card] => data[card];
        public CardStack(int Cards) 
        {
            count = Cards;
            data = new int[count];
            for (int i = 0; i < count; i++)
                data[i] = i;
        }
        public void Evaluate(string command)
        {
            string com = command.Split(' ')[0];
            if (com == "cut")
                Cut(int.Parse(command.Split(' ')[1]));
            else if (int.TryParse(command.Split(' ').Last(), out int inc))
                Deal(inc);
            else
                Deal();
        }
        public void Cut(int howmany)
        {
            int[] newdata = new int[count];
            if (howmany < 0)
            {
                howmany *= -1;
                for (int i = 0; i < howmany; i++)
                    newdata[i] = data[count - howmany + i];
                for (int i = howmany; i < count; i++)
                    newdata[i] = data[i - howmany];
            }
            else
            {
                for (int i = 0; i < howmany; i++)
                    newdata[count - howmany + i] = data[i];
                for (int i = 0; i < count - howmany; i++)
                    newdata[i] = data[howmany + i];
            }
            data = newdata;
        }
        public void Deal(int increment)
        {
            int[] newdata = new int[count];
            for (int i = 0; i < count; i++)
                newdata[(i * increment) % count] = data[i];
            data = newdata;               
        }
        public void Deal()
        {
            int[] newdata = new int[count];
            for (int i = 0; i < count; i++)
                newdata[i] = data[count - 1 - i];
            data = newdata;
        }
    }
}