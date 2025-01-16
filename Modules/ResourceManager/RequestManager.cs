using IO.Modules.Security;
using RequestModule;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;

namespace IO.Modules.ResourceManager
{
    public class RequestManager : IRequestManager, IDisposable, IAsyncDisposable
    {
        private readonly SQLiteConnection _connection;

        public RequestManager()
        {
            _connection = new SQLiteConnection(
                "Data Source=./Modules/ResourceManager/databases/requestDatabase.db;Version=3;FailIfMissing=True;");
            _connection.Open();
        }

        bool IRequestManager.AddRequestToDatabase(Request request)
        {
            //var packedUser = JsonSerializer.Serialize();

            List<Resource> resources = (List<Resource>) request.ResourcesRequired;

            StringBuilder sb = new StringBuilder();

            foreach (Resource res in resources)
            {
                sb.Append(JsonSerializer.Serialize(res).ToString());
            }

            string query =
                "INSERT INTO Request(Id, Title, Description, CreatedAt, DateUpdated, Status, User, Address, ResourcesRequired, IsVerified, HandlingOrganization) VALUES(@id, @title, @description, @createdAt, @dateUpdated, @status, @user, @address, @resRequired, @isVerified, @handlingOrganization)";
            try
            {
                using (var command = new SQLiteCommand(query, _connection)) //MySqlCommand(query, _userConnection))
                {
                    command.Parameters.AddWithValue("@id", request.Id);
                    command.Parameters.AddWithValue("@title", request.Title);
                    command.Parameters.AddWithValue("@description", request.Description);
                    command.Parameters.AddWithValue("@createdAt", request.CreatedAt);
                    command.Parameters.AddWithValue("@dateUpdated", request.DateUpdated);
                    command.Parameters.AddWithValue("@status", request.Status.ToString());
                    command.Parameters.AddWithValue("@user", request.User.Email);
                    command.Parameters.AddWithValue("@address", request.Address.ToString());
                    command.Parameters.AddWithValue("@resRequired", sb.ToString());
                    command.Parameters.AddWithValue("@isVerified", request.IsVerified);
                    command.Parameters.AddWithValue("@handlingOrganization", request.HandlingOrganization);
                   
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        string IRequestManager.CustomQuery(string query)
        {
            if (query.ToLower().Contains("DROP DATABASE".ToLower()) || query.ToLower().Contains("DROP TABLE".ToLower()))
            {
                throw new Exception("Are you dumb? What are you trying to do?");
            }

            if (query == null) throw new ArgumentNullException("query");

            try
            {
                using (var command = new SQLiteCommand(query, _connection))
                using (var reader = command.ExecuteReader())
                {
                    var result = new StringBuilder();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        result.Append(reader.GetName(i));
                        if (i < reader.FieldCount - 1) result.Append(", ");
                    }

                    result.AppendLine();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            result.Append(reader.GetValue(i));
                            if (i < reader.FieldCount - 1) result.Append(", ");
                        }

                        result.AppendLine();
                    }

                    return result.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        bool IRequestManager.RemoveRequestFromDatabase(int id)
        {
            string query =
                "DELETE FROM Request WHERE Id = @id";
            try
            {
                using (var command = new SQLiteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public List<Request> GetAllRequests()
        {
            string query = "SELECT * FROM Request";
            var requests = new List<Request>();

            IUserManager um = new UserExecuter();

            try
            {
                using (var command = new SQLiteCommand(query, _connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var request = new Request
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            DateUpdated = reader.IsDBNull(reader.GetOrdinal("DateUpdated"))
                                ? (DateTime?)null
                                : reader.GetDateTime(reader.GetOrdinal("DateUpdated")),
                            Status = Enum.Parse<RequestStatus>(reader.GetString(reader.GetOrdinal("Status"))),
                            User = um.GetUserFromDataBase(reader.GetString(reader.GetOrdinal("User"))),
                            Address = JsonSerializer.Deserialize<Address>(reader.GetString(reader.GetOrdinal("Address"))),
                            IsVerified = reader.GetBoolean(reader.GetOrdinal("IsVerified")),
                            HandlingOrganization = reader.GetString(reader.GetOrdinal("HandlingOrganization")),
                        };

                        
                        string resourcesString = reader.GetString(reader.GetOrdinal("ResourcesRequired"));
                        var resources = new List<Resource>();

                        if (!string.IsNullOrWhiteSpace(resourcesString))
                        {
                            
                            var resourceStrings = resourcesString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var resourceJson in resourceStrings)
                            {
                                try
                                {
                                    var resource = JsonSerializer.Deserialize<Resource>(resourceJson);
                                    if (resource != null)
                                    {
                                        resources.Add(resource);
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    Console.WriteLine($"Error while deserialisation: {ex.Message}");
                                }
                            }
                        }

                        request.ResourcesRequired = resources;
                        requests.Add(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetAllRequests: {ex.Message}");
                throw;
            }

            return requests;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }

    }
}
