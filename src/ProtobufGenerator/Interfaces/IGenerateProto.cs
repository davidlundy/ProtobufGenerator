using ProtobufGenerator.JobParameters;
using System;

namespace ProtobufGenerator.Interfaces
{
    public interface IGenerateProto
    {
        /// <summary>
        /// Process a collection of Jobs given a file path to an XML defining a Parameters definition object
        /// </summary>
        /// <param name="path">File path to an Parameters XML document</param>
        void LoadParameters(string path);

        /// <summary>
        /// Process a collection of Jobs loaded from an XML document. 
        /// </summary>
        void ProcessProto();
        
        /// <summary>
        /// Process a collection of Jobs given a Parameters definition object
        /// </summary>
        /// <param name="parameters">Parameters definition object</param>
        void ProcessProto(Parameters parameters);
    }
}
