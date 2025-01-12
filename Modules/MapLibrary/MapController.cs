namespace IO.Modules.MapLibrary {
    
    public class MapController
    {
        
        private readonly GoogleMapsClient client;
        public readonly string id;
        
        private LocationWrapper _location;
        private Route _route;
        
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
                if (_route != null)
                {
                    await client.DrawRoute(this);
                }
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

        public void SetRoute(Route route)
        {
            _route = route;
        }

        public void AddRoutePoint(Coordinates point)
        {
            if (_route == null)
            {
                throw new InvalidOperationException("Route is not set. Use SetRoute() to initialize the route.");
            }

            _route.AddRoutePoint(point);
        }
        public async Task<List<Coordinates>> GetRouteCoordinates()
        {
            if (_route == null)
            {
                throw new InvalidOperationException("Route is not set.");
            }

            return await _route.GenerateCoordinatesList(client);
        }
    }
    
}
