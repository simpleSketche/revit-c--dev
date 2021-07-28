using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 30;
            int goalCount = count;
            int letterCount = 26;

            List<string> allLetters = new List<string> {"A","B","C","D","E","F",
            "G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X",
            "Y","Z"};
            string header = "";
            List<string> headers = new List<string>();
            float loopCount = count / 26;
            int outerLoopCount = Convert.ToInt32(Floor(loopCount) + 1);

            for (int i = 0; i < outerLoopCount; i++)
            {
                header = defaultLetters(header);
                for (int j = 0; j < letterCount; j++)
                {
                    var headerTx = header.ToCharArray();
                    headerTx[i] = allLetters[j].ToCharArray()[0];
                    Console.WriteLine(headerTx);
                    string newHeader = new string(headerTx);
                    headers.Add(newHeader);
                    if (goalCount == 0) break;
                    goalCount -= 1;
                }

            }
        }
        public static string defaultLetters(string header)
        {
            var newheader = "";

            for (int i = 0; i < header.Count() + 1; i++)
            {
                newheader += "A";
            }
            return newheader;
        }
    }
}

