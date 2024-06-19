namespace D04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "165432-707912";
            int startidx = int.Parse(input.Split('-')[0]);
            int endidx = int.Parse(input.Split('-')[1]);
            int count1 = 0;
            int count2 = 0;
            for(int i = startidx; i <= endidx; i++) 
            {
                bool db = false;
                bool dec = true;
                int number = i;
                
                int digit = 11;
                while(number > 0)
                {                    
                    if (number % 10 > digit)
                    {
                        dec = false;
                        break;
                    }                       
                    if (number % 10 == digit)
                        db = true;
                    digit = number % 10;
                    number /= 10;
                }
                if (dec && db)
                {
                    count1++;

                    db = false;
                    string nr = i.ToString();
                    int k = 0;
                    while(k < nr.Length - 1)
                    {
                        if (nr[k] == nr[k + 1])
                        {
                            if(k == 0)
                            {                               
                                if(nr[k + 2] != nr[k])
                                {
                                    db = true;
                                }
                            }
                            else if(k == nr.Length - 2)
                            {
                                if (nr[k - 1] != nr[k])
                                {
                                    db = true;
                                }
                            }
                            else
                            {
                                if (nr[k - 1] != nr[k] && nr[k + 2] != nr[k])
                                {
                                    db = true;
                                }
                            }                          
                        }
                        k++;
                    }
                    if (dec && db)
                        count2++;
                }
                    
            }
            Console.WriteLine("Part 1 solution:");
            Console.WriteLine(count1);
            Console.WriteLine("Part 2 solution:");
            Console.WriteLine(count2);
        }
    }
}