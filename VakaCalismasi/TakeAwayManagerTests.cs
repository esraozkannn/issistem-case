using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace VakaCalismasi
{
    [TestClass]
    public class TakeAwayManagerTests
    {
        TakeAwayManager _takeAwayManager;

        public TakeAwayManagerTests()
        {
            _takeAwayManager = new TakeAwayManager();
        }

        [TestMethod]
        public void DoOperationTest()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            _takeAwayManager.DoOperation();
            var output = stringWriter.ToString();
            Assert.AreEqual("Bu işlem çok yakında hizmetinizde olacaktır. Lütfen başka işlem seçiniz." + "\r\n", output);
        }
    }
}
