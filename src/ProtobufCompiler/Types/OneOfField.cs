using System.Collections.Generic;

namespace ProtobufCompiler.Types
{
    internal class OneOfField : Field
    {
        public OneOfField(string type, string name, int fieldNum, IEnumerable<Option> fieldOptions) :
            base(type, name, fieldNum, fieldOptions, false)
        {
            
        }
    }
}
