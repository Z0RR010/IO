using IO.Modules.Volunteer;
<<<<<<< Updated upstream
using MySql.Data.MySqlClient;
=======
using System.Data.SQLite;
>>>>>>> Stashed changes

public class VolunteerExecuter : IDisposable, IAsyncDisposable
{
    private readonly string _connectionString =
<<<<<<< Updated upstream
        "Server=localhost;Port=3306;Database=volunteerDatabase;User Id=root;Password=root;";
=======
        "Data Source=Modules/ResourceManager/databases/volunteerDatabase.db;Version=3;FailIfMissing=True;";
    //"Data Source=Modules/ResourceManager/databases/volunteerDatabase.db;Version=3;FailIfMissing=True;Pooling=true;";
>>>>>>> Stashed changes

    public bool AddOrganisationToDatabase(Organisation organisation)
    {
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string orgQuery = @"
            INSERT INTO Organisations (OrganisationID, OrganisationName, PhoneNumber, Address)
            VALUES (@id, @name, @phone, @address)
            ON DUPLICATE KEY UPDATE
                OrganisationName = @name,
                PhoneNumber = @phone,
                Address = @address";

            using var orgCommand = new MySqlCommand(orgQuery, connection);
            orgCommand.Parameters.AddWithValue("@id", organisation.OrganisationID);
            orgCommand.Parameters.AddWithValue("@name", organisation.OrganisationName);
            orgCommand.Parameters.AddWithValue("@phone", organisation.PhoneNumber);
            orgCommand.Parameters.AddWithValue("@address", organisation.Address);

            int rowsAffected = orgCommand.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0
                ? $"Organisation {organisation.OrganisationName} saved/updated in the database."
                : $"Failed to save/update organisation {organisation.OrganisationName}.");
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error saving organisation to the database: {e.Message}");
            return false;
        }
    }

    public bool AddVolunteerToDatabase(Volunteer volunteer, Organisation organisation)
    {
        if (volunteer == null) throw new ArgumentNullException(nameof(volunteer));
        if (organisation == null) throw new ArgumentNullException(nameof(organisation));

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string volQuery = @"
            INSERT INTO Volunteers (VolunteerID, FirstName, LastName, Email, Gender, PhoneNumber, Address, Experience, AdditionalInfo, Skills, Availability, OrganisationID)
            VALUES (@id, @firstName, @lastName, @email, @gender, @phone, @address, @experience, @additionalInfo, @skills, @availability, @orgId)
            ON DUPLICATE KEY UPDATE
                FirstName = @firstName,
                LastName = @lastName,
                Email = @email,
                Gender = @gender,
                PhoneNumber = @phone,
                Address = @address,
                Experience = @experience,
                AdditionalInfo = @additionalInfo,
                Skills = @skills,
                Availability = @availability";

            using var volCommand = new MySqlCommand(volQuery, connection);
            volCommand.Parameters.AddWithValue("@id", volunteer.VolunteerID);
            volCommand.Parameters.AddWithValue("@firstName", volunteer.FirstName);
            volCommand.Parameters.AddWithValue("@lastName", volunteer.LastName);
            volCommand.Parameters.AddWithValue("@email", volunteer.Email);
            volCommand.Parameters.AddWithValue("@gender", volunteer.Gender);
            volCommand.Parameters.AddWithValue("@phone", volunteer.PhoneNumber);
            volCommand.Parameters.AddWithValue("@address", volunteer.Address);
            volCommand.Parameters.AddWithValue("@experience", volunteer.Experience);
            volCommand.Parameters.AddWithValue("@additionalInfo", volunteer.AdditionalInfo);
            volCommand.Parameters.AddWithValue("@skills", volunteer.Skills);
            volCommand.Parameters.AddWithValue("@availability", string.Join(", ", volunteer.Availability.Select(a => a.ToString("yyyy-MM-dd"))));
            volCommand.Parameters.AddWithValue("@orgId", organisation.OrganisationID);

            int rowsAffected = volCommand.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0
                ? $"Volunteer {volunteer.FirstName} {volunteer.LastName} saved to database."
                : $"Failed to save volunteer {volunteer.FirstName} {volunteer.LastName} to database.");
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error saving volunteer to the database: {e.Message}");
            return false;
        }
    }


    public List<Volunteer> LoadVolunteerList(List<Organisation> organisationList)
    {
        var volunteers = new List<Volunteer>();

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"
        SELECT v.VolunteerID, v.FirstName, v.LastName, v.Email, v.Gender, v.PhoneNumber, v.Address, v.Experience, v.AdditionalInfo, v.Skills, v.Availability, v.OrganisationID
        FROM Volunteers v";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var volunteer = new Volunteer();
                volunteer.VolunteerID = reader.GetInt32("VolunteerID");
                volunteer.FirstName = reader.GetString("FirstName");
                volunteer.LastName = reader.GetString("LastName");
                volunteer.Email = reader.GetString("Email");
                volunteer.Gender = Enum.TryParse(reader.GetString("Gender"), out Gender gender) ? gender : Gender.Female; // Default to Female if parsing fails
                volunteer.PhoneNumber = reader.GetString("PhoneNumber").ToCharArray();
                volunteer.Address = reader.GetString("Address");
                volunteer.Experience = reader.GetString("Experience");
                volunteer.AdditionalInfo = reader.GetString("AdditionalInfo");
                volunteer.Skills = reader.GetString("Skills");
                volunteer.Availability = reader.GetString("Availability").Split(", ").Select(DateTime.Parse).ToList();
                volunteer.Organisation = organisationList.FirstOrDefault(o => o.OrganisationID == reader.GetInt32("OrganisationID")); // This can be filled later with the Organisation from its ID

                // Optionally, you can also fetch the associated organisation here and set it to the volunteer
                // For simplicity, we are not doing this here

                volunteers.Add(volunteer);
            }

            return volunteers;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading volunteers from database: {e.Message}");
            return new List<Volunteer>(); // Return empty list on error
        }
    }



    public List<Organisation> LoadOrganisationList()
    {
        var organisations = new List<Organisation>();

        try
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT OrganisationID, OrganisationName, PhoneNumber, Address FROM Organisations";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var organisation = new Organisation();

<<<<<<< Updated upstream
                organisation.OrganisationID = reader.GetInt32("OrganisationID");
                organisation.OrganisationName = reader.GetString("OrganisationName");
                organisation.PhoneNumber = reader.GetString("PhoneNumber").ToCharArray();
                organisation.Address = reader.GetString("Address");
                
=======
                organisation.OrganisationID = reader.GetInt32(0);//"OrganisationID");
                organisation.OrganisationName = reader.GetString(1);//"OrganisationName");
                organisation.PhoneNumber = reader.GetString(2).ToCharArray();//"PhoneNumber").ToCharArray();
                organisation.Address = reader.GetString(3);//"Address");

>>>>>>> Stashed changes

                organisations.Add(organisation);
            }

            return organisations;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading organisations from database: {e.Message}");
            return new List<Organisation>(); // Return empty list on error
        }
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
