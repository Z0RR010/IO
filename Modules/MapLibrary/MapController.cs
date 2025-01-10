namespace IO.Modules.MapLibrary {
    
    public class MapController
    {
        
        private readonly GoogleMapsClient client;
        public readonly string id;
        
        private LocationWrapper _location;
        
        public MapController(GoogleMapsClient client, string id)
        {
            this.client = client;
            this.id = id;
        }

        public async Task DrawMap()
        {
            try
            {
                var center = await _location.GetCoordinatesAwait(client);
                await client.DrawMap(this, center);
                await client.AddMarker(this, center, _location.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        
        public void SetLocation(Address address)
        {
            _location = new AddressWrapper(address);
        }
        
        public void SetLocation(string address)
        {
            _location = new AddressStringWrapper(address);
        }
        
        public void SetLocation(Coordinates coordinates)
        {
            _location = new CoordinatesWrapper(coordinates);
        }
    }
    
}
