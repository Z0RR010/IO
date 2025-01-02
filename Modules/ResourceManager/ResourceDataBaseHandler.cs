using System.Data.SQLite;
using System.Text;
using System.Text.Json;
<<<<<<< Updated upstream
using MySql.Data.MySqlClient;
=======
>>>>>>> Stashed changes

namespace ResourceManager;

/// <summary>
/// Class establishing connection to database (local so far).
/// Its methods return different necessary info and modifies DB if needed
/// </summary>
public class ResourceDataBaseHandler : IDisposable, IAsyncDisposable
{
    private readonly MySqlConnection _resourceConnection;

    public ResourceDataBaseHandler(string connectionString)
    {
        try
        {
            //If connection is not established try downloading mySQL server so far
            _resourceConnection = new MySqlConnection(connectionString);
            _resourceConnection.Open();
            //Diagnostics stuff
            Console.WriteLine(_resourceConnection.Database);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Adds item to database
    /// </summary>
    /// <param name="item"></param>
    /// <returns>True if operation was successful. Otherwise, false</returns>
    public bool AddItem(Resource item)
    {
        string query = "INSERT INTO resources(hashcode_id, resource) VALUES(@hashcode, @packedResource)";
        try
        {
            using (var command = new MySqlCommand(query, _resourceConnection))
            {
                command.Parameters.AddWithValue("@hashcode", item.GetHashCode());
                command.Parameters.AddWithValue("@packedResource", JsonSerializer.Serialize(item));
                Console.WriteLine("Added resource");
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
    /// Removes item from resource database
    /// </summary>
    /// <param name="item">Resource to be removed</param>
    /// <returns>True if operation was successful. Otherwise, false</returns>
    public bool RemoveFromDataBase(Resource item)
    {
        string query = "DELETE FROM resources WHERE hashcode_id = @hashcode";
        try
        {
            using (var command = new MySqlCommand(query, _resourceConnection))
            {
                command.Parameters.AddWithValue("@hashcode", item.GetHashCode());
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
    /// Changes status (avialability) of the concrete resource
    /// </summary>
    /// <param name="name">The name of the resource to modify</param>
    /// <param name="status">The new status of the resource to modify/param>
    /// <returns>True if operation was successful. Otherwise, false</returns>
    public bool ChangeStatus(string name, bool status)
    {
        List<Resource> items = GetAllItems();

        foreach (Resource res in items)
        {
            if (res.Name.Equals(name))
            {
                int hash = res.GetHashCode();
                res.Status = status;

                string query = "UPDATE resources SET resource = @resource WHERE hashcode_id = @hashcode";

                try
                {
                    using (var command = new MySqlCommand(query, _resourceConnection))
                    {
                        command.Parameters.AddWithValue("@hashcode", hash);
                        command.Parameters.AddWithValue("@resource", JsonSerializer.Serialize(res));

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
        }

        return false;
    }

    /// <summary>
    /// Sets the new amount of concrete resources
    /// </summary>
    /// <param name="name">The name of the resource to modify</param>
    /// <param name="newAmount">The new amount of resource to modify</param>
    /// <returns>True if operation was successful. Otherwise, false</returns>
    public bool SetNewAmount(string name, int newAmount)
    {
        List<Resource> items = GetAllItems();

        foreach (Resource res in items)
        {
            if (res.Name.Equals(name))
            {
                int hash = res.GetHashCode();
                res.Amount = newAmount;

                string query = "UPDATE resources SET resource = @resource WHERE hashcode_id = @hashcode";

                try
                {
                    using (var command = new MySqlCommand(query, _resourceConnection))
                    {
                        command.Parameters.AddWithValue("@hashcode", hash);
                        command.Parameters.AddWithValue("@resource", JsonSerializer.Serialize(res));
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
        }

        return false;
    }

    /// <summary>
    /// Gain items and decrease their amount based on the amount taken
    /// </summary>
    /// <param name="name">The name of the requested resource</param>
    /// <param name="amount">The wanted amount to be taken</param>
    /// <returns>True resource class object operation was successful. Otherwise, null</returns>
    public Resource GetItem(string name, int amount)
    {
        List<Resource> items = GetAllItems();

        foreach (Resource res in items)
        {
            if (res.Name.Equals(name) && res.Amount - amount >= 0)
            {
                int hash = res.GetHashCode();

                res.Amount -= amount;

                string query = "UPDATE resources SET resource = @newResAmount WHERE hashcode_id = @hashcode";
                try
                {
                    using (var command = new MySqlCommand(query, _resourceConnection))
                    {
                        command.Parameters.AddWithValue("@newResAmount", JsonSerializer.Serialize(res));
                        command.Parameters.AddWithValue("@hashcode", hash);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0 ? res : null;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Get amount of items of concrete category
    /// </summary>
    /// <param name="category">The category to be found</param>
    /// <returns>Size of list of all items ever found in database</returns>
    public int GetItemsByCategory(Category category)
    {
        List<Resource> items = GetAllItems();

        int count = 0;

        foreach (Resource res in items)
        {
            if (res.Category == category)
            {
                count += res.Amount;
            }
        }

        return count;
    }

    public void Dispose()
    {
        _resourceConnection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceConnection.DisposeAsync();
    }

    /// <summary>
    /// Get all the item in resource database
    /// </summary>
    /// <returns>List of objects Resource</returns>
    public List<Resource> GetAllItems()
    {
        string query = "SELECT * FROM resources";
        List<Resource> items = new List<Resource>();

        using (var command = new MySqlCommand(query, _resourceConnection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var item = JsonSerializer.Deserialize<Resource>(reader.GetString(1));

                    items.Add(item);
                }
            }
        }

        return items;
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
            using (var command = new MySqlCommand(query, _resourceConnection))
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
}