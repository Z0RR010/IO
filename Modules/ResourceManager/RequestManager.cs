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
            string resourcesJson = JsonSerializer.Serialize(request.ResourcesRequired);

            string query =
                "INSERT INTO Request(Title, Description, CreatedAt, DateUpdated, Status, User, Address, ResourcesRequired, IsVerified) VALUES(@title, @description, @createdAt, @dateUpdated, @status, @user, @address, @resRequired, @isVerified)";
            try
            {
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@title", request.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@description", request.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@createdAt", request.CreatedAt);
                    command.Parameters.AddWithValue("@dateUpdated", request.DateUpdated ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@status", request.Status.ToString());
                    command.Parameters.AddWithValue("@user", request.User ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@address", JsonSerializer.Serialize(request.Address) ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@resRequired", resourcesJson ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@isVerified", request.IsVerified ? 1 : 0);

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

        public async Task<bool> RemoveRequestFromDatabase(int id)
        {
            string query = "DELETE FROM Request WHERE Id = @id";
            try
            {
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
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

        public List<Request> GetUserRequests(string email)
        {
            string query = "SELECT * FROM Request WHERE [User] = @user";
            var requests = new List<Request>();

            try
            {
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@user", email);

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
                            };

                            string resourcesString = reader.GetString(reader.GetOrdinal("ResourcesRequired"));
                            request.ResourcesRequired = ParseResources(resourcesString);

                            requests.Add(request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserRequests: {ex.Message}");
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


        public bool UpdateRequestStatus(int id, RequestStatus newStatus)
        {
            string query =
                "UPDATE Request SET Status = @status WHERE Id = @id";
            try
            {
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@status", newStatus.ToString());
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

        public async Task<bool> UpdateRequest(Request updatedRequest)
        {
            string query = @"
        UPDATE Request 
        SET 
            Title = @title, 
            Description = @description, 
            DateUpdated = @dateUpdated, 
            Status = @status, 
            User = @user, 
            Address = @address, 
            ResourcesRequired = @resourcesRequired, 
            IsVerified = @isVerified
        WHERE 
            Id = @id";

            try
            {
                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", updatedRequest.Id);
                    command.Parameters.AddWithValue("@title", updatedRequest.Title ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@description", updatedRequest.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@dateUpdated", DateTime.Now);
                    command.Parameters.AddWithValue("@status", updatedRequest.Status.ToString());
                    command.Parameters.AddWithValue("@user", updatedRequest.User ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@address", JsonSerializer.Serialize(updatedRequest.Address) ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@resourcesRequired", JsonSerializer.Serialize(updatedRequest.ResourcesRequired) ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@isVerified", updatedRequest.IsVerified ? 1 : 0);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in UpdateRequest: {e.Message}");
                return false;
            }
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
