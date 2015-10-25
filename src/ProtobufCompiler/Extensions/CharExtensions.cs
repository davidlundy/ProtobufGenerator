namespace ProtobufCompiler.Extensions
{
    internal static class CharExtensions
    {
        internal static bool IsCarriageReturn(this char self)
        {
            return '\r'.Equals(self);
        }

        internal static bool IsLineFeed(this char self)
        {
            return '\n'.Equals(self);
        }

        internal static bool IsForwardSlash(this char self)
        {
            return '/'.Equals(self);
        }

        internal static bool IsAsterisk(this char self)
        {
            return '*'.Equals(self);
        }

        /// <summary>
        /// Characters which may be placed beside another character and not be considered the same token. 
        /// </summary>
        internal static bool IsInlineToken(this char self)
        {
            return ';'.Equals(self) || '{'.Equals(self) || '}'.Equals(self) || '='.Equals(self) ||
                   ','.Equals(self) || '<'.Equals(self) || '>'.Equals(self) || '['.Equals(self) ||
                   ']'.Equals(self) || '('.Equals(self) || ')'.Equals(self);
        }
    }
}
