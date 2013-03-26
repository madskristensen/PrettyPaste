using System;
using PasteR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PasteR.Test
{
    [TestClass]
    public class CleanTest
    {
        PasteCleaner _cleaner;

        [TestMethod, TestCategory("Clean")]
        public void CleanEmptyLines()
        {
            string raw = "public static string RemoveWhiteSpaceFromStylesheets(string body)\r\n\r\n{\r\n\r\n  body = Regex.Replace(body, @\"[a-zA-Z]+#\", \"#\");\r\n\r\n  body = Regex.Replace(body, @\"[\\n\\r]+\\s*\", string.Empty);\r\n\r\n  body = Regex.Replace(body, @\"\\s+\", \" \");\r\n\r\n  body = Regex.Replace(body, @\"\\s?([:,;{}])\\s?\", \"$1\");\r\n\r\n  body = body.Replace(\";}\", \"}\");\r\n\r\n  body = Regex.Replace(body, @\"([\\s:]0)(px|pt|%|em)\", \"$1\");\r\n\r\n \r\n\r\n  // Remove comments from CSS\r\n\r\n  body = Regex.Replace(body, @\"/\\*[\\d\\D]*?\\*/\", string.Empty);\r\n\r\n \r\n\r\n  return body;\r\n\r\n}\r\n";
            string expected = "public static string RemoveWhiteSpaceFromStylesheets(string body)\r\n{\r\n  body = Regex.Replace(body, @\"[a-zA-Z]+#\", \"#\");\r\n  body = Regex.Replace(body, @\"[\\n\\r]+\\s*\", string.Empty);\r\n  body = Regex.Replace(body, @\"\\s+\", \" \");\r\n  body = Regex.Replace(body, @\"\\s?([:,;{}])\\s?\", \"$1\");\r\n  body = body.Replace(\";}\", \"}\");\r\n  body = Regex.Replace(body, @\"([\\s:]0)(px|pt|%|em)\", \"$1\");\r\n \r\n  // Remove comments from CSS\r\n  body = Regex.Replace(body, @\"/\\*[\\d\\D]*?\\*/\", string.Empty);\r\n \r\n  return body;\r\n}";

            _cleaner = new PasteCleaner(raw);
            _cleaner.IsDirty();

            string actual = _cleaner.Clean();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("Clean")]
        public void LineNumberChrome()
        {
            string raw = "1\r\n<PropertyGroup>\r\n2\r\n  <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild>\r\n3\r\n</PropertyGroup>\r\n";
            string expected = "<PropertyGroup>\r\n  <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild>\r\n</PropertyGroup>";

            _cleaner = new PasteCleaner(raw);
            _cleaner.IsDirty();

            string actual = _cleaner.Clean();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("Clean")]
        public void LineNumberFirefox()
        {
            string raw = "1\t<PropertyGroup>\r\n2\t  <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild>\r\n3\t</PropertyGroup>";
            string expected = "<PropertyGroup>\r\n  <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild>\r\n</PropertyGroup>";

            _cleaner = new PasteCleaner(raw);
            _cleaner.IsDirty();

            string actual = _cleaner.Clean();
            Assert.AreEqual(expected, actual);
        }


    }
}
