using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace VakaCalismasi
{
    public class ReservationManager : IOperation
    {
        readonly IEmailService _emailService;
        readonly IConsoleReadLine _consoleReadLine;
        readonly List<ReservationTimeSlots> reservationTimeSlotsList = new();
        DateTime reservationDate;
        int selectedIndex;

        /// <summary>
        /// Making the reservation operation with taking information from user.
        /// </summary>
        /// <param name="emailService"></param>
        /// <param name="consoleReadLine"></param>
        public ReservationManager(IEmailService emailService, IConsoleReadLine consoleReadLine)
        {
            _emailService = emailService;
            _consoleReadLine = consoleReadLine;
            CreateReservationTimeSlotsData();
        }

        /// <summary>
        /// Making the reservation operation with taking information from user.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="date"></param>
        /// <param name="guests"></param>
        /// <param name="dummyTableDataList"></param>
        /// <param name="dummyReservationDataList"></param>
        public void MakeReservation(string name, string email, DateTime date, int guests, List<Table> dummyTableDataList, List<Reservation> dummyReservationDataList)
        {
            ShowReservationTimeSlots();
            var tables = GetTables(date, guests, dummyTableDataList);
            if (tables == null || tables.Count == 0)
            {
                Console.WriteLine("Üzgünüz, uygun masa bulunamadı.");
                return;
            }

            var table = tables[0];
            var reservation = new Reservation
            {
                CustomerName = name,
                ReservationDate = date,
                NumberOfGuests = guests,
                TableNumber = table.Number,
                ResevationTimeSlotIndex = selectedIndex
            };

            SaveTableReservedTimes(table.Number, date, dummyTableDataList);
            SaveReservation(reservation, dummyReservationDataList);
            SendMail(name, email, date, guests, table);
            Console.WriteLine("Rezervasyon başarıyla yapıldı.");
        }

        /// <summary>
        /// Sends e-mail when reservation completed.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="date"></param>
        /// <param name="guests"></param>
        /// <param name="table"></param>
        private void SendMail(string name, string email, DateTime date, int guests, Table table)
        {
            var selectedTimeSlot = reservationTimeSlotsList.FirstOrDefault(x => x.Index == selectedIndex);
            var body = $"Sayın {name}, rezervasyonunuz başarıyla alındı. Masa No: {table.Number}, Tarih: {date.ToString("dd.MM.yyyy")} {selectedTimeSlot.StartTime} - {selectedTimeSlot.EndTime}, Kişi Sayısı: {guests}";
            _emailService.SendEmail(email, "Rezervasyon Onayı", body);
        }

        /// <summary>
        /// Saves new created reservation to the associated table 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="date"></param>
        /// <param name="dummyTableDataList"></param>
        private void SaveTableReservedTimes(int number, DateTime date, List<Table> dummyTableDataList)
        {
            var selectedTable = dummyTableDataList.Where(x => x.Number == number).FirstOrDefault();
            int listIndex = dummyTableDataList.IndexOf(selectedTable);
            dummyTableDataList[listIndex].ReservedTimes.Add(new ReservedTimes() { Date = date, ResevationTimeSlotIndex = selectedIndex });
        }

        /// <summary>
        /// Returns available tables by user input
        /// </summary>
        /// <param name="date"></param>
        /// <param name="guests"></param>
        /// <param name="dummyTableDataList"></param>
        /// <returns></returns>
        public List<Table> GetTables(DateTime date, int guests, List<Table> dummyTableDataList)
        {
            var availableTables = dummyTableDataList.Where(x => x.Capacity >= guests && (!x.ReservedTimes.Any(x => x.Date == date && x.ResevationTimeSlotIndex == selectedIndex) || x.ReservedTimes.Count == 0))
                                                    .OrderBy(x => x.Capacity)
                                                    .ToList();
            return availableTables;
        }

        /// <summary>
        /// Shows reservation time slots
        /// </summary>
        private void ShowReservationTimeSlots()
        {
            for (int i = 0; i < reservationTimeSlotsList.Count; i++)
            {
                Console.WriteLine(i + 1 + ") " + reservationTimeSlotsList[i].StartTime + " - " + reservationTimeSlotsList[i].EndTime);
            }

            Console.WriteLine("Lütfen rezervasyon yapmak istediğiniz saat aralığı numarasını giriniz.");
            string selectedTimeSlotNumber = _consoleReadLine.ReadLine();
            if (!int.TryParse(selectedTimeSlotNumber, out int index) || index < 1)
            {
                Console.WriteLine("Lütfen geçerli bir saat aralığı numarası giriniz.");
                ShowReservationTimeSlots();
            }
            else
            {
                selectedIndex = index;
            }
        }

        /// <summary>
        /// Saves revervation
        /// </summary>
        /// <param name="reservation"></param>
        public void SaveReservation(Reservation reservation, List<Reservation> dummyReservationDataList)
        {
            dummyReservationDataList.Add(reservation);

        }

        /// <summary>
        /// Main function of the reservation
        /// </summary>
        /// <param name="dummyTableDataList"></param>
        /// <param name="dummyReservationDataList"></param>
        public void DoOperation(List<Table> dummyTableDataList, List<Reservation> dummyReservationDataList)
        {
            string nameSurname, email;
            int guests;

            GetInformationFromInput(out nameSurname, out email, out guests);
            MakeReservation(nameSurname, email, reservationDate, guests, dummyTableDataList, dummyReservationDataList);
        }

        /// <summary>
        /// Takes the validated information from user 
        /// </summary>
        /// <param name="nameSurname"></param>
        /// <param name="email"></param>
        /// <param name="guests"></param>
        private void GetInformationFromInput(out string nameSurname, out string email, out int guests)
        {
            Console.WriteLine("Lütfen rezervasyon için sırasıyla aşağıdaki bilgileri giriniz");
            nameSurname = NameSurnameChecker();
            email = EmailChecker();
            ReservationDateChecker();
            guests = GuestsChecker();
        }

        /// <summary>
        /// Validates the name surname with empty and include number.
        /// </summary>
        /// <returns></returns>
        public string NameSurnameChecker()
        {
            Console.Write("Ad Soyad: ");
            string name = _consoleReadLine.ReadLine();
            if(string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || name.Any(char.IsDigit))
            {
                Console.WriteLine("Geçerli ad soyad bilgisi giriniz.");
                return NameSurnameChecker();
            }
            return name;
        }



        /// <summary>
        /// Validates the email with MailAddress class
        /// </summary>
        /// <returns></returns>
        private string EmailChecker()
        {
            Console.Write("E-Mail: ");
            string mailAdress = string.Empty;
            try
            {
                string email = _consoleReadLine.ReadLine();
                if(string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Geçerli email adresi giriniz.");
                    return EmailChecker();
                }
                else
                {
                    MailAddress m = new MailAddress(email);
                    mailAdress = m.Address;
                }
               
            }
            catch (FormatException)
            {
                Console.WriteLine("Geçerli email adresi giriniz.");
                return EmailChecker();
            }
            return mailAdress;
        }

        /// <summary>
        /// Validates the guest input by checking the input type if integer
        /// </summary>
        /// <returns></returns>
        public int GuestsChecker()
        {
            Console.Write("Misafir Sayısı: ");
            if (!int.TryParse(_consoleReadLine.ReadLine(), out int guest) || guest < 1)
            {
                Console.WriteLine("Geçerli misafir sayısı giriniz");
                GuestsChecker();
            }
            return guest;
        }

        /// <summary>
        /// Validates the date input by checking the input type if dateTime
        /// </summary>
        /// <returns></returns>
        public void ReservationDateChecker()
        {
            Console.Write("Rezervasyon Tarihi: ");
            string dateStr = _consoleReadLine.ReadLine();
            if (!DateTime.TryParse(dateStr, out DateTime date) || date < DateTime.Now.Date)
            {
                Console.WriteLine("Geçerli bir tarih giriniz");
                ReservationDateChecker();
            }
            else
            {
                reservationDate = date.Date;
            }
        }

        /// <summary>
        /// Creates reservation time slots data
        /// </summary>
        private void CreateReservationTimeSlotsData()
        {
            SetReservationTimeSlotsData(1, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0));
            SetReservationTimeSlotsData(2, new TimeSpan(11, 0, 0), new TimeSpan(13, 0, 0));
            SetReservationTimeSlotsData(3, new TimeSpan(13, 0, 0), new TimeSpan(15, 0, 0));
            SetReservationTimeSlotsData(4, new TimeSpan(15, 0, 0), new TimeSpan(17, 0, 0));
            SetReservationTimeSlotsData(5, new TimeSpan(17, 0, 0), new TimeSpan(19, 0, 0));
            SetReservationTimeSlotsData(6, new TimeSpan(19, 0, 0), new TimeSpan(21, 0, 0));
            SetReservationTimeSlotsData(7, new TimeSpan(21, 0, 0), new TimeSpan(23, 0, 0));
        }

        /// <summary>
        /// Sets reservation time slots data
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        private void SetReservationTimeSlotsData(int index, TimeSpan startTime, TimeSpan endTime)
        {
            ReservationTimeSlots reservationTimeSlots = new();
            reservationTimeSlots.Index = index;
            reservationTimeSlots.StartTime = startTime;
            reservationTimeSlots.EndTime = endTime;
            reservationTimeSlotsList.Add(reservationTimeSlots);
        }

        public void DoOperation()
        {
            throw new NotImplementedException();
        }
    }

    public class Reservation
    {
        public string CustomerName { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfGuests { get; set; }
        public int TableNumber { get; set; }
        public int ResevationTimeSlotIndex { get; set; }
    }

    public class Table
    {
        public int Number { get; set; }
        public int Capacity { get; set; }
        public List<ReservedTimes> ReservedTimes { get; set; } = new List<ReservedTimes>();
    }

    public class ReservationTimeSlots
    {
        public int Index { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class ReservedTimes
    {
        public int ResevationTimeSlotIndex { get; set; }
        public DateTime Date { get; set; }
    }
}