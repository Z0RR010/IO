namespace IO.Modules.MapLibrary {
    
    public class MapController
    {
        
        private readonly GoogleMapsClient client;
        public readonly string id;
        
        public string Address { get; set; } = "";
        
        public MapController(GoogleMapsClient client, string id)
        {
            this.client = client;
            this.id = id;
        }

        public async Task DrawMap()
        {
            try
            {
                var center = await client.GetCoordinates(Address);
                await client.DrawMap(this, center);
                await client.AddMarker(this, center, Address);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    
}
