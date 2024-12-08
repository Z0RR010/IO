using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1;

namespace DonorClass
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var donorManager = new DonorManager();
                var donationManager = new DonationManager();
                var donor1 = new Donor(1, new Individual("Anna", "Kowalska", "123 Street", "555-1234", "90010112345"));
                var donor2 = new Donor(2, new Company("Tech Solutions", "456 Business Rd", "555-5678", "1234567890"));

                donorManager.RegisterDonor(donor1);
                donorManager.RegisterDonor(donor2);

                Console.WriteLine("Darczyńcy zarejestrowani:");
                Console.WriteLine(donor1);
                Console.WriteLine(donor2);
                if (donorManager.Login(1))
                {
                    Console.WriteLine("\nDarczyńca zalogowany:");
                    Console.WriteLine($"Zalogowany: {donorManager.GetCurrentDonor()}");

                    donor1.Donate(101, "żywność", 50, "12.11.2023", donationManager);
                    donor1.Donate(102, "odzież", 20, "12.11.2023", donationManager); 
                    Console.WriteLine("\nDarowizny zostały dodane:");
                    foreach (var donation in donorManager.GetCurrentDonor().donations)
                    {
                        Console.WriteLine(donation);
                    }
                    donationManager.GetDonationById(101).SetDonationStatus(Status.Accepted);

                    Console.WriteLine("\nDarowizny darczyńcy 1 po zmianie statusu:");
                    foreach (var donation in donationManager.GetDonationsByDonor(donor1))
                    {
                        Console.WriteLine(donation);
                    }
                    Console.WriteLine();
                    donorManager.GenerateDonorReport(1);
                }

                donorManager.Logout();
                Console.WriteLine("\nDarczyńca wylogowany.");
                Console.WriteLine(donor1);
                if (donorManager.Login(2))
                {
                    Console.WriteLine("\nDarczyńca zalogowany:");
                    Console.WriteLine($"Zalogowany: {donorManager.GetCurrentDonor()}");

                    donor2.Donate(103, "żywność", 50, "12.12.2024", donationManager);
                    donor2.Donate(10, "odzież", 20, "12.11.2023" ,donationManager);
                    Console.WriteLine("\nDarowizny zostały dodane:");
                    foreach (var donation in donorManager.GetCurrentDonor().donations)
                    {
                        Console.WriteLine(donation);
                    }
                    Console.WriteLine();
                    donorManager.GenerateDonorReport(2);
                }

                donorManager.Logout();
                Console.WriteLine("\nDarczyńca wylogowany.");
                Console.WriteLine(donor2);
                donationManager.RemoveDonation(103);
                Console.WriteLine(donor2);
                Console.WriteLine();
                donorManager.GenerateDonorReport(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
    }
}