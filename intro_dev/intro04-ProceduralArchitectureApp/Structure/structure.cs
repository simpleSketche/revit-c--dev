using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Architecture;

namespace Structure
{
    public class structure
    {
        public void CalRoomWidth()
        {
            double width = architecture.RoomWidth();
            Console.WriteLine(width);
        }
    }
}
