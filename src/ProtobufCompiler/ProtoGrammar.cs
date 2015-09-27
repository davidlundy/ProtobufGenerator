using ProtobufCompiler.Types;
using Sprache;
using System;
using System.Linq;

namespace ProtobufCompiler
{
    internal class ProtoGrammar
    {
        /// <summary>
        /// A <see cref="HexDigit"/> has a range from 0-9 and from A-F (case insensitive)
        /// </summary>
        internal virtual Parser<char> HexDigit
        {
            get
            {
                return Parse.Char(t => (t >= '0') && (t <= '9') || (char.ToLower(t) >= 'a') && (char.ToLower(t) <= 'f'), "Hex Digit");
            }
        }

        /// <summary>
        /// A <see cref="OctalDigit"/> has a range from 0 to 7
        /// </summary>
        internal virtual Parser<char> OctalDigit
        {
            get
            {
                return Parse.Char(t => (t >= '0') && (t <= '7'), "Octal Digit");
            }
        }

        /// <summary>
        /// A <see cref="Identifier"/> is a string of Letters, Digits, and Underscores that begin with a Letter.
        /// </summary>
        internal virtual Parser<string> Identifier
        {
            get
            {
                return Parse.Identifier(Parse.Letter, Parse.Char(t => (char.IsLetterOrDigit(t) || t == '_'), "Letter, Digit, or Underscore"));
            }
        }

        /// <summary>
        /// A <see cref="CapitalizedIdentifier"/> is an <see cref="Identifier"/> which must start with an upper case letter. 
        /// </summary>
        internal virtual Parser<string> CapitalizedIdentifier
        {
            get
            {
                return Parse.Identifier(Parse.Upper, Parse.Char(t => (char.IsLetterOrDigit(t) || t == '_'), "Letter, Digit, or Underscore"));
            }
        }

        #region Identifier Definitions
        /// <summary>
        /// A <see cref="MessageName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> MessageName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="EnumName"/>is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> EnumName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="FieldName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> FieldName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="OneOfName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> OneOfName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="MapName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> MapName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="ServiceName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> ServiceName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="RpcName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> RpcName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        /// <summary>
        /// A <see cref="StreamName"/> is defined as an <see cref="Identifier"/>
        /// </summary>
        internal virtual Parser<string> StreamName
        {
            get
            {
                return from text in Identifier select text;
            }
        }

        #endregion

        /// <summary>
        /// A <see cref="DotSeparatedIdentifier"/> is an <see cref="Identifier"/> preceded immediately by a '.'
        /// </summary>
        internal virtual Parser<string> DotSeparatedIdentifier
        {
            get
            {
                return from dot in Parse.Char('.').Once()
                       from rest in Identifier.Once()
                       select dot.Single().ToString() + rest.Single();
            }
        }

        /// <summary>
        /// A <seealso cref="FullIdentifier"/> is a group of <see cref="Identifier"/> separated by a '.'
        /// </summary>
        /// <example>
        /// "Foo.Bar.Baz"
        /// </example>
        internal virtual Parser<string> FullIdentifier
        {
            get
            {
                return from start in Identifier.Once()
                       from rest in DotSeparatedIdentifier.Many().Optional()
                       select start.First() + string.Join(string.Empty, rest.IsDefined ? rest.Get()
                                : new string[] { });
            }
        }

        /// <summary>
        /// A <see cref="MessageType"/> is a field type declaration inside a Message
        /// </summary>
        /// <example>IdentA.IdentB.MessageName</example>
        internal virtual Parser<string> MessageType
        {
            get
            {
                return from optionalDot in Parse.Char('.').Once().Text().Optional()
                       from id in FullIdentifier.End()
                       select optionalDot.GetOrElse(string.Empty) + id;
            }
        }

        /// <summary>
        /// A <see cref="EnumType"/> is a field type declaration inside a Message
        /// </summary>
        /// <example>IdentA.IdentB.EnumType</example>
        internal virtual Parser<string> EnumType
        {
            get
            {
                return from optionalDot in Parse.Char('.').Once().Text().Optional()
                       from id in FullIdentifier.End()
                       select optionalDot.GetOrElse(string.Empty) + id;
            }
        }

        /// <summary>
        /// A <see cref="GroupName"/> is a field type declaration inside a Message
        /// </summary>
        internal virtual Parser<string> GroupName
        {
            get
            {
                return from groupName in CapitalizedIdentifier
                       select groupName;
            }
        }

        /// <summary>
        /// A <see cref="IntegerLiteral"/> is a literal integer string that may be integer, hex, or octal
        /// </summary>
        /// <example>54, 07, 0X5F</example>
        internal virtual Parser<string> IntegerLiteral
        {
            get
            {
                var normal = Parse.Digit.Many().End().Text();
                var octal = OctalDigit.Many().End().Text();
                var hex = HexLiteral;

                return normal.Or(octal.Or(hex));
            }
        }

        /// <summary>
        /// A <see cref="OctalLiteral"/> is a representation of an integer literal in octal notation. 
        /// </summary>
        /// <example>07</example>
        internal virtual Parser<string> OctalLiteral
        {
            get
            {
                return from octal in OctalDigit.Many().End().Text()
                       select octal;
            }
        }

