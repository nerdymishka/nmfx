using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NerdyMishka.Collections.Generic
{
    public class LinesEnumerable : IEnumerable<string>
    {
        private readonly TextReader reader;

        private int consumed;

        public LinesEnumerable(string value)
            : this(new StringReader(value))
        {
        }

        public LinesEnumerable(TextReader reader)
        {
            this.reader = reader;
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (Interlocked.Exchange(ref this.consumed, 1) != 0)
                throw new InvalidOperationException("The enumerable returned by GetLines() can only be enumerated once");

            string? line;
            while ((line = this.reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
