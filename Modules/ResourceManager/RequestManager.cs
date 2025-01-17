using IO.Modules.Security;
using RequestModule;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Text;
using System.Text.Json;

namespace IO.Modules.ResourceManager
{
    public class RequestManager : IRequestManager, IDisposable, IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        public RequestManager()
        {
            _connection = new SqliteConnection(
                "Data Source=./databases/requestDatabase.db;Cache=Shared");
            _connection.Open();
        }

        public bool AddRequestToDatabase(Request request)
        {
            List<Resource> resources = (List<Resource>)request.ResourcesRequired;

            StringBuilder sb = new StringBuilder();

            foreach (Resource res in resources)
            {
                sb.Append(JsonSerializer.Serialize(res).ToString());
            }

            string query =
                "INSERT INTO Request(Title, Description, CreatedAt, DateUpdated, Status, User, Address, ResourcesRequired, IsVerified, HandlingOrganization) VALUES(@title, @description, @createdAt, @dateUpdated, @status, @user, @address, @resRequired, @isVerified, @handlingOrganization)";
            try
            {
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@title", request.Title);
                    command.Parameters.AddWithValue("@description", request.Description);
                    command.Parameters.AddWithValue("@createdAt", request.CreatedAt);
                    command.Parameters.AddWithValue("@dateUpdated", request.DateUpdated ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@status", request.Status.ToString());
                    command.Parameters.AddWithValue("@user", request.User);
                    command.Parameters.AddWithValue("@address", JsonSerializer.Serialize(request.Address));
                    command.Parameters.AddWithValue("@resRequired", sb.ToString());
                    command.Parameters.AddWithValue("@isVerified", request.IsVerified ? 1 : 0);
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

        public string CustomQuery(string query)
        {
            if (query.ToLower().Contains("DROP DATABASE".ToLower()) || query.ToLower().Contains("DROP TABLE".ToLower()))
            {
                throw new Exception("Are you dumb? What are you trying to do?");
            }

            if (query == null) throw new ArgumentNullException("query");

            try
            {
                using (var command = new SqliteCommand(query, _connection))
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

        public bool RemoveRequestFromDatabase(int id)
        {
            string query =
                "DELETE FROM Request WHERE Id = @id";
            try
            {
                using (var command = new SqliteCommand(query, _connection))
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
                using (var command = new SqliteCommand(query, _connection))
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
                            User = reader.GetString(reader.GetOrdinal("User")),
                            Address = JsonSerializer.Deserialize<Address>(reader.GetString(reader.GetOrdinal("Address"))),
                            IsVerified = reader.GetInt32(reader.GetOrdinal("IsVerified")) == 1,
                            HandlingOrganization = reader.GetString(reader.GetOrdinal("HandlingOrganization")),
                        };

                        string resourcesString = reader.GetString(reader.GetOrdinal("ResourcesRequired"));
                        request.ResourcesRequired = ParseResources(resourcesString);

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

        public async Task<Request> GetRequestById(int id)
        {
            try
            {
                var query = "SELECT * FROM Request WHERE Id = @id";
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
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
                                User = reader.GetString(reader.GetOrdinal("User")),
                                Address = JsonSerializer.Deserialize<Address>(reader.GetString(reader.GetOrdinal("Address"))),
                                IsVerified = reader.GetBoolean(reader.GetOrdinal("IsVerified")),
                                HandlingOrganization = reader.GetString(reader.GetOrdinal("HandlingOrganization")),
                            };

                            string resourcesString = reader.GetString(reader.GetOrdinal("ResourcesRequired"));
                            request.ResourcesRequired = ParseResources(resourcesString);

                            return request;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRequestByIdAsync: {ex.Message}");
                throw;
            }

            return null;
        }

        public List<Resource> GetResourcesForRequest(int requestId)
        {
            var resources = new List<Resource>();
            try
            {
                var query = "SELECT ResourcesRequired FROM Request WHERE Id = @id";
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", requestId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string resourcesString = reader.GetString(reader.GetOrdinal("ResourcesRequired"));
                            resources = ParseResources(resourcesString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetResourcesForRequest: {ex.Message}");
                throw;
            }

            return resources;
        }
        private List<Resource> ParseResources(string resourcesString)
        {
            var resources = new List<Resource>();
            if (!string.IsNullOrWhiteSpace(resourcesString))
            {
                try
                {
                    resources = JsonSerializer.Deserialize<List<Resource>>(resourcesString);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing resources: {ex.Message}");
                }
            }
            return resources;
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
