using System;
using System.Collections.Generic;

namespace VakaCalismasi
{
    class OperationFactory
    {
        public static void DoOperation(int operationNumber, List<Table> dummyTableDataList, List<Reservation> dummyReservationDataList)
        {
            IEmailService emailService = new SmtpEmailService();
            IConsoleReadLine consoleReadLine = new ConsoleReadLine();
            switch (operationNumber)
            {
                case (int)Operations.Reservation:
                    var reservation = new ReservationManager(emailService, consoleReadLine);
                    reservation.DoOperation(dummyTableDataList, dummyReservationDataList);
                    break;
                case (int)Operations.OnlineOrder:
                    var onlineOrder = new OnlineOrderManager();
                    onlineOrder.DoOperation();
                    break;
                case (int)Operations.TakeAway:
                    var takeAway = new TakeAwayManager();
                    takeAway.DoOperation();
                    break;
                default:
                    Console.WriteLine("Girmiş olduğunuz numara için tanımlı işlem bulunamadı.");
                    break;
            }
        }
    }
}
