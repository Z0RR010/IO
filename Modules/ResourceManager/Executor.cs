namespace IO.Modules.ResourceManager
{
    public class Executor : IResManager
    {
        private readonly ResourceDataBaseHandler _handler;

        public Executor()
        {
            _handler = new ResourceDataBaseHandler(
                "Data Source=./databases/resourceBase.db;Version=3;FailIfMissing=True;");
        }

        private Resource CreateResource(string name, Category category, int amount)
        {
            return new Resource(name, category, amount);
        }

        /// <summary>
        /// Add new resource item to database
        /// </summary>
        /// <param name="name">The name of resource to add</param>
        /// <param name="category">Category of the resource</param>
        /// <param name="amount">The amount of the resources</param>
        /// <returns>True if the operation was successful. Otherwise, false</returns>
        public bool AddItems(string name, string category, int amount)
        {
            Category output;
            // Robimy switch-case niewrażliwym przez registr podanego stringa
            switch (category.ToLower())
            {
                case "food": output = Category.Food; break;
                case "water": output = Category.Water; break;
                case "clothing": output = Category.Clothing; break;
                case "money": output = Category.Money; break;
                case "transport": output = Category.Transport; break;
                default: throw new AggregateException("Unknown category");
            }

            return _handler.AddItem(CreateResource(name, output, amount));
        }

        /// <summary>
        /// Gain items and decrease their amount based on the amount taken
        /// </summary>
        /// <param name="name">The name of the requested resource</param>
        /// <param name="amount">The wanted amount to be taken</param>
        /// <returns>True string info operation was successful. Otherwise, null</returns>
        public string GetItem(string name, int amount)
        {
            return _handler.GetItem(name, amount).ToString();
        }

        /// <summary>
        /// Get amount of items of concrete category
        /// </summary>
        /// <param name="category">The category to be found</param>
        /// <returns>Size of list of all items ever found in database</returns>
        public int GetItemAmountByCategory(string category)
        {
            Category output;
            // Robimy switch-case niewrażliwym przez registr podanego stringa
            switch (category.ToLower())
            {
                case "food": output = Category.Food; break;
                case "water": output = Category.Water; break;
                case "clothing": output = Category.Clothing; break;
                case "money": output = Category.Money; break;
                case "transport": output = Category.Transport; break;
                default: throw new AggregateException("Unknown category");
            }

            return _handler.GetItemsByCategory(output);
        }

        /// <summary>
        /// Get string info about all resources stored in resource databse
        /// </summary>
        /// <returns>List of string containing info</returns>
        public List<string> GetAllAvailableItems()
        {
            List<string> output = new List<string>();

            foreach (Resource item in _handler.GetAllItems())
            {
                output.Add(item.Name + " - " + item.Category + " - " + item.Amount);
            }

            return output;
        }

        /// <summary>
        /// Changes status of provided item
        /// </summary>
        /// <param name="name">The name of required resource</param>
        /// <param name="newStatus">New status of the required resource</param>
        /// <returns>True if operation was successful. Otherwise, false</returns>
        public bool ChangeItemStatus(string name, bool newStatus)
        {
            return _handler.ChangeStatus(name, newStatus);
        }

        /// <summary>
        /// Changes amount of provided item
        /// </summary>
        /// <param name="name">The name of required resource</param>
        /// <param name="newAmount">New amount of the required resource</param>
        /// <returns>True if operation was successful. Otherwise, false</returns>
        public bool ChangeItemAmount(string name, int newAmount)
        {
            return _handler.SetNewAmount(name, newAmount);
        }

        /// <summary>
        /// Make your own query if you dislike our functions
        /// </summary>
        /// <param name="query">Command</param>
        /// <returns>Result of command execution</returns>
        public string CustomQuery(string query)
        {
            return _handler.CustomQuery(query);
        }
    }
}