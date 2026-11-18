using System.Globalization;

namespace CityBike;

public static class CsvReader
{
    public static CityBikeData LoadData(string directory)
    {
        CityBikeData dataBase = new CityBikeData();

        string[] filesPaths = Directory.GetFiles(directory);

        foreach (string path in filesPaths)
        {
            foreach (string line in File.ReadLines(path).Skip(1))
            {
                string[] data = line.Split(',');

                int startStationId = int.Parse(data[3]);
                int endStationId = int.Parse(data[7]);
                int bikeId = int.Parse(data[11]);

                if (!dataBase.Stations.ContainsKey(startStationId))
                {
                    dataBase.Stations[startStationId] = new Station
                    {
                        Id = startStationId,
                        Name = data[4].Trim('"'),
                        Latitude = double.Parse(data[5], CultureInfo.InvariantCulture),
                        Longitude = double.Parse(data[6], CultureInfo.InvariantCulture)
                    };
                }

                if (!dataBase.Stations.ContainsKey(endStationId))
                {
                    dataBase.Stations[endStationId] = new Station
                    {
                        Id = endStationId,
                        Name = data[8].Trim('"'),
                        Latitude = double.Parse(data[9], CultureInfo.InvariantCulture),
                        Longitude = double.Parse(data[10], CultureInfo.InvariantCulture)
                    };
                }

                if (!dataBase.Bikes.ContainsKey(bikeId))
                {
                    dataBase.Bikes[bikeId] = new Bike
                    {
                        Id = bikeId
                    };
                }

                BikeRide bikeRide = new BikeRide
                {
                    TripDuration = int.Parse(data[0]),
                    StartTime = DateTime.Parse(data[1].Trim('"')),
                    StopTime = DateTime.Parse(data[2].Trim('"')),
                    StartStationId = startStationId,
                    EndStationId = endStationId,
                    BikeId = bikeId,
                    UserType = Enum.Parse<UserType>(data[12].Trim('"')),
                    BirthYear = int.Parse(data[13]),
                    Gender = Enum.Parse<Gender>(data[14])
                };

                dataBase.Rides.Add(bikeRide);
            }
        }

        return dataBase;
    }
}