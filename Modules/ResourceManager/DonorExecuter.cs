using IO.Modules.Volunteer;
using System.Data;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using IO.Modules.DonorLibrary;
using IO.Modules.Security;


namespace IO.Modules.ResourceManager
{
    public class DonorExecuter
    {
        private readonly string _connectionString =
                    "Data Source=./databases/donorDatabase.db;Cache=Shared";

        public bool SendDonationToDatabase(IO.Modules.DonorLibrary.Donation donation)
        {
            if (donation == null) throw new ArgumentNullException(nameof(donation));
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Donation WHERE donationID = @donationID";
                using var checkCommand = new SqliteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@donationID", donation.DonationID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                string query;
                if (count > 0)
                {
                    query = @"
        UPDATE Donation 
        SET donationStatus = @donationStatus, item = @item, quantity = @quantity, date = @date, email = @email
        WHERE donationID = @donationID";
                }
                else
                {
                    query = @"
        INSERT INTO Donation (donationStatus, item, quantity, date, email)
        VALUES (@donationStatus, @item, @quantity, @date, @email);
        SELECT last_insert_rowid();";
                }

                using var command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@donationStatus", donation.DonationStatus);
                command.Parameters.AddWithValue("@item", donation.Item);
                command.Parameters.AddWithValue("@quantity", donation.Quantity);
                command.Parameters.AddWithValue("@date", donation.Date);
                command.Parameters.AddWithValue("@email", donation.Email);


                int rowsAffected;
                if (count > 0)
                {
                    command.Parameters.AddWithValue("@donationID", donation.DonationID);
                    rowsAffected = command.ExecuteNonQuery();
                }
                else
                {
                    donation.DonationID = Convert.ToInt32(command.ExecuteScalar());
                    rowsAffected = 1;
                }

                Console.WriteLine(rowsAffected > 0
                    ? $"VolunteerTask {donation.DonationID} saved/updated in the database."
                    : $"Failed to save/update volunteerTask {donation.DonationID} to database.");
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving volunteerTask to the database: {e.Message}");
                return false;
            }
        }

        public List<Donation> LoadDonationList()
        {
            var donations = new List<Donation>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"
            SELECT donationID, donationStatus, item, quantity, date, email
            FROM Donation";

                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var donation = new Donation();
                    donation.DonationID = reader.GetInt32(0);
                    donation.DonationStatus = Enum.Parse<IO.Modules.DonorLibrary.Status>(reader.GetString(1));
                    donation.Item = reader.GetString(2);
                    donation.Quantity = reader.GetInt32(3);
                    donation.Date = reader.GetString(4);
                    donation.Email = reader.GetString(5);
                    donations.Add(donation);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading VolunteerTasks from database: {e.Message}");
            }

            return donations;
        }

        public void Dispose()
        {
            // Nothing to dispose explicitly in the current setup
        }

        public async ValueTask DisposeAsync()
        {
            // Nothing to dispose explicitly in the current setup
        }
    }
}