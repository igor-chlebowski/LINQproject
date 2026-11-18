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
    public int EndStationId;
    public int BikeId;
    public UserType UserType;
    public int BirthYear;
    public Gender Gender;
}
