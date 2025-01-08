namespace IO.Modules.MapLibrary {
    
    public class MapController
    {
        
        private readonly GoogleMapsClient client;
        public readonly string id;
        
        public Address Address { get; set; }
        
        public MapController(GoogleMapsClient client, string id)
        {
            this.client = client;
            this.id = id;
        }

        public async Task DrawMap()
        {
            // TODO: metoda ToString() w klasie Address???
            string addressString = $"{Address.Street} {Address.StreetNumber}, {Address.Apartment}, {Address.City}, {Address.ZipCode}"
                .Replace(", ,", ",").Trim(',').Trim();

            try
            {
                var center = await client.GetCoordinates(addressString);
                await client.DrawMap(this, center);
                await client.AddMarker(this, center, addressString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    
}
