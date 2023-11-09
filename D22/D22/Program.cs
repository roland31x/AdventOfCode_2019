using System.Data;
using System.Reflection.Metadata.Ecma335;

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