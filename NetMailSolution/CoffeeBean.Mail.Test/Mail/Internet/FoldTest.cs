using System.IO;
using CoffeeBean.Mail.Extension;
using System.Text;
using System.Collections.Generic;
using CoffeeBean.Mail.Internet;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace CoffeeBean.Mail.Test.Mail.Internet
{
    [ExcludeFromCodeCoverage]
    public class FoldTest
    {
        private string direction;
        private string actuals;
        private string expected;

        public FoldTest(string direction, string actuals, string expected)
        {
            this.direction = direction;
            this.actuals = actuals;
            this.expected = expected;
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                using(Stream s = typeof(FoldTest).Assembly.GetManifestResourceStream("CoffeeBean.Mail.Test.folddata"))
                {
                    return parse(s);
                }
            }
        }

        private static IEnumerable<object[]> parse(Stream s)
        {
            var arrObject = new List<object[]>();
            using(StreamReader reader = new StreamReader(s))
            {
                string line;
                while((line = reader.ReadLine()).IsNotNull())
                {
                    if (line.StartsWith("#") || line.Length == 0) continue;
                    string orig = readString(reader);
                    if (line.Equals("BOTH"))
                        arrObject.Add(new object[] { line, orig, null });
                    else
                    {
                        string e = reader.ReadLine();
                        if(!e.Equals("EXPECT"))
                            throw new IOException("TEST DATA FORMAT ERROR");
                        string expect = readString(reader);
                        arrObject.Add(new object[] { line, orig, expect });
                    }
                }
            }
            return arrObject;
        }

        private static string readString(StreamReader r)
        {
            StringBuilder s = new StringBuilder();
            int c;
            while ((c = r.Read()) != '$')
                s.Append(c); // throw away rest of line
            r.ReadLine();
            return s.ToString();
        }

        [Theory, MemberData(nameof(Data))]
        public void TestFoldMethod(string line, string actual, string expect)
        {
            if (direction.Equals("BOTH"))
            {
                string fs = MimeUtility.Fold(0, actuals);
                string us = MimeUtility.Unfold(fs);
                Assert.Equal(actuals, us);
            }
            else if (direction.Equals("FOLD"))
            {
                Assert.Equal(expected, MimeUtility.Fold(0, actuals));
            }
            else if (direction.Equals("UNFOLD"))
            {
                Assert.Equal(expected, MimeUtility.Unfold(actuals));
            }
            else {
                //Assert.Throws("Unknown direction: " + direction);
            }
        }
    }
}
