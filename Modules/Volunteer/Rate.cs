namespace IO.Modules.Volunteer
{
    public class Rate{
        public int RateID { get; set; }
        public string Description { get; set; }

        public Rate() { }

        public Rate(
            string description)
        {
            Description = description;
        }


    }
}
