using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace C_2.Harmony
{
    public class MethodsToPatch
    {
        public int i = 10;
        public int j = 0;

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public int ReturnNumber1() //Transpiler
        {
            if (this == null)
                throw null;
            return this.i;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public int ReturnNumber2() //Prefix
        {
            if (this == null)
                throw null;
            return this.i;
        }

        public override string ToString()
        {
            return i.ToString();
        }

    }
}
