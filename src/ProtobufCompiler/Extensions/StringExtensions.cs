using System;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Extensions
{
    public static class StringExtensions
    {
#if DNXCORE50
        private static readonly StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        private static readonly StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        public static bool EqualsIgnoreCase(this string self, string other)
        {
            return !ReferenceEquals(self, null) && InvCultIc.Equals(self, other);
        }
    }
}
