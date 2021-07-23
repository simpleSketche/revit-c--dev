using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intro01_MathFun
{
    class Program
    {
        static void Main(string[] args)
        {
            int itemACount = 10000;
            float itemAPrice = 1.66f;
            int itemBCount = 9843;
            float itemBPrice = 2.45f;
            float theSum = getSum(itemACount, itemAPrice, itemBCount, itemBPrice);

            Console.WriteLine($"The sum between itemA and itemB is: {0}", theSum);
        }
        public static float getSum(int itemACount, float itemAPrice, int itemBCount, float itemBPrice)
        {
            return itemACount * itemAPrice + itemBCount * itemBPrice;
        }
    }
}
