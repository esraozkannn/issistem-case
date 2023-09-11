using System.Collections.Generic;

namespace VakaCalismasi
{
    public interface IOperation
    {
        void DoOperation();
        void DoOperation(List<Table> dummyTableDataList, List<Reservation> dummyReservationDataList);
    }
}
