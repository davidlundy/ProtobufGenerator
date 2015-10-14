using System;
#if DNXCORE50
using System.Globalization;
#endif

namespace ProtobufCompiler.Compiler
{
    internal class ParseError : IEquatable<ParseError>
    {
        internal Token Token { get; }
        internal string Message { get; }

#if DNXCORE50
        internal StringComparer InvCultIc = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.None);
#else
        internal StringComparer InvCultIc = StringComparer.InvariantCultureIgnoreCase;
#endif

        public ParseError(Token token, string message)
        {
            Token = token;
            Message = message;
        }

        public override string ToString()
        {
            return string.Format("{0} : @ {1}", Message, Token);
        }

        public bool Equals(ParseError other)
        {
            if (other == null) return false;
            return Token.Equals(other.Token) && InvCultIc.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as ParseError);
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + Token.GetHashCode();
            hash = (hash * 7) + Message.GetHashCode();
            return hash;
        }

    }
}
