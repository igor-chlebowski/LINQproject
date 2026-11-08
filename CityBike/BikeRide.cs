using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CityBike;

//"tripduration","starttime","stoptime","start station id","start station name","start station latitude","start station longitude","end station id","end station name","end station latitude","end station longitude","bikeid","usertype","birth year","gender"
//226,"2020-01-01 00:04:50.1920","2020-01-01 00:08:37.0370",3186,"Grove St PATH",40.71958611647166,-74.04311746358871,3211,"Newark Ave",40.72152515,-74.046304543,29444,"Subscriber",1984,2

public enum UserType
{
    Subscriber,
    Customer
}
public enum Gender
{
    Unknown = 0,
    Male = 1,
    Female = 2
}
public class BikeRide
{
    public int TripDuration;
    public DateTime StartTime;
    public DateTime StopTime;
    public int StartStationId;
    public string StartStationName = "";
    public double StartStationLatitude;
    public double StartStationLongitude;
    public int EndStationId;
    public string EndStationName = "";
    public double EndStationLatitude;
    public double EndStationLongitude;
    public int BikeId;
    public UserType UserType;
    public int BirthYear;
    public Gender Gender;

    public static BikeRide FromCsvLine(string line)
    {
        string[] data = line.Split(',');
        BikeRide bikeRide = new BikeRide
        {
            TripDuration = int.Parse(data[0]),
            StartTime = DateTime.Parse(data[1].Trim('"')),
            StopTime = DateTime.Parse(data[2].Trim('"')),
            StartStationId = int.Parse(data[3]),
            StartStationName = data[4].Trim('"'),
            StartStationLatitude = double.Parse(data[5], CultureInfo.InvariantCulture),
            StartStationLongitude = double.Parse(data[6], CultureInfo.InvariantCulture),
            EndStationId = int.Parse(data[7]),
            EndStationName = data[8].Trim('"'),
            EndStationLatitude = double.Parse(data[9], CultureInfo.InvariantCulture),
            EndStationLongitude = double.Parse(data[10], CultureInfo.InvariantCulture),
            BikeId = int.Parse(data[11]),
            UserType = Enum.Parse<UserType>(data[12].Trim('"')),
            BirthYear = int.Parse(data[13]),
            Gender = Enum.Parse<Gender>(data[14])
        };


        return bikeRide;
    }
}
