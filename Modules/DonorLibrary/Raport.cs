using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1
{
    public class Raport
    {
        private static int RaportCounter = 0;

        public int RaportID { get; private set; }

        public Raport()
        {
            RaportID = ++RaportCounter;
        }

        public string GenerateReport(int donorID, List<Donation> donations)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"Raport ID: {RaportID}");
            report.AppendLine($"ID Darczyńcy: {donorID}");
            report.AppendLine("Darowizny:");

            foreach (var donation in donations)
            {
                report.AppendLine($"- ID Darowizny: {donation.DonationID}, Status: {donation.DonationStatus}, Przedmiot: {donation.Item}, Ilosc: {donation.Quantity}, Data: {donation.Date}");
            }

            return report.ToString();
        }
    }
}
