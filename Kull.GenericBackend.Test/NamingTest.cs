using Kull.GenericBackend.Common;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;

namespace Kull.GenericBackend.Test
{
    [TestClass]
    public class NamingTest
    {
        [TestMethod]
        public void TestByIdAndSecond()
        {
            string url = "/Cases/{CaseId:int}/Brand";
            string expected = "CaseBrand";
            TestNaming(url, expected);
        }

        private static void TestNaming(string url, string expected)
        {
            //MPa added implicit conversion
            Method method = new HttpMethod("Get");
            var ent = new Entity(url, new Dictionary<HttpMethod, Method>()
            {
                { HttpMethod.Get, method }
            });
            var conv = new SwaggerGeneration.CodeConvention();
            var displayString = conv.GetTag(ent, method);
            Assert.AreEqual(expected, displayString);
        }

        [TestMethod]
        public void TestById()
        {
            TestNaming("/Cases/{CaseId:int}", "Case");
        }

        [TestMethod]
        public void TestResource()
        {
            TestNaming("/Cases", "Cases");
        }
    }
}
