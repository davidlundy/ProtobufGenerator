﻿using System;
using System.IO;
using ProtobufCompiler.Interfaces;

namespace ProtobufCompiler.Compiler
{
    internal class Source : ISource
    {
        private readonly StreamReader _streamReader;

        public int Column { get; private set; }
        public int Line { get; private set; }

        public bool EndStream => _streamReader.EndOfStream;

        private const char LineFeed = '\n';
        private const char CarriageReturn = '\r';
        private const int PeekEndFile = -1;

        internal Source(StreamReader streamReader)
        {
            if(streamReader == null) throw new ArgumentNullException(nameof(streamReader));
            _streamReader = streamReader;
            Line = 1;
            Column = 0;
        }

        public char Next()
        {
            if(EndStream) throw new EndOfStreamException("End of Stream Reached.");

            var singleBuffer = new char[1];
            _streamReader.Read(singleBuffer, 0, 1);

            var next = singleBuffer[0];

            // Peek returns -1 at EoF, and we can't Convert or cast this to char. 
            var peek = _streamReader.Peek();
            var lookahead = (peek.Equals(PeekEndFile)) ? ' ' : Convert.ToChar(peek);

            Column++;

            // Windows - Dump the extraneous LineFeed
            if (next.Equals(CarriageReturn) && lookahead.Equals(LineFeed)) _streamReader.Read();

            // All Platforms -> Increase the Line Count and reset Column to 1
            if (next.Equals(CarriageReturn) /* Mac or Win */ || next.Equals(LineFeed) /* Unix */)
            {
                Line++;
                // Next call to 'Next' will have first character in the line returned as Column 1
                Column = 0; 
            }

            return next;
        }
    }
}