        /// <summary>
        /// A <see cref="HexLiteral"/> is a representation of a Hexadecimal value
        /// </summary>
        /// <example>0X5F</example>
        internal virtual Parser<string> HexLiteral
        {
            get
            {
                return from leadingZero in Parse.Char('0').Once().Text()
                       from x in Parse.IgnoreCase('x').Once().Text()
                       from restDigits in HexDigit.AtLeastOnce().Text().End()
                       select leadingZero + x + restDigits;
            }
        }

        /// <summary>
        /// A <see cref="Exponent"/> is a representation of scientific exponential notation, and 
        /// must be the final component of a floating point literal. 
        /// </summary>
        /// <example>e+12 in '1.23e+12'</example>
        internal virtual Parser<string> Exponent
        {
            get
            {
                return from e in Parse.Char(t => (t == 'e' || t == 'E'), "Exponent Base").Once().Text()
                       from plusminus in Parse.Char(t => (t == '+' || t == '-'), "Plus or Minus").Optional()
                       from failonLetter in Parse.Letter.Not().Named("No Letters in Exponent")
                       from exponent in Parse.Digit.Many().Text()
                       select e + plusminus.GetOrElse('+').ToString() + exponent;

            }
        }

        /// <summary>
        /// A <see cref="FloatLiteral"/> is a literal floating point value string
        /// </summary>
        /// <example>12.345e-12</example>
        internal virtual Parser<string> FloatLiteral
        {
            get
            {
                return from dec in Parse.Decimal
                       from exp in Exponent.Optional()
                       from notrailer in Parse.AnyChar.Not()
                       select dec + exp.GetOrElse(string.Empty);
            }
        }

        /// <summary>
        /// A <see cref="BooleanLiteral"/> is a string which is 'true' or 'false', in lower case
        /// </summary>
        internal virtual Parser<string> BooleanLiteral
        {
            get
            {
                return from boolvalue in Parse.String("true").Or(Parse.String("false")).Text()
                       select boolvalue;
            }
        }

        /// <summary>
        /// A <see cref="EmptyStatement"/> is simply a semicolon ';'
        /// </summary>
        internal virtual Parser<string> EmptyStatement
        {
            get
            {
                return from single in Parse.Char(';')
                       select single.ToString();
            }
        }

        /// <summary>
        /// A <see cref="Quote"/> is a character which may be used to delimit string literals.
        /// </summary>
        internal virtual Parser<char> Quote
        {
            get
            {
                return from quote in Parse.Char('\'').Or(Parse.Char('"')).Once()
                    select quote.Single();
            }
        }

        /// <summary>
        /// A <see cref="HexEscape"/> is an escape sequence to use hex in string literals.
        /// </summary>
        internal virtual Parser<string> HexEscape
        {
            get
            {
                return from backslash in Parse.Char('\\').Once().Text()
                    from sep in Parse.IgnoreCase('x').Once().Text()
                    from hexone in HexDigit.Once().Text()
                    from hextwo in HexDigit.Once().Text()
                    select backslash + sep + hexone + hextwo;
            }
        }

        /// <summary>
        /// A <see cref="OctalEscape"/> is a.
        /// </summary>
        internal virtual Parser<string> OctalEscape
        {
            get
            {
                return from backslash in Parse.Char('\\').Once().Text()
                    from octone in OctalDigit.Once().Text()
                    from octtwo in OctalDigit.Once().Text()
                    from octtre in OctalDigit.Once().Text()
                    select backslash + octone + octtwo + octtre;
            }
        }

        /// <summary>
        /// A <see cref="CharEscape"/> is a.
        /// </summary>
        internal virtual Parser<string> CharEscape
        {
            get
            {
                return from backslash in Parse.Char('\\').Once().Text()
                    from escChar in Parse.Chars("abfnrtv")
                        .Or(Parse.Char('\''))
                        .Or(Parse.Char('\\'))
                        .Or(Parse.Char('\"'))
                        .Once().Text()
                    select backslash + escChar;

            }
        }

        /// <summary>
        /// A <see cref="CharValue"/> is a.
        /// </summary>
        internal virtual Parser<string> CharValue
        {
            get
            {
                return from ch in CharEscape.Or(HexEscape).Or(OctalEscape).Or(Parse.AnyChar.Except(Quote).Once().Text())
                       select ch;
            }
        }

        /// <summary>
        /// A <see cref="StringLiteral"/> is a.
        /// </summary>
        internal virtual Parser<string> StringLiteral
        {
            get
            {
                return from open in Quote
                    from literal in CharValue.Many()
                    from close in Quote.End()
                    select string.Join(string.Empty, literal);

            }
        }

        /// <summary>
        /// A <see cref="OptionName"/> is a.
        /// </summary>
        internal virtual Parser<string> OptionName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A <see cref="Syntax"/> is a.
        /// </summary>
        internal virtual Parser<Syntax> Syntax
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A <see cref="Import"/> is a.
        /// </summary>
        internal virtual Parser<Import> Import
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A <see cref="Package"/> is a.
        /// </summary>
        internal virtual Parser<Package> Package
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A <see cref="Option"/> is a.
        /// </summary>
        internal virtual Parser<Option> Option
        {
            get
            {
                throw new NotImplementedException();
            }
        }




    }
}
