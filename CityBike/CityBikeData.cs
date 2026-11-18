namespace CityBike;

public class CityBikeData
{
    public List<BikeRide> Rides = new();

    public Dictionary<int, Station> Stations = new();

    public Dictionary<int, Bike> Bikes = new();
}