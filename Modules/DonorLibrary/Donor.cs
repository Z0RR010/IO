namespace IO.Modules.DonorLibrary
{
    public class Donor
    {
        public int donorID { get; private set; }
        public IDonorType donorType { get; set; }
        public List<Donation> donations { get; private set; }

        public Donor(int donorID, IDonorType donorType)
        {
            this.donorID = donorID;
            this.donorType = donorType;
            donations = new List<Donation>();
        }
        public void Donate(string item, int quantity, string date, DonationManager donationManager)
        {
            donationManager.AddDonation(item, quantity, date);
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