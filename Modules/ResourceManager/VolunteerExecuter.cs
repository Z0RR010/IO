using IO.Modules.Volunteer;
using System.Data;
using System.Text.Json;
using Microsoft.Data.Sqlite;


namespace IO.Modules.ResourceManager
{
    public class VolunteerExecuter : IVolunteerManager, IDisposable, IAsyncDisposable
    {
        private readonly string _connectionString =
                    "Data Source=./databases/volunteerDatabase.db;Cache=Shared";

        public bool SendOrganisationToDatabase(Organisation organisation)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                Console.WriteLine("Connection to " + connection.ConnectionString + " established");

                // Sprawdzenie czy organizacja już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Organisations WHERE OrganisationID = @id";
                using var checkCommand = new SqliteCommand(checkQuery, connection);
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
            INSERT INTO Organisations ( OrganisationName, Email, PhoneNumber, Address)
            VALUES ( @name, @email, @phone, @address);
            SELECT last_insert_rowid();";
                }

                using var orgCommand = new SqliteCommand(orgQuery, connection);

                orgCommand.Parameters.AddWithValue("@email", organisation.Email);
                orgCommand.Parameters.AddWithValue("@name", organisation.OrganisationName);
                orgCommand.Parameters.AddWithValue("@phone", organisation.PhoneNumber);
                orgCommand.Parameters.AddWithValue("@address", organisation.Address);

                int rowsAffected;
                if (count > 0)
                {
                    orgCommand.Parameters.AddWithValue("@id", organisation.OrganisationID);
                    rowsAffected = orgCommand.ExecuteNonQuery();

                }
                else
                {
                    organisation.OrganisationID = Convert.ToInt32(orgCommand.ExecuteScalar());
                    rowsAffected = 1;
                }


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

        public bool SendVolunteerToDatabase(IO.Modules.Volunteer.Volunteer volunteer)
        {
            if (volunteer == null) throw new ArgumentNullException(nameof(volunteer));

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                // Sprawdzenie, czy wolontariusz już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Volunteers WHERE VolunteerID = @id";
                using var checkCommand = new SqliteCommand(checkQuery, connection);
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
            INSERT INTO Volunteers ( FirstName, LastName, Email, PhoneNumber, Address, OrganisationID)
            VALUES ( @firstName, @lastName, @email, @phone, @address, @orgId);
            SELECT last_insert_rowid();";
                }


                using var volCommand = new SqliteCommand(volQuery, connection);

                volCommand.Parameters.AddWithValue("@firstName", volunteer.FirstName);
                volCommand.Parameters.AddWithValue("@lastName", volunteer.LastName);
                volCommand.Parameters.AddWithValue("@email", volunteer.Email);
                volCommand.Parameters.AddWithValue("@phone", volunteer.PhoneNumber);
                volCommand.Parameters.AddWithValue("@address", volunteer.Address);
                volCommand.Parameters.AddWithValue("@orgId", volunteer.OrganisationID);

                int rowsAffected;
                if (count > 0)
                {
                    volCommand.Parameters.AddWithValue("@id", volunteer.VolunteerID);
                    rowsAffected = volCommand.ExecuteNonQuery();
                }
                else
                {
                    volunteer.VolunteerID = Convert.ToInt32(volCommand.ExecuteScalar());
                    rowsAffected = 1;
                }

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

