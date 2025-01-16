using IO.Modules.Security;
using IO.Modules.Volunteer;
using MySql.Data.MySqlClient;
using System.Data.SQLite;
using System.Text.Json;
using System.Threading.Tasks;

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
            SET OrganisationName = @name, PhoneNumber = @phone, Address = @address, Everythink = @everythink

            WHERE OrganisationID = @id";
                }
                else
                {
                    // Wstawianie nowej organizacji
                    orgQuery = @"
            INSERT INTO Organisations (OrganisationID, OrganisationName, PhoneNumber, Address, Everythink)
            VALUES (@id, @name, @phone, @address, @everythink)";
                }

                using var orgCommand = new SQLiteCommand(orgQuery, connection);
                var packedOrg = JsonSerializer.Serialize(organisation);

                orgCommand.Parameters.AddWithValue("@id", organisation.OrganisationID);
                orgCommand.Parameters.AddWithValue("@name", organisation.OrganisationName);
                orgCommand.Parameters.AddWithValue("@phone", organisation.PhoneNumber);
                orgCommand.Parameters.AddWithValue("@address", organisation.Address);
                orgCommand.Parameters.AddWithValue("@everythink", packedOrg);

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
                    Gender = @gender,
                    PhoneNumber = @phone,
                    Address = @address,
                    Experience = @experience,
                    AdditionalInfo = @additionalInfo,
                    Skills = @skills,
                    Availability = @availability,
                    OrganisationID = @orgId
                WHERE VolunteerID = @id";
                }
                else
                {
                    // Wstawianie nowego wolontariusza
                    volQuery = @"
            INSERT INTO Volunteers (VolunteerID, FirstName, LastName, Email, Gender, PhoneNumber, Address, Experience, AdditionalInfo, Skills, Availability, OrganisationID)
            VALUES (@id, @firstName, @lastName, @email, @gender, @phone, @address, @experience, @additionalInfo, @skills, @availability, @orgId)";
                }


                using var volCommand = new SQLiteCommand(volQuery, connection);
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
                volCommand.Parameters.AddWithValue("@availability",
                    string.Join(", ", volunteer.Availability.Select(a => a.ToString("yyyy-MM-dd"))));
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

        public bool AddTaskToDatabase(IO.Modules.Volunteer.Task task)
        {
            Console.WriteLine("ADD TASK TO DATABASE");
            if (task == null) throw new ArgumentNullException(nameof(task));
            try
            {
                using var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                // Sprawdzenie, czy task już istnieje
                string checkQuery = "SELECT COUNT(*) FROM Tasks WHERE TaskID = @taskID";
                using var checkCommand = new SQLiteCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@taskID", task.TaskID);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                string query;
                Console.WriteLine("ADD OR UPDATE");

                if (count > 0)
                {
                    // Aktualizacja istniejącego taska
                    query = @"
        UPDATE Tasks 
        SET AllInfo = @allInfo
        WHERE TaskID = @taskID";
                    Console.WriteLine("UPDATE");
                }
                else
                {
                    // Wstawianie nowego taska
                    query = @"
        INSERT INTO Tasks (TaskID, AllInfo)
        VALUES (@taskID, @allInfo)";
                }

                var packedTask = JsonSerializer.Serialize(task);
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@taskID", task.TaskID);
                command.Parameters.AddWithValue("@allInfo", packedTask);
                Console.WriteLine(packedTask);
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected > 0
                    ? $"Task {task.TaskID} saved/updated in the database."
                    : $"Failed to save/update task {task.TaskID} to database.");
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving task to the database: {e.Message}");
                return false;
            }
        }

        public List<IO.Modules.Volunteer.Task> LoadTaskList()
        {
            var tasks = new List<IO.Modules.Volunteer.Task>();

            //if (IsUserInDataBase(email))
            //{
            //    string query = "SELECT user FROM users WHERE email = @email";

            //    using (var command = new SQLiteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
            //    {
            //        command.Parameters.AddWithValue("@email", email);

            //        var input = command.ExecuteScalar()?.ToString();
            //        if (input != null)
            //        {
            //            return JsonSerializer.Deserialize<Individual>(input);
            //        }

            //        return null;
            //    }
            //}

            //return null;

            for (int i = 0; ;i++)
            {
                string query = "SELECT AllInfo FROM tasks WHERE TaskID = @taskID";
                try
                {
                    using var connection = new SQLiteConnection(_connectionString);

                    using var command = new SQLiteCommand(query, connection);
                    connection.Open();

                    command.Parameters.AddWithValue("@taskID", i);
                    var input = command.ExecuteScalar()?.ToString();
                    if (input != null)
                    {
                        IO.Modules.Volunteer.Task task = JsonSerializer.Deserialize< IO.Modules.Volunteer.Task> (input);
                        tasks.Add(task);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error saving task to the database: {e.Message}");
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
        SELECT v.VolunteerID, v.FirstName, v.LastName, v.Email, v.Gender, v.PhoneNumber, v.Address, v.Experience, v.AdditionalInfo, v.Skills, v.Availability, v.OrganisationID
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
                    volunteer.Gender =
                        Enum.TryParse(reader.GetString(4), out Gender gender)
                            ? gender
                            : Gender
                                .Female; //"Gender"), out Gender gender) ? gender : Gender.Female; // Default to Female if parsing fails
                    volunteer.PhoneNumber = reader.GetString(5).ToCharArray(); //"PhoneNumber").ToCharArray();
                    volunteer.Address = reader.GetString(6); //"Address");
                    volunteer.Experience = reader.GetString(7); //"Experience");
                    volunteer.AdditionalInfo = reader.GetString(8); //"AdditionalInfo");
                    volunteer.Skills = reader.GetString(9); //"Skills");
                    volunteer.Availability =
                        reader.GetString(10).Split(", ").Select(DateTime.Parse)
                            .ToList(); //"Availability").Split(", ").Select(DateTime.Parse).ToList();
                    volunteer.Organisation =
                        organisationList.FirstOrDefault(o =>
                            o.OrganisationID ==
                            reader.GetInt32(
                                11)); //"OrganisationID")); // This can be filled later with the Organisation from its ID

                    // Optionally, you can also fetch the associated organisation here and set it to the volunteer
                    // For simplicity, we are not doing this here

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

                string query = "SELECT OrganisationID, OrganisationName, PhoneNumber, Address FROM Organisations";

                using var command = new SQLiteCommand(query, connection); //MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var organisation = new Organisation();

                    organisation.OrganisationID = reader.GetInt32(0); //"OrganisationID");
                    organisation.OrganisationName = reader.GetString(1); //"OrganisationName");
                    organisation.PhoneNumber = reader.GetString(2).ToCharArray(); //"PhoneNumber").ToCharArray();
                    organisation.Address = reader.GetString(3); //"Address");


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