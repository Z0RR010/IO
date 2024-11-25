using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class DonationManager
    {
        private List<Donation> donations;

        public DonationManager()
        {
            donations = new List<Donation>();
        }

        public void AddDonation(Donation donation)
        {
            if (donation == null)
                throw new ArgumentNullException(nameof(donation), "Donation cannot be null.");

            if (donations.Any(d => d.DonationID == donation.DonationID))
                throw new InvalidOperationException($"Darowizna o ID {donation.DonationID} już istnieje w systemie.");

            donations.Add(donation);
        }
        public void RemoveDonation(int donationID)
        {
            var donation = donations.FirstOrDefault(d => d.DonationID == donationID);
            if (donation == null)
                throw new KeyNotFoundException($"Donation with ID: {donationID} not found.");

            donations.Remove(donation);
            donation.AssignedDonor?.donations.Remove(donation);
        }

        public Donation GetDonationById(int donationID)
        {
            var donation = donations.FirstOrDefault(d => d.DonationID == donationID);
            if (donation == null)
                throw new KeyNotFoundException($"Donation with ID: {donationID} not found.");
            return donation;
        }

        public IEnumerable<Donation> GetDonationsByDonor(Donor donor)
        {
            return donations.Where(d => d.AssignedDonor == donor);
        }
    }
}
