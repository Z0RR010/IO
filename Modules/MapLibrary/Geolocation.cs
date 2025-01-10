namespace IO.Modules.MapLibrary;

using Microsoft.JSInterop;


public class Geolocation
{
    private readonly IJSRuntime _jsRuntime;
    
    public Geolocation(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<Coordinates> GetUserLocation()
    {
        await using var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/geolocation.js");
        
        return await module.InvokeAsync<Coordinates>("getUserLocation");
    }
}