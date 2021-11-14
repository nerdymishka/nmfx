using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.IO
{
    public class TeeTextWriter : TextWriter
    {
        private readonly TextWriter outWriter;

        private readonly TextWriter writer;

        private readonly bool dispose;

        public TeeTextWriter()
            : this(new StringWriter(), true)
        {
        }

        public TeeTextWriter(TextWriter writer)
            : this(writer, Console.Out, false)
        {
        }

        public TeeTextWriter(TextWriter writer, bool dispose)
            : this(writer, Console.Out, dispose)
        {
        }

        public TeeTextWriter(TextWriter writer, TextWriter outWriter)
            : this(writer, outWriter, false)
        {
        }

        public TeeTextWriter(TextWriter writer, TextWriter outWriter, bool dispose)
        {
            this.dispose = dispose;
            this.outWriter = outWriter;
            this.writer = writer;
        }

        public override Encoding Encoding => this.writer.Encoding;

        public override void Flush()
        {
            this.outWriter.Flush();
            this.writer.Flush();

            base.Flush();
        }

        public override Task FlushAsync()
        {
            return Task.WhenAll(
                this.writer.FlushAsync(),
                this.outWriter.FlushAsync());
        }

        public override void Close()
        {
            this.writer.Close();
        }

        public override void Write(bool value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(char value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            this.outWriter.Write(buffer);
            this.writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this.outWriter.Write(buffer, index, count);
            this.writer.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(double value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(int value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(long value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(object value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(float value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(string value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            this.outWriter.Write(format, arg0);
            this.writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            this.outWriter.Write(format, arg0, arg1);
            this.writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            this.outWriter.Write(format, arg0, arg1, arg2);
            this.writer.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            this.outWriter.Write(format, arg);
            this.writer.Write(format, arg);
        }

        public override void Write(uint value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override void Write(ulong value)
        {
            this.outWriter.Write(value);
            this.writer.Write(value);
        }

        public override Task WriteAsync(char value)
        {
            return Task.WhenAll(
                this.outWriter.WriteAsync(value),
                this.writer.WriteAsync(value));
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Task.WhenAll(
                this.writer.WriteAsync(buffer, index, count),
                this.outWriter.WriteAsync(buffer, index, count));
        }

        public override Task WriteAsync(string value)
        {
            return Task.WhenAll(
                this.writer.WriteAsync(value),
                this.outWriter.WriteAsync(value));
        }

        public override void WriteLine()
        {
            this.writer.WriteLine();
            this.outWriter.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            this.writer.WriteLine(buffer);
            this.outWriter.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.writer.WriteLine(buffer, index, count);
            this.outWriter.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            this.writer.WriteLine(format, arg0);
            this.outWriter.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            this.writer.WriteLine(format, arg0, arg1);
            this.outWriter.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            this.writer.WriteLine(format, arg0, arg1, arg2);
            this.writer.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            this.writer.WriteLine(format, arg);
            this.writer.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            this.writer.WriteLine(value);
            this.outWriter.WriteLine(value);
        }

        public override Task WriteLineAsync()
        {
            return Task.WhenAll(
                this.writer.WriteLineAsync(),
                this.outWriter.WriteLineAsync());
        }

        public override Task WriteLineAsync(char value)
        {
            return Task.WhenAll(
                this.writer.WriteLineAsync(value),
                this.outWriter.WriteLineAsync(value));
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Task.WhenAll(
                this.writer.WriteLineAsync(buffer, index, count),
                this.outWriter.WriteLineAsync(buffer, index, count));
        }

        public override Task WriteLineAsync(string value)
        {
            return Task.WhenAll(
                this.writer.WriteLineAsync(value),
                this.outWriter.WriteLineAsync(value));
        }

        public override int GetHashCode()
        {
            return this.writer.GetHashCode() ^ this.outWriter.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is TeeTextWriter tee)
                return tee.writer == this.writer && tee.outWriter == this.outWriter;

            return false;
        }

        public override string ToString()
        {
            if (this.writer is StringWriter sw)
                return sw.ToString();

            return base.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (this.dispose)
                this.writer.Dispose();
        }
    }
}
