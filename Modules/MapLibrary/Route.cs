namespace IO.Modules.MapLibrary
{
	public class Route
	{

		private List<LocationWrapper> _locations;

		internal Route(LocationWrapper startPoint, LocationWrapper endPoint)
		{
			_locations = new List<LocationWrapper>{startPoint, endPoint};

		}

        internal void AddRouteLocation(LocationWrapper location)
        {
            _locations.Insert(_locations.Count - 1, location);
        }


        public async Task<List<Coordinates>> GenerateCoordinatesList(GoogleMapsClient client)
        {
            var coordinatesList = new List<Coordinates>();

            foreach (var location in _locations)
            {
                var coordinates = await location.GetCoordinatesAwait(client);
                coordinatesList.Add(coordinates);
            }

            return coordinatesList;
        }

    }
}
