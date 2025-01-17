using IO.Modules.Volunteer;
using System.Data;
using System.Data.SQLite;
using System.Text.Json;

namespace IO.Modules.ResourceManager
{
    public class VolunteerExecuter : IDisposable, IAsyncDisposable
    {
        private readonly string _connectionString =
            "Data Source=./Modules/ResourceManager/databases/volunteerDatabase.db;Version=3;FailIfMissing=True;";

        public bool AddOrganisationToDatabase(Organisation organisation)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();
                Console.WriteLine("Connection to " + connection.FileName + " established");

                // Sprawdzenie czy organizacja już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Organisations WHERE OrganisationID = @id";
                using var checkCommand = new SQLiteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@id", organisation.OrganisationID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());


                string orgQuery;
                if (count > 0)
                {
                    // Aktualizacja istniejącej organizacji
                    orgQuery = @"
            UPDATE Organisations 
            SET OrganisationName = @name, Email = @email, PhoneNumber = @phone, Address = @address

            WHERE OrganisationID = @id";
                }
                else
                {
                    // Wstawianie nowej organizacji
                    orgQuery = @"
            INSERT INTO Organisations (OrganisationID, OrganisationName, Email, PhoneNumber, Address)
            VALUES (@id, @email, @name, @phone, @address)";
                }

                using var orgCommand = new SQLiteCommand(orgQuery, connection);
                var packedOrg = JsonSerializer.Serialize(organisation);

                orgCommand.Parameters.AddWithValue("@id", organisation.OrganisationID);
                orgCommand.Parameters.AddWithValue("@email", organisation.Email);
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

        public bool AddVolunteerToDatabase(IO.Modules.Volunteer.Volunteer volunteer, Organisation organisation)
        {
            if (volunteer == null) throw new ArgumentNullException(nameof(volunteer));
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                // Sprawdzenie, czy wolontariusz już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Volunteers WHERE VolunteerID = @id";
                using var checkCommand = new SQLiteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@id", volunteer.VolunteerID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                string volQuery;
                if (count > 0)
                {
                    // Aktualizacja istniejącego wolontariusza
                    volQuery = @"
                UPDATE Volunteers
                SET FirstName = @firstName,
                    LastName = @lastName,
                    Email = @email,
                    PhoneNumber = @phone,
                    Address = @address,
                    OrganisationID = @orgId
                WHERE VolunteerID = @id";
                }
                else
                {
                    // Wstawianie nowego wolontariusza
                    volQuery = @"
            INSERT INTO Volunteers (VolunteerID, FirstName, LastName, Email, PhoneNumber, Address, OrganisationID)
            VALUES (@id, @firstName, @lastName, @email, @phone, @address, @orgId)";
                }


                using var volCommand = new SQLiteCommand(volQuery, connection);
                volCommand.Parameters.AddWithValue("@id", volunteer.VolunteerID);
                volCommand.Parameters.AddWithValue("@firstName", volunteer.FirstName);
                volCommand.Parameters.AddWithValue("@lastName", volunteer.LastName);
                volCommand.Parameters.AddWithValue("@email", volunteer.Email);
                volCommand.Parameters.AddWithValue("@phone", volunteer.PhoneNumber);
                volCommand.Parameters.AddWithValue("@address", volunteer.Address);
                volCommand.Parameters.AddWithValue("@orgId", organisation.OrganisationID);

                int rowsAffected = volCommand.ExecuteNonQuery();
                Console.WriteLine(rowsAffected > 0
                    ? $"Volunteer {volunteer.FirstName} {volunteer.LastName} saved/updated in the database."
                    : $"Failed to save/update volunteer {volunteer.FirstName} {volunteer.LastName} to database.");
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving volunteer to the database: {e.Message}");
                return false;
            }
        }

