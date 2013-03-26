using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PasteR.Test
{
    [TestClass]
    public class IsDirtyTest
    {
        [TestMethod, TestCategory("IsDirty")]
        public void IsDirty1()
        {
            string raw = "public static string RemoveWhiteSpaceFromStylesheets(string body)\r\n\r\n{\r\n\r\n  body = Regex.Replace(body, @\"[a-zA-Z]+#\", \"#\");\r\n\r\n  body = Regex.Replace(body, @\"[\\n\\r]+\\s*\", string.Empty);\r\n\r\n  body = Regex.Replace(body, @\"\\s+\", \" \");\r\n\r\n  body = Regex.Replace(body, @\"\\s?([:,;{}])\\s?\", \"$1\");\r\n\r\n  body = body.Replace(\";}\", \"}\");\r\n\r\n  body = Regex.Replace(body, @\"([\\s:]0)(px|pt|%|em)\", \"$1\");\r\n\r\n \r\n\r\n  // Remove comments from CSS\r\n\r\n  body = Regex.Replace(body, @\"/\\*[\\d\\D]*?\\*/\", string.Empty);\r\n\r\n \r\n\r\n  return body;\r\n\r\n}\r\n";
            bool expected = true;
            bool actual = new PasteCleaner(raw).IsDirty();
                
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("IsDirty")]
        public void IsDirty2()
        {
            string raw = "1 <PropertyGroup> \r\n2   <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild> \r\n3 </PropertyGroup> \r\n";
            bool expected = true;
            bool actual = new PasteCleaner(raw).IsDirty();

            Assert.AreEqual(expected, actual);

        }

        [TestMethod, TestCategory("IsDirty")]
        public void IsDirty3()
        {
            string raw = "01 <PropertyGroup> \r\n02   <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild> \r\n03 </PropertyGroup> \r\n";
            bool expected = true;
            bool actual = new PasteCleaner(raw).IsDirty();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("IsDirty")]
        public void IsDirty4()
        {
            string raw = "01. <PropertyGroup> \r\n02.   <DeployOnBuild Condition=\" '$(DeployProjA)'!='' \">$(DeployProjA)</DeployOnBuild> \r\n03. </PropertyGroup> \r\n";
            bool expected = true;
            bool actual = new PasteCleaner(raw).IsDirty();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("IsDirty")]
        public void IsDirty5()
        {
            string raw = "        [Authorize]\r\n        public ActionResult Edit(int id)\r\n";
            bool expected = false;
            bool actual = new PasteCleaner(raw).IsDirty();
            Assert.AreEqual(expected, actual);
        }
    }
}
