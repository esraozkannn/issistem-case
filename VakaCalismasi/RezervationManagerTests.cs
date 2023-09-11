using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VakaCalismasi
{
    [TestClass]
    public class RezervationManagerTests
    {
        private static IEmailService mockEmailService()
        {
            Mock<IEmailService> mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            return mockEmailService.Object;
        }

        private static IEnumerable<object[]> GetTestData
        {
            get
            {
                return new List<object[]>()
                {
                    new object[] {  new List<string> { "customerName", "esraozkan47", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "", "customerName", "esraozkan47", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "asdf123", "customerName", "esraozkan47", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "  ", "customerName", "esraozkan47", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "  ", "customerName", "", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "  ", "customerName", "  ", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "-1", "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "0", "2", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "-3", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "0", "3"}, "Rezervasyon başarıyla yapıldı", "1" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", "invalidDate", DateTime.Now.ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "2" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "invalidCapacity", "2", "3"}, "Rezervasyon başarıyla yapıldı", "3" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "2", "invalidReservationTimeSlotIndex", "3"}, "Rezervasyon başarıyla yapıldı", "4" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.AddDays(1).ToString(), "2", "3"}, "Rezervasyon başarıyla yapıldı", "5"},
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.AddDays(1).ToString(), "3", "3"}, "Üzgünüz, uygun masa bulunamadı", "6"},
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.AddDays(1).ToString(), "3", "5"}, "Üzgünüz, uygun masa bulunamadı", "7"},
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.AddDays(1).ToString(), "1", "5"}, "Rezervasyon başarıyla yapıldı", "8"},
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "1", "1"}, "Rezervasyon başarıyla yapıldı", "9" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.AddDays(1).ToString(), "1", "1"}, "Rezervasyon başarıyla yapıldı", "10" },
                    new object[] {  new List<string> { "customerName", "esraozkan47@gmail.com", DateTime.Now.ToString(), "3", "1"}, "Üzgünüz, uygun masa bulunamadı", "11" },
                };
            }
        }

        [DynamicData("GetTestData")]
        [TestMethod]
        public void DoOperationTest(List<string> inputs, string expectedOutput, string testCase)
        {
            List<Table> dummyTableDataList = new List<Table>
            {
                new Table() { Capacity = 2, Number = 1, ReservedTimes = new List<ReservedTimes>() { new ReservedTimes() { Date = DateTime.Now.Date, ResevationTimeSlotIndex = 1 } } },
                new Table() { Capacity = 2, Number = 2, ReservedTimes = new List<ReservedTimes>() { new ReservedTimes() { Date = DateTime.Now.Date.AddDays(1), ResevationTimeSlotIndex = 1 } } }
            };

            List<Reservation> dummyReservationDataList = new List<Reservation>
            {
                new Reservation() { CustomerName = "CustomerName", NumberOfGuests = 2, ReservationDate = DateTime.Now, ResevationTimeSlotIndex = 1, TableNumber = 1 },
                new Reservation() { CustomerName = "CustomerName", NumberOfGuests = 2, ReservationDate = DateTime.Now.AddDays(1), ResevationTimeSlotIndex = 1, TableNumber = 2 }
            };

            var mockConsoleReadLine = new MockConsoleReadLine(inputs);
            ReservationManager _reservationManager = new ReservationManager(mockEmailService(), mockConsoleReadLine);
            var output = new StringWriter();
            Console.SetOut(output);
            _reservationManager.DoOperation(dummyTableDataList, dummyReservationDataList);
            var outputs = output.ToString().Replace("\r\n", "").Split('.');
            Assert.AreEqual(expectedOutput, outputs[outputs.Length-2]);
            if(expectedOutput == "Rezervasyon başarıyla yapıldı")
            {
                Assert.IsTrue(dummyReservationDataList.Count == 3);
                var tableReservedTimesCount = dummyTableDataList.Where(x => x.Number == dummyReservationDataList[2].TableNumber).FirstOrDefault().ReservedTimes.Count();
                Assert.IsTrue(tableReservedTimesCount == 2);
            }
            else
            {
                Assert.IsTrue(dummyReservationDataList.Count == 2);
            }
        }
    }

    public class MockConsoleReadLine : IConsoleReadLine
    {
        private readonly Queue<string> inputQueue;
        public MockConsoleReadLine(IEnumerable<string> input)
        {
            inputQueue = new Queue<string>(input);
        }
        public string ReadLine()
        {
            if (inputQueue.Count > 0)
            {
                string input = inputQueue.Dequeue();
                return input;
            }
            return null;
        }
    }
}
