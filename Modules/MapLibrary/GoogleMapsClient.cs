using System.Globalization;
using Microsoft.JSInterop;

namespace IO.Modules.MapLibrary
{
    public class GoogleMapsClient
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly string _apiKey;
        private IJSObjectReference _module;
        
        public GoogleMapsClient(IJSRuntime jsRuntime, string apiKey)
        {
            _jsRuntime = jsRuntime;
            _apiKey = apiKey;
        }

        public async Task InitializeApi(Dictionary<string, object> options)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/maps.js");
            options.Add("key", _apiKey);
            options.Add("v", "weekly");
            options.Add("language", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            await _module.InvokeVoidAsync("loadGoogleMapsAPI", options);
        }
        
        public async Task InitializeApi()
        {
            await InitializeApi(new Dictionary<string, object>());
        }
        
        public async Task<Coordinates> GetCoordinates(string address)
        {
            Console.WriteLine($"Geocoding address: {address}");
            return await _module.InvokeAsync<Coordinates>("geocode", address);
        }
        
        public async Task DrawMap(MapController map, Coordinates center)
        {
            Console.WriteLine($"Drawing map with center at {center.Latitude}, {center.Longitude}");
            await _module.InvokeVoidAsync("initMap", map.id, center.Latitude, center.Longitude);
        }
        
        public async Task AddMarker(MapController map, Coordinates position, string title)
        {
            Console.WriteLine($"Adding marker at {position.Latitude}, {position.Longitude}");
            await _module.InvokeVoidAsync("addMarker", map.id, position.Latitude, position.Longitude, title);
        }

        public async Task DrawRoute(MapController map)
        {
            Console.WriteLine("Drawing route on map.");
            var routeCoords = await map.GetRouteCoordinates();

            var points = routeCoords.Select(c => new { lat = c.Latitude, lng = c.Longitude }).ToArray();
            try
            {
                await _module.InvokeVoidAsync("drawRoute", map.id, points, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            }
            catch (Exception error)
            {
                Console.Write(error.Message);
            }
        }

    }
}
