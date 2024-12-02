using Microsoft.JSInterop;

namespace IO.Modules.MapLibrary
{
    public class GoogleMapsClient
    {
        private readonly IJSRuntime _jsRuntime;
        
        public GoogleMapsClient(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        
        public async Task<Coordinates> GetCoordinates(string address)
        {
            Console.WriteLine($"Geocoding address: {address}");
            return await _jsRuntime.InvokeAsync<Coordinates>("geocode", address);
        }
        
        public async Task DrawMap(MapController map, Coordinates center)
        {
            Console.WriteLine($"Drawing map with center at {center.Latitude}, {center.Longitude}");
            await _jsRuntime.InvokeVoidAsync("initMap", map.id, center.Latitude, center.Longitude);
        }
        
        public async Task AddMarker(MapController map, Coordinates position, string title)
        {
            Console.WriteLine($"Adding marker at {position.Latitude}, {position.Longitude}");
            await _jsRuntime.InvokeVoidAsync("addMarker", map.id, position.Latitude, position.Longitude, title);
        }
        
    }
}
