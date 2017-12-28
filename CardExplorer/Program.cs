using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            int i = 20;
            Card card;
            while( i > 0)
            {
                try { 
                    card = Card.Factory(random.Next(0x10000));
                    Console.Out.WriteLine(card);
                }catch ( Exception e){Console.Out.WriteLine(e.Message);}
                i--;
            }

            Console.In.ReadLine();
        }
    }
}
