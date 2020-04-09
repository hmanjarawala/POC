using System.Text;
using System.IO;
using CoffeeBean.Mail.Util;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace CoffeeBean.Mail.Test.Mail.Util
{
    /// <summary>
    /// Summary description for QPEncoderTest
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class QPEncoderTest
    {
        [Fact]
        public void TestTrailingSpace()
        {
            MemoryStream ms = new MemoryStream();
            QPEncoderStream qs = new QPEncoderStream(ms);
            qs.Write(Encoding.ASCII.GetBytes("test "));
            qs.Flush();
            string result = Encoding.ASCII.GetString(ms.ToArray());
            Assert.Equal("test=20", result);
        }
    }
}
