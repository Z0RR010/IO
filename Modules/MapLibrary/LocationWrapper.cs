namespace IO.Modules.MapLibrary;

internal abstract record LocationWrapper
{
    internal abstract Task<Coordinates> GetCoordinatesAwait(GoogleMapsClient client);
    
    internal abstract string Name { get; }
}


internal record AddressWrapper(Address Address) : LocationWrapper
{
    internal override async Task<Coordinates> GetCoordinatesAwait(GoogleMapsClient client)
    {
        var addressString = Name;
        return await client.GetCoordinates(addressString);
    }

    internal override string Name =>
        $"{Address.Street} {Address.StreetNumber}, {Address.Apartment}, {Address.City}, {Address.ZipCode}"
            .Replace(", ,", ",").Trim(',').Trim();
}

internal record AddressStringWrapper(string AddressString) : LocationWrapper
{
    internal override async Task<Coordinates> GetCoordinatesAwait(GoogleMapsClient client)
    {
        return await client.GetCoordinates(Name);
    }

    internal override string Name => AddressString;
}

internal record CoordinatesWrapper(Coordinates Coordinates) : LocationWrapper
{
    internal override Task<Coordinates> GetCoordinatesAwait(GoogleMapsClient client)
    {
        return Task.FromResult(Coordinates);
    }

    internal override string Name => Coordinates.ToString();
}