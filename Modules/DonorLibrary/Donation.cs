namespace IO.Modules.DonorLibrary
{
    public class Donation
    {
        public int DonationID { get; private set; }
        public Status DonationStatus { get; private set; }
        public Donor AssignedDonor { get; private set; }
        public string Item { get; private set; }
        public int? Quantity { get; private set; }
        public string Date { get; private set; }
        public bool Selected { get; set; }
        public Donation(int donationID, string item, int? quantity, string date)
        {
            DonationID = donationID;
            DonationStatus = Status.Registered;
            Item = item;
            Quantity = quantity;
            Date = date;
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
