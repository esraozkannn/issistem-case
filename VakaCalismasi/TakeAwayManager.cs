using System;
using System.Collections.Generic;

namespace VakaCalismasi
{
    public class TakeAwayManager : IOperation
    {
        public void DoOperation()
        {
            Console.WriteLine("Bu işlem çok yakında hizmetinizde olacaktır. Lütfen başka işlem seçiniz.");
        }

        public void DoOperation(List<Table> dummyTableDataList, List<Reservation> dummyReservationDataList)
        {
            throw new NotImplementedException();
        }
    }
}
