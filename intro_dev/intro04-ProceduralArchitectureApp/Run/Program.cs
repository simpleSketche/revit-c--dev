using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Architecture;

namespace Run
{
    class Run
    {
        static void Main(string[] args)
        {
            int[] numbers = { 1, 2, 3, 4, 5 };
            string[] names = { "a", "b", "c", "d", "e" };
            architecture[] buildings = new architecture[10];
            List<architecture> bldgs = new List<architecture>();
            List<double> nums = new List<double>();
            nums.Add(0.5);

            Console.WriteLine(numbers);
            Console.WriteLine(names);
            Console.WriteLine(buildings);
            Console.WriteLine(bldgs);
            Console.WriteLine(nums);
            Console.WriteLine(0);

            Console.ReadKey(true);
        }
    }
}
