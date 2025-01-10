namespace IO.Modules.MapLibrary
{
    public struct Coordinates
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public override string ToString()
        {
            return $"{Latitude}, {Longitude}";
        }
    }
}
