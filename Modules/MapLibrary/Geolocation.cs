namespace IO.Modules.MapLibrary;

using Microsoft.JSInterop;


public class Geolocation
{
    private readonly IJSRuntime _jsRuntime;
    
    public Geolocation(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<Coordinates?> GetUserLocation()
    {
        await using var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./scripts/geolocation.js");

        try
        {
            return await module.InvokeAsync<Coordinates>("getUserLocation");
        }
        catch (JSException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
}