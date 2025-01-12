namespace IO.Modules.MapLibrary
{
	public class Route
	{

		private List<LocationWrapper> _locations;
		private List<Coordinates> coordinates;

		internal Route(LocationWrapper startPoint, LocationWrapper endPoint)
		{
			_locations = new List<LocationWrapper>{startPoint, endPoint};
			coordinates = new List<Coordinates>();
		}

		public void AddRoutePoint(Coordinates point)
		{
			coordinates.Insert(coordinates.Count-1, point);
		}

        public List<Coordinates> GetCoordinates()
        {
            return coordinates;
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
