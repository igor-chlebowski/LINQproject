using System;
using System.Collections.Generic;
using System.Text;

namespace CityBike;

public static class CsvReader
{
    public static List<BikeRide> LoadRides(string directory)
    {
        string[] filesPaths = Directory.GetFiles(directory);
        List<BikeRide> rides = new List<BikeRide>();
        foreach(string path in filesPaths)
        {
            foreach (string line in File.ReadLines(path).Skip(1))
            {
                BikeRide ride = BikeRide.FromCsvLine(line);
                rides.Add(ride);
            }
        }
        return rides;
    }
}
