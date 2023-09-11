using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace VakaCalismasi
{
    [TestClass]
    public class OnlineOrderManagerTests
    {
        OnlineOrderManager _onlineOrderManager;

        public OnlineOrderManagerTests()
        {
            _onlineOrderManager = new OnlineOrderManager();
        }

        [TestMethod]
        public void DoOperationTest()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            _onlineOrderManager.DoOperation();
            var output = stringWriter.ToString();
            Assert.AreEqual("Bu işlem çok yakında hizmetinizde olacaktır. Lütfen başka işlem seçiniz." + "\r\n", output);
        }
    }
}
