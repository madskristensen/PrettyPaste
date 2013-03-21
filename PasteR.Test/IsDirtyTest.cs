using System;
using PasteR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PasteR.Test
{
    [TestClass]
    public class IsDirtyTest
    {
        [TestMethod]
        public void IsDirty1()
        {
            string raw = "public static string RemoveWhiteSpaceFromStylesheets(string body)\r\n\r\n{\r\n\r\n  body = Regex.Replace(body, @\"[a-zA-Z]+#\", \"#\");\r\n\r\n  body = Regex.Replace(body, @\"[\\n\\r]+\\s*\", string.Empty);\r\n\r\n  body = Regex.Replace(body, @\"\\s+\", \" \");\r\n\r\n  body = Regex.Replace(body, @\"\\s?([:,;{}])\\s?\", \"$1\");\r\n\r\n  body = body.Replace(\";}\", \"}\");\r\n\r\n  body = Regex.Replace(body, @\"([\\s:]0)(px|pt|%|em)\", \"$1\");\r\n\r\n \r\n\r\n  // Remove comments from CSS\r\n\r\n  body = Regex.Replace(body, @\"/\\*[\\d\\D]*?\\*/\", string.Empty);\r\n\r\n \r\n\r\n  return body;\r\n\r\n}\r\n";
            bool expected = true;
            bool actual = PasteCleaner.IsDirty(raw);
                
            Assert.AreEqual(expected, actual);
        }
    }
}