        public bool AddTaskToDatabase(IO.Modules.Volunteer.VolunteerTask volunteerTask)
        {
            if (volunteerTask == null) throw new ArgumentNullException(nameof(volunteerTask));
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                // Sprawdzenie, czy volunteerTask już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Tasks WHERE VolunteerTaskID = @volunteerTaskID";
                using var checkCommand = new SQLiteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@volunteerTaskID", volunteerTask.VolunteerTaskID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                string query;
                if (count > 0)
                {
                    // Aktualizacja istniejącego taska
                    query = @"
        UPDATE Tasks 
        SET AllInfo = @allInfo
        WHERE VolunteerTaskID = @volunteerTaskID";
                }
                else
                {
                    // Wstawianie nowego taska
                    query = @"
        INSERT INTO Tasks (VolunteerTaskID, AllInfo)
        VALUES (@volunteerTaskID, @allInfo)";
                }

                var packedTask = JsonSerializer.Serialize(volunteerTask);
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@volunteerTaskID", volunteerTask.VolunteerTaskID);
                command.Parameters.AddWithValue("@allInfo", packedTask);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected > 0
                    ? $"VolunteerTask {volunteerTask.VolunteerTaskID} saved/updated in the database."
                    : $"Failed to save/update volunteerTask {volunteerTask.VolunteerTaskID} to database.");
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving volunteerTask to the database: {e.Message}");
                return false;
            }
        }

        public List<VolunteerTask> LoadTaskList()
        {
            var tasks = new List<VolunteerTask>();


            for (int i = 0; ;i++)
            {
                string query = "SELECT AllInfo FROM tasks WHERE VolunteerTaskID = @volunteerTaskID";
                try
                {
                    using var connection = new SQLiteConnection(_connectionString);

                    using var command = new SQLiteCommand(query, connection);
                    connection.Open();

                    command.Parameters.AddWithValue("@volunteerTaskID", i);
                    var input = command.ExecuteScalar()?.ToString();
                    if (input != null)
                    {
                        VolunteerTask volunteerTask = JsonSerializer.Deserialize<VolunteerTask> (input);
                        tasks.Add(volunteerTask);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error saving volunteerTask to the database: {e.Message}");
                }

                return tasks;
            }

        }
        public List<IO.Modules.Volunteer.Volunteer> LoadVolunteerList(List<Organisation> organisationList)
        {
            var volunteers = new List<IO.Modules.Volunteer.Volunteer>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString); //MySqlConnection(_connectionString);
                connection.Open();

                string query = @"
        SELECT v.VolunteerID, v.FirstName, v.LastName, v.Email, v.PhoneNumber, v.Address, v.OrganisationID
        FROM Volunteers v";

                using var command = new SQLiteCommand(query, connection); //MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var volunteer = new IO.Modules.Volunteer.Volunteer();
                    // Problems encountered that sqlite data reader methods don't accept strings with name of column
                    volunteer.VolunteerID = reader.GetInt32(0); //"VolunteerID");
                    volunteer.FirstName = reader.GetString(1); //"FirstName");
                    volunteer.LastName = reader.GetString(2); //"LastName");
                    volunteer.Email = reader.GetString(3); //"Email");
                    volunteer.PhoneNumber = reader.GetString(4).ToCharArray(); //"PhoneNumber").ToCharArray();
                    volunteer.Address = reader.GetString(5); //"Address");
                    volunteer.Organisation =
                        organisationList.FirstOrDefault(o =>
                            o.OrganisationID ==
                            reader.GetInt32(
                                6)); 

                    volunteers.Add(volunteer);
                }

                return volunteers;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading volunteers from database: {e.Message}");
                return new List<IO.Modules.Volunteer.Volunteer>(); // Return empty list on error
            }
        }


        public List<Organisation> LoadOrganisationList()
        {
            var organisations = new List<Organisation>();

            try
            {
                using var connection = new SQLiteConnection(_connectionString); //MySqlConnection(_connectionString);
                connection.Open();

                string query = "SELECT OrganisationID, Email, OrganisationName, PhoneNumber, Address FROM Organisations";

                using var command = new SQLiteCommand(query, connection); //MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var organisation = new Organisation();

                    organisation.OrganisationID = reader.GetInt32("OrganisationID");
                    organisation.Email = reader.GetString("Email");
                    organisation.OrganisationName = reader.GetString("OrganisationName");
                    organisation.PhoneNumber = reader.GetString("PhoneNumber").ToCharArray();
                    organisation.Address = reader.GetString("Address");
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
}