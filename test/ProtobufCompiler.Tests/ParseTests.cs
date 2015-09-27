using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufCompiler.Tests
{
    [TestFixture]
    public class ParseTests
    {
        private ProtoGrammar _sys;

        [SetUp]
        public void Setup()
        {
            _sys = new ProtoGrammar();
        }

        // syntax = "syntax" "=" quote "proto3" quote ";"
        public string Parse_Syntax_Declaration(string input)
        {
            var syntax = _sys.Syntax.Parse(input);
            return syntax.Name;
        }

        // import = "import" [ "weak" | “public”] strLit ";" 
        public void Parse_Import_Declaration(string input, string expMod, string expValue)
        {
            var import = _sys.Import.Parse(input);
            Assert.That(import, Is.EqualTo(new Import(expMod, expValue)));
        }

        // package = "package" fullIdent ";"
        public string Parse_Package_Declaration(string input)
        {
            var package = _sys.Package.Parse(input);
            return package.Name;
        }

        // option = "option" optionName  "=" constant ";"
        // optionName = (ident | "(" fullIdent ")") {"." ident}
        public void Parse_Option_Declaration(string input, string expKey, string expValue)
        {
            var option = _sys.Option.Parse(input);
            Assert.That(option, Is.EqualTo(new Option(expKey, expValue)));
        }
    }
}
