using CoffeeBean.Mail.Extension;
using System;
using System.IO;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// This abstract class is the superclass of all classes representing various format of bytes.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
	public abstract class OutputStream : IDisposable
	{
		protected volatile Stream s;
		
		public OutputStream(Stream s) { this.s = s; }
		
		public abstract void Write(int b);
		
		public virtual void Write(byte[] b) { Write(b, 0, b.Length); }
		
		public virtual void Write(byte[] buff, int off, int len)
		{
			buff.ThrowIfNull(new ArgumentNullException());
			if((off < 0) || (off > buff.Length) || (len < 0) ||
				((off + len) > buff.Length) || (off + len) < 0)
				throw new IndexOutOfRangeException();
			else if(len == 0) return;
			
			for(int i = 0; i < len; i++)
				Write(buff[off + i]);
		}
		
		public virtual void Flush() { }
		
		public virtual void Close() { }

        protected virtual void dispose(bool disposable)
        {
            if (s.IsNotNull())
            {
                s.Close();
                if (disposable) s.Dispose();
            }
            s = null;
        }

        public void Dispose()
        {
            Close();
            dispose(true);
            GC.SuppressFinalize(this);
        }

        ~OutputStream()
        {
            dispose(false);
        }
    }
}