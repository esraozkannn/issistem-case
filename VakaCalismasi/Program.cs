using System;
using System.Collections.Generic;

namespace VakaCalismasi
{
    public class Program
    {
        static readonly List<Table> dummyTableDataList = new();
        static readonly List<Reservation> dummyReservationDataList = new();
        static void Main(string[] args)
        {
            CreateDummyTableData();
            CreateDummyReservationData();

            ChooseOperation();
        }

        /// <summary>
        /// Creates dummy reservation data
        /// </summary>
        private static void CreateDummyReservationData()
        {
            SetReservationData("Onur B", 2, DateTime.Now.Date, 1, 2);
            SetReservationData("Ezran B", 2, DateTime.Now.Date, 1, 3);
            SetReservationData("Esra B", 4, DateTime.Now.AddDays(1).Date, 4, 3);
        }

        /// <summary>
        /// Sets reservation data
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="numberOfGuests"></param>
        /// <param name="reservationDate"></param>
        /// <param name="tableNumber"></param>
        /// <param name="resevationTimeSlotIndex"></param>
        private static void SetReservationData(string customerName, int numberOfGuests, DateTime reservationDate, int tableNumber, int resevationTimeSlotIndex)
        {
            Reservation reservation = new();
            reservation.CustomerName = customerName;
            reservation.NumberOfGuests = numberOfGuests;
            reservation.ReservationDate = reservationDate;
            reservation.TableNumber = tableNumber;
            reservation.ResevationTimeSlotIndex = resevationTimeSlotIndex;
            dummyReservationDataList.Add(reservation);
        }

        /// <summary>
        /// Creates dummy table data
        /// </summary>
        private static void CreateDummyTableData()
        {
            SetTableData(1, 4, new List<DateTime> { DateTime.Now.Date, DateTime.Now.Date }, new List<int> { 2, 3 });
            SetTableData(2, 4, new List<DateTime>(), new List<int>());
            SetTableData(3, 6, new List<DateTime>(), new List<int>());
            SetTableData(4, 6, new List<DateTime> { DateTime.Now.AddDays(1).Date }, new List<int> { 3 });
        }

        /// <summary>
        /// Sets table data
        /// </summary>
        /// <param name="number"></param>
        /// <param name="capacity"></param>
        private static void SetTableData(int number, int capacity, List<DateTime> dates, List<int> indexes)
        {
            Table table = new();
            table.Number = number;
            table.Capacity = capacity;
            for (int i = 0; i < dates.Count; i++)
            {
                ReservedTimes reservedTimes = new();
                reservedTimes.Date = dates[i];
                reservedTimes.ResevationTimeSlotIndex = indexes[i];
                table.ReservedTimes.Add(reservedTimes);
            }

            dummyTableDataList.Add(table);
        }

        /// <summary>
        /// Chooses the operation
        /// </summary>
        public static void ChooseOperation()
        {
            Console.WriteLine("1 - Rezervasyon");
            Console.WriteLine("2 - Online Sipariş");
            Console.WriteLine("3 - Al/Götür");
            Console.WriteLine("Lütfen işlem numarasını giriniz.");

            string operationNumber = Console.ReadLine();
            var isNumeric = int.TryParse(operationNumber, out int number);

            if (!isNumeric)
            {
                Console.WriteLine("Lütfen geçerli bir işlem numarası giriniz." + "\r\n\r\n");
                ChooseOperation();
            }
            OperationFactory.DoOperation(number, dummyTableDataList, dummyReservationDataList);

            Console.WriteLine("\r\n\r\n" + "Başka işlem yapmak istiyor musunuz?" + "\n");
            ChooseOperation();

            Console.ReadKey();
        }
    }

    public enum Operations
    {
        Reservation = 1,
        OnlineOrder = 2,
        TakeAway = 3
    }
}
