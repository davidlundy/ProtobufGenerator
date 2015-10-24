using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtobufCompiler.Extensions
{
    internal static class CharExtensions
    {
        public static bool IsCarriageReturn(this char self)
        {
            return '\r'.Equals(self);
        }

        public static bool IsLineFeed(this char self)
        {
            return '\n'.Equals(self);
        }
    }
}
