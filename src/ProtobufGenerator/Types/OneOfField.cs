using System.Collections.Generic;

namespace ProtobufGenerator.Types
{
    public class OneOfField : Field
    {
        public OneOfField(string type, string name, int fieldNum, IEnumerable<Option> fieldOptions) :
            base(type, name, fieldNum, fieldOptions, false)
        {
            
        }
    }
}