        public bool SendTaskToDatabase(IO.Modules.Volunteer.VolunteerTask volunteerTask)
        {
            if (volunteerTask == null) throw new ArgumentNullException(nameof(volunteerTask));
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                // Sprawdzenie, czy volunteerTask już istnieje
                string checkQuery = "SELECT COUNT(*) FROM VolunteerTasks WHERE VolunteerTaskID = @volunteerTaskID";
                using var checkCommand = new SqliteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@volunteerTaskID", volunteerTask.VolunteerTaskID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                string query;
                if (count > 0)
                {
                    // Aktualizacja istniejącego taska
                    query = @"
        UPDATE VolunteerTasks 
        SET Description = @description, Address = @address, TaskStatus = @taskStatus, EndDate = @endDate, OrganisationID = @organisationID, VolunteerID = @volunteerID, RequestID = @requestID
        WHERE VolunteerTaskID = @volunteerTaskID";
                }
                else
                {
                    // Wstawianie nowego taska
                    query = @"
        INSERT INTO VolunteerTasks (Description, Address, TaskStatus, EndDate, OrganisationID, VolunteerID, RequestID)
        VALUES (@description, @address, @taskStatus, @endDate, @organisationID, @volunteerID, @requestID);
        SELECT last_insert_rowid();";
                }

                using var command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@description", volunteerTask.Description);
                command.Parameters.AddWithValue("@address", volunteerTask.Address);
                command.Parameters.AddWithValue("@taskStatus", volunteerTask.TaskStatus.ToString());
                command.Parameters.AddWithValue("@endDate", volunteerTask.EndDate.ToString());
                command.Parameters.AddWithValue("@organisationID", volunteerTask.OrganisationID);
                command.Parameters.AddWithValue("@volunteerID", volunteerTask.VolunteerID);
                command.Parameters.AddWithValue("@requestID", volunteerTask.RequestID);


                int rowsAffected;
                if (count > 0)
                {
                    command.Parameters.AddWithValue("@volunteerTaskID", volunteerTask.VolunteerTaskID);
                    rowsAffected = command.ExecuteNonQuery();
                }
                else
                {
                    volunteerTask.VolunteerTaskID = Convert.ToInt32(command.ExecuteScalar());
                    rowsAffected = 1;
                }

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
        public bool SendRateToDatabase(Rate rate)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                // Sprawdzenie, czy rate już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Rates WHERE RateID = @rateID";
                using var checkCommand = new SqliteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@rateID", rate.RateID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                string query;
                if (count > 0)
                {
                    // Aktualizacja istniejącego rate
                    query = @"SELECT RateID FROM Rates WHERE RateID = @rateID";
                }
                else
                {
                    // Wstawianie nowego rate
                    query = @"
                    INSERT INTO Rates (Description)
                    VALUES (@description);
                    SELECT last_insert_rowid();";
                }

                using var command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@rateID", rate.RateID);
                command.Parameters.AddWithValue("@description", rate.Description);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending rate to database: {e.Message}");
                return false;
            }
        }

        public List<Rate> LoadRateList()
        {
            var rates = new List<Rate>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"
            SELECT RateID, Description
            FROM Rates";

                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var rate = new Rate();
                    rate.RateID = reader.GetInt32(0);
                    rate.Description = reader.GetString(1);
                    rates.Add(rate);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading rates from database: {e.Message}");
            }

            return rates;
        }

        public List<VolunteerTask> LoadTaskList()
        {
            var tasks = new List<VolunteerTask>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"
            SELECT VolunteerTaskID, Description, Address, TaskStatus, EndDate, OrganisationID, VolunteerID, RequestID
            FROM VolunteerTasks";

                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var task = new VolunteerTask();
                    task.VolunteerTaskID = reader.GetInt32(0);
                    task.Description = reader.GetString(1);
                    task.Address = reader.GetString(2);
                    task.TaskStatus = Enum.Parse<IO.Modules.Volunteer.TaskStatus>(reader.GetString(3));
                    task.EndDate = DateTime.Parse(reader.GetString(4));
                    task.OrganisationID = reader.GetInt32(5);
                    task.VolunteerID = reader.GetInt32(6);
                    task.RequestID = reader.GetInt32(7);
                    tasks.Add(task);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading VolunteerTasks from database: {e.Message}");
            }

            return tasks;
        }


        public List<IO.Modules.Volunteer.Volunteer> LoadVolunteerList()
        {
            var volunteers = new List<IO.Modules.Volunteer.Volunteer>();

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();
                string query = @"
        SELECT v.VolunteerID, v.FirstName, v.LastName, v.Email, v.PhoneNumber, v.Address, v.OrganisationID
        FROM Volunteers v";

                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var volunteer = new IO.Modules.Volunteer.Volunteer();
                    volunteer.VolunteerID = reader.GetInt32(0);
                    volunteer.FirstName = reader.GetString(1);
                    volunteer.LastName = reader.GetString(2);
                    volunteer.Email = reader.GetString(3);
                    volunteer.PhoneNumber = reader.GetString(4);
                    volunteer.Address = reader.GetString(5);
                    volunteer.OrganisationID = reader.GetInt32(6);


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
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "SELECT OrganisationID, OrganisationName, Email, PhoneNumber, Address FROM Organisations";

                using var command = new SqliteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var organisation = new Organisation();

                    organisation.OrganisationID = reader.GetInt32(0);
                    organisation.OrganisationName = reader.GetString(1);
                    organisation.Email = reader.GetString(2);
                    organisation.PhoneNumber = reader.GetString(3);
                    organisation.Address = reader.GetString(4);
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