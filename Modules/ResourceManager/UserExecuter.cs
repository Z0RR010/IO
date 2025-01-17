using System.Data;
using System.Text;
using IO.Modules.Security;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IO.Modules.ResourceManager
{
    /// <summary>
    /// Class for managing user accounts
    /// </summary>
    public class UserExecuter : IUserManager, IDisposable, IAsyncDisposable
    {
        //private readonly MySqlConnection _userConnection;
        private readonly SqliteConnection _userConnection;

        //If connection is not established try downloading mySQL server so far
        public UserExecuter()
        {
            try
            {
                _userConnection =
                    //new MySqlConnection("Server=localhost;Port=3306;Database=userDatabase;User Id=root;Password=root;");
                    new SqliteConnection(
                        "Data Source=./databases/userDatabase.db;Cache=Shared");
                _userConnection.Open();
                //Diagnostics stuff
                Console.WriteLine("Connection to " + _userConnection.ConnectionString + " established");
                
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get the password of the concrete user
        /// </summary>
        /// <param name="email">an email of requested user</param>
        /// <returns>String containing password. Otherwise, empty string</returns>
        private string GetPasswordFromDataBase(string email)
        {
            string query = "SELECT password FROM users WHERE email = @email";

            using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
            {
                command.Parameters.AddWithValue("@email", email);

                string input = command.ExecuteScalar()?.ToString();

                if (input != null)
                {
                    return input;
                }

                return "";
            }
        }

        /// <summary>
        /// Looks for user and checks if it's stored in user database
        /// </summary>
        /// <param name="email">Email of requested user</param>
        /// <returns>True if user was found in database. Otherwise, false</returns>
        public bool IsUserInDataBase(string email)
        {
            string query = "SELECT * FROM users WHERE email = @email";

            using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
            {
                command.Parameters.AddWithValue("@email", email);
                return command.ExecuteScalar()?.ToString() != null;
            }

            // connection closes automatically as this class implements IDisposable interface
        }


		/// <summary>
		/// Get a concrete individual user from database as only they have PESEL
		/// </summary>
		/// <param name="email">Email of requested user</param>
		/// <returns>User of type Individual if found in DB. Otherwise, NULL</returns>
		public BasicUser GetBasicUser(string email)
		{
			string query = "SELECT * FROM users WHERE email = @email";
			using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
            {
                command.Parameters.AddWithValue("@email", email);
                using var reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var readEmail = reader.GetString(0);
                        var readJSON = reader.GetString(1);
                        var readEncryptionKey = reader.GetString(2);
                        var readPassword = reader.GetString(3);
                        var readEmailVerified = reader.GetBoolean(4);
                        var readToken = reader.GetString(5);
                        var readRole = reader.GetString(6);
                        string readWebsite = "";
                        if(!reader.IsDBNull(7)) readWebsite = reader.GetString(7);
                        string readKrs = "";
						if (!reader.IsDBNull(8)) readKrs = reader.GetString(8);
                        string readInstitution = "";
						if (!reader.IsDBNull(9)) readInstitution = reader.GetString(9);

                        Individual individual = JsonSerializer.Deserialize<Individual>(readJSON);

                        BasicUser basicUser = new BasicUser(readEmail,
                                                           individual.Name,
                                                           individual.PhoneNumber,
                                                           individual.Address,
                                                           individual.IsVerified,
                                                           individual.Surname,
                                                           individual.Pesel,
                                                           readInstitution,
                                                           readWebsite,
                                                           readKrs,
                                                           readRole);

                        return basicUser;
					}
                }
			}

            return null;
		}
        
        public Individual GetUserFromDataBase(string email)
        {
            if (IsUserInDataBase(email))
            {
                string query = "SELECT user FROM users WHERE email = @email";

                using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    var input = command.ExecuteScalar()?.ToString();
                    if (input != null)
                    {
                        return JsonSerializer.Deserialize<Individual>(input);
                    }

                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Add user to data base
        /// </summary>
        /// <param name="user">Object of type User. It'll be serialised in JSON and sent</param>
        /// <param name="encryptionKey">Key for security purposes</param>
        /// <param name="password">Password of the new user</param>>
        /// <param name="token">Token that is supposed to be stored</param>>
        /// <returns>True if operation was successful. Otherwise, false</returns>
        public bool SendToDataBase(Individual user, string encryptionKey, string password, string token, string role, string website, string krs, string institution)
        {
            //FIXME
            // var options = new JsonSerializerOptions
            // {
            //     WriteIndented = true,
            //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // }

            var packedUser = JsonSerializer.Serialize(user);
            string query =
                "INSERT INTO users(email, user, encryptionKey, password, emailVerified, token, role, website, krs, institution) VALUES(@email, @packedUser, @encryptionKey, @password, @emailVerified, @token, @role, @website, @krs, @institution)";
            try
            {
                using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
                {
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@packedUser", packedUser);
                    command.Parameters.AddWithValue("@encryptionKey", encryptionKey);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@emailVerified", false);
                    command.Parameters.AddWithValue("@token", token);
                    command.Parameters.AddWithValue("@role", role);
                    command.Parameters.AddWithValue("@website", website);
					command.Parameters.AddWithValue("@krs", krs);
					command.Parameters.AddWithValue("@institution", institution);

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

        /// <summary>
        /// Checks the validality of password
        /// </summary>
        /// <param name="email">Email of requested user</param>
        /// <param name="password">Provided password</param>
        /// <returns>True if password is valid. Otherwise, false</returns>
        public bool IsPasswordCorrect(string email, string password)
        {
            return password.Equals(GetPasswordFromDataBase(email));
        }

        /// <summary>
        /// Get user PESEL (individual number) providing their Email
        /// </summary>
        /// <param name="email">Email of user</param>
        /// <returns>String PESEL of user if it's in database or is individual person. Otherwise, empty string</returns>
        public string GetUserPESEL(string email)
        {
            User user = GetUserFromDataBase(email);
            if (user != null && user.GetType().IsSubclassOf(typeof(User)))
            {
                Individual output = (Individual)user;
                return output.Pesel;
            }

            return "";
        }

        /// <summary>
        /// Make your own query if you dislike our functions
        /// </summary>
        /// <param name="query">Command</param>
        /// <returns>Result of command execution</returns>
        public string CustomQuery(string query)
        {
            if (query.ToLower().Contains("DROP DATABASE".ToLower()) || query.ToLower().Contains("DROP TABLE".ToLower()))
            {
                throw new Exception("Are you dumb? What are you trying to do?");
            }

            try
            {
                using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
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

        /// <summary>
        /// Gain encryption key of a user by given email
        /// </summary>
        /// <param name="email">Email of requested user</param>
        /// <returns>String containing encryption key (AES, whatever)</returns>
        public string GetEncryptionKey(string email)
        {
            if (IsUserInDataBase(email))
            {
                string query = "SELECT encryptionKey FROM users WHERE email = @email";

                using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    string input = command.ExecuteScalar()?.ToString();

                    return input;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gain token of a user by given email
        /// </summary>
        /// <param name="email">Email of requested user</param>
        /// <returns>String containing token</returns>
        public string GetToken(string email)
        {
            if (IsUserInDataBase(email))
            {
                string query = "SELECT token FROM users WHERE email = @email";

                using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
                {
                    command.Parameters.AddWithValue("@email", email);

                    string input = command.ExecuteScalar()?.ToString();

                    return input;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Updates user's email verification status
        /// </summary>
        /// <param name="email">Email of requested user</param>
        /// <param name="value">New status of email verification</param>
        /// <returns>True if operation was successful. Otherwise, false</returns>
        public bool UpdateEmailVerified(string email, bool value)
        {
            string query = "UPDATE users SET emailVerified = @value WHERE email = @email";
            try
            {
                using (var command = new SqliteCommand(query, _userConnection)) //MySqlCommand(query, _userConnection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@value", value);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _userConnection.Close();
            _userConnection.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _userConnection.DisposeAsync();
        }
    }
}