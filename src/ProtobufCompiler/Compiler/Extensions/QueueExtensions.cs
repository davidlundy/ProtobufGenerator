using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtobufCompiler.Compiler.Extensions
{
    public static class QueueExtensions
    {
        /// <summary>
        /// Skip any and all lines that are only a <see cref="TokenType.EndLine"/> token.
        /// </summary>
        /// <param name="queue">The <see cref="Queue{Token}"/></param>
        internal static void SkipEmptyLines(this Queue<Token> queue)
        {
            while (queue.Any() && queue.Peek().Type.Equals(TokenType.EndLine))
            {
                queue.Dequeue();
            }
        }

        /// <summary>
        /// After all meaningful syntax has been processed for a statement, dump any trailing <see cref="TokenType.EndLine"/> tokens.
        /// </summary>
        /// <param name="queue">The <see cref="Queue{Token}"/></param>
        internal static void DumpEndline(this Queue<Token> queue)
        {
            // Empty queues have no endlines.
            if (!queue.Any()) return; 

            if (queue.Peek().Type.Equals(TokenType.EndLine)) queue.Dequeue(); 
        }

        /// <summary>
        /// Wherever we are in parsing a line of tokens, dequeue tokens until and including the <see cref="TokenType.EndLine"/>.
        /// </summary>
        /// <param name="queue">The <see cref="Queue{Token}"/></param>
        internal static void BurnLine(this Queue<Token> queue)
        {
            Token token;
            do
            {
                if (!queue.Any()) return;
                token = queue.Dequeue();
            } while (!token.Type.Equals(TokenType.EndLine));
        }

        /// <summary>
        /// Dequeue elements of type T from the <see cref="Queue{T}"/> while predicate is true.
        /// <para><see cref="DequeueWhile{T}(Queue{T}, Func{T, bool})"/> has internal checks for available entries so it is not necessary to include a check to Any() in the predicate.</para>
        /// </summary>
        /// <typeparam name="T">The type stored in the <see cref="Queue{T}"/></typeparam>
        /// <param name="queue">The <see cref="Queue{T}"/></param>
        /// <param name="predicate">The predicate for evaluating the queue.</param>
        /// <returns>An <see cref="IList{T}"/>, does not support lazy enumeration. Evaluated when called.</returns>
        internal static IList<T> DequeueWhile<T>(this Queue<T> queue, Func<T, bool> predicate)
        {
            if (ReferenceEquals(queue, null)) throw new ArgumentNullException(nameof(queue));
            if (ReferenceEquals(predicate, null)) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();

            while(queue.Any() && predicate(queue.Peek()))
            {
                result.Add(queue.Dequeue());
            }

            return result;
        }

        /// <summary>
        /// Wherever we are in parsing a line of tokens, extract the remaining tokens until the <see cref="TokenType.EndLine"/> as a string of text. 
        /// </summary>
        /// <param name="queue">The <see cref="Queue{Token}"/></param>
        /// <returns>The remaining tokens as a string.</returns>
        internal static string DumpStringToEndline(this Queue<Token> queue)
        {
            var stringTokens = queue.DequeueWhile(t => !t.Type.Equals(TokenType.EndLine))
                .Select(t => t.Lexeme);

            queue.DumpEndline();

            return string.Join(" ", stringTokens);
        }

    }
}


