using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2GEN.Helpers
{
    public static class HandlingHelper
    {
        public static long TicksFromJSToNET(long Ticks)
        {
            return (Ticks * 10000) + 621356148000000000;
        }
        public static long TicksFromNETToJS(long Ticks)
        {
            return (Ticks - 621356148000000000) / 10000;
        }
    }
}
