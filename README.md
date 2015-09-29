# ProtobufGenerator

Configurable tooling that can process .proto definition files into C# class files compatible with [Protobuf-Net]. 

### Project Goals

- Support the [Proto3 Language Specification] and feature set.
- Support cross platform development with DNXCore 5.0 and .NET on Mono.
- Provide fully configurable class generation into defined namespace and file path with custom annotations and class level options. 

### Project Structure

- ProtobufCompiler : Handles parsing .proto definitions into an object representing a C# class definition. **Experimental**
- ProtobufGenerator : Handles generation of C# class (.cs) files given some defined job parameters and a compiled representation of the .proto definition files. Salvaged from working implementation of ProtobufGenerator which used embedded protoc compiler.  **Mostly Stable**

[Protobuf-Net]: <https://github.com/mgravell/protobuf-net>
[Proto3 Language Specification]: <https://developers.google.com/protocol-buffers/docs/reference/proto3-spec>
