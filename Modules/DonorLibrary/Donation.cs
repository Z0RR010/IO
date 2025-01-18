namespace IO.Modules.DonorLibrary
{
    public class Donation
    {
        public int DonationID { get;  set; }
        public Status DonationStatus { get;  set; }
        public Donor AssignedDonor { get;  set; }
        public string Item { get;  set; }
        public int? Quantity { get;  set; }
        public string Date { get;  set; }
        public string Email { get; set; }
        public bool Selected { get; set; }

        public Donation(int donationID, string item, int? quantity, string date, string email)
        {
            DonationID = donationID;
            DonationStatus = Status.Registered;
            Item = item;
            Quantity = quantity;
            Date = date;
            Email = email;
        }

        public Donation()
        {
        }

        public void AssignDonor(Donor donor)
        {
            if (AssignedDonor != null)
                throw new InvalidOperationException($"Darowizna (ID: {DonationID}) została już przypisana do darczyńcy {AssignedDonor.getDonorInfo()}.");

            AssignedDonor = donor;
        }

        public void SetDonationStatus(Status newStatus)
        {
            DonationStatus = newStatus;
        }

        public override string ToString()
        {
            return $"Donation ID: {DonationID}, Item: {Item}, Quantity: {Quantity}, Data: {Date}, Status: {DonationStatus}, Donor: {AssignedDonor?.getDonorInfo() ?? "Brak"}";
        }
    }
}
