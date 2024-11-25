using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class Donor
    {
        public int donorID { get; private set; }
        private IDonorType donorType { get; set; }
        public List<Donation> donations { get; private set; }

        public Donor(int donorID, IDonorType donorType)
        {
            this.donorID = donorID;
            this.donorType = donorType;
            donations = new List<Donation>();
        }
        public void Donate(int donationID, string item, int quantity, string date, DonationManager donationManager)
        {
            var donation = new Donation(donationID, item, quantity, date);
            donationManager.AddDonation(donation);
            donations.Add(donation);
            donation.AssignDonor(this);
        }

        public Donation GetDonationById(int donationID)
        {
            foreach (var donation in donations)
            {
                if (donation.DonationID == donationID)
                    return donation;
            }
            throw new KeyNotFoundException($"Donation with ID: {donationID} not found.");
        }
        //public List<Donation> getDonations()
        //{
        //    return donations;
        //}

        public string getDonorInfo()
        {
            return $"ID: {donorID}, Type: {donorType.getTypeName()}, Name: {donorType.getName()}, Address: {donorType.getAddress()}, " +
                $"Phone Number: {donorType.getContactNumber()}, Identifier: {donorType.getIdentifier()}";
        }

        public override string ToString()
        {
            return $"{getDonorInfo()}, Donations Count: {donations.Count}";
        }
    }
}