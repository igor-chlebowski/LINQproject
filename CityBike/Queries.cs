using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CityBike;

public static class Queries
{
    // Stack overflow: https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function
    private static double Haversine(double lat1, double lat2, double lon1, double lon2)
    {
        const double r = 6371000; // meters

        lat1 *= Math.PI / 180;
        lat2 *= Math.PI / 180;
        lon1 *= Math.PI / 180;
        lon2 *= Math.PI / 180;

        var sdlat = Math.Sin((lat2 - lat1) / 2);
        var sdlon = Math.Sin((lon2 - lon1) / 2);
        var q = sdlat * sdlat + Math.Cos(lat1) * Math.Cos(lat2) * sdlon * sdlon;
        var d = 2 * r * Math.Asin(Math.Sqrt(q));

        return d;
    }

    //  1.  Średnia długość przejazdu dla mężczyzny, kobietu i unknown.
    public static void AverageTripDurationByGender(CityBikeData dataBase)
    {
        var result = dataBase.Rides
            .GroupBy(ride => ride.Gender)
            .Select(group => new
            {
                gender = group.Key,
                avgTripDur = group.Average(ride => ride.TripDuration)
            });

        foreach (var item in result)
        {
            Console.WriteLine($"{item.gender}: {item.avgTripDur / 60.0:F2} min");
        }
    }

    //  2.  Porównanie Częstotliwości przejazdów Subscriber vs Customer
    //      pod względem dni roboczych i weekendów.
    public static void SubscriversVsCustomers(CityBikeData dataBase)
    {
        var weekDays = dataBase.Rides
            .Where(ride => ride.StartTime.DayOfWeek != DayOfWeek.Saturday && ride.StartTime.DayOfWeek != DayOfWeek.Sunday)
            .GroupBy(ride => ride.UserType)
            .Select(group => new
            {
                UserType = group.Key,
                Count = group.Count()
            });

        var weekendDays = dataBase.Rides
            .Where(ride => ride.StartTime.DayOfWeek == DayOfWeek.Saturday || ride.StartTime.DayOfWeek == DayOfWeek.Sunday)
            .GroupBy(ride => ride.UserType)
            .Select(group => new
            {
                UserType = group.Key,
                Count = group.Count()
            });

        int totalWeekRides = weekDays.Sum(x => x.Count);
        int totalWeekendRides = weekendDays.Sum(x => x.Count);

        Console.Write("Weekdays: ");
        foreach(var item in weekDays)
        {
            Console.Write($"{item.UserType}: {(double)item.Count / totalWeekRides * 100:F2}% ");
        }
        Console.WriteLine();
        Console.Write("Weekends: ");
        foreach (var item in weekendDays)
        {
            Console.Write($"{item.UserType}: {(double)item.Count / totalWeekendRides * 100:F2}% ");
        }
        Console.WriteLine();
    }

    //  3.  10 najczęstrzych tras na których jeździli ludzie przez cały rok.
    public static void TenMostPopularRoutes(CityBikeData dataBase)
    {
        var result = dataBase.Rides
            .GroupBy(ride => (ride.StartStationId, ride.EndStationId))
            .Select(group => new
            {
                StartStation = dataBase.Stations[group.Key.StartStationId],
                EndStation = dataBase.Stations[group.Key.EndStationId],
                Count = group.Count()
            })
            .OrderByDescending(group => group.Count)
            .Take(10);

        foreach (var item in result)
        {
            Console.WriteLine($"{item.StartStation.Name} -> {item.EndStation.Name} has been ridden {item.Count} times");
        }
    }

    //  4.  3 najczęściej użytkowane rowery w każym miesiącu wg. ilości 
    //      przejazdów, oraz łączna ilość godzin jaką jeździły.
    public static void MostPopularBikes(CityBikeData dataBase)
    {
        var result = dataBase.Rides
            .GroupBy(ride => (ride.BikeId, ride.StartTime.Month))
            .Select(group => new
            {
                Bike = group.Key.BikeId,
                RideMonth = group.Key.Month,
                Count = group.Count(),
                TotalTime = group.Sum(ride => ride.TripDuration)
            })
            .GroupBy(group => group.RideMonth)
            .Select(month => month
                        .OrderByDescending(x => x.Count)
                        .Take(3));
            
            
        foreach (var month in result)
        {
            foreach(var ride in month)
            {
                Console.WriteLine(
                    $"Month: {CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(ride.RideMonth)}, " +
                    $"Bike: {ride.Bike}, " +
                    $"Rides: {ride.Count}, " +
                    $"Hours: {(double)ride.TotalTime / 3600:F2}"
                );
            }
            Console.WriteLine();
        }
    }

    //  5.  3 Najdłuższe tras wg. wzoru Haversine'a i średni czas trwania każdego miesiąca.
    public static void LongestRoutes(CityBikeData dataBase)
    {
        var result = dataBase.Rides
            .GroupBy(ride => (ride.StartStationId, ride.EndStationId,
                                ride.StartTime.Month))
            .Select(group => 
            {
                Station startStation = dataBase.Stations[group.Key.StartStationId];
                Station endStation = dataBase.Stations[group.Key.EndStationId];

                return new
                {
                    Route = startStation.Name + " -> " + endStation.Name,
                    Distance = Haversine(startStation.Latitude, endStation.Latitude, startStation.Longitude, endStation.Longitude),
                    RideMonth = group.Key.Month,
                    AvgTime = group.Average(ride => ride.TripDuration)
                };

            })
            .GroupBy(route => route.RideMonth)
            .Select(month => new
            {
                Month = month.Key,
                Routes = month
                        .OrderByDescending(x => x.Distance)
                        .Take(3)
            });


        foreach (var month in result)
        {
            Console.WriteLine($"Furthest routes in {CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month.Month)}: ");
            foreach (var ride in month.Routes)
            {
                Console.WriteLine(
                    $"Route: {ride.Route}, " +
                    $"Distance: {ride.Distance/1000:F2} km, " +
                    $"Avg time: {(double)ride.AvgTime / 60:F2} min"
                );
            }
            Console.WriteLine();
        }
    }


    //  6.  3 Najbardziej pożądane stacje w każdym miesiącu (dużo wypożyczeń, 
    //      mało oddań). Test dla proporcji spełniający warunek np > 5 i nq > 5
    //      na poziomie istotności 0.05 zwracam najbardziej pożądane stacje wg. p
    public static void MostPopularStations(CityBikeData dataBase)
    {

        const double criticalValue = 1.64485;
        var rents = dataBase.Rides
            .GroupBy(ride => (ride.StartTime.Month, ride.StartStationId))
            .Select(ride => new
            {
                ride.Key.Month,
                StationId = ride.Key.StartStationId,
                RentCount = ride.Count()
            });

        var returns = dataBase.Rides
            .GroupBy(ride => (ride.StopTime.Month, ride.EndStationId))
            .Select(ride => new
            {
                ride.Key.Month,
                Station = ride.Key.EndStationId,
                ReturnCount = ride.Count()
            });

        var statsFiltered = rents.Join(
            returns,
            rent => (rent.Month, rent.StationId), ret => (ret.Month, ret.Station),
            (rent, ret) => new
            {
                rent.Month,
                rent.StationId,
                rent.RentCount,
                ret.ReturnCount,
                p = (double)rent.RentCount / (rent.RentCount + ret.ReturnCount),
                q = (double)ret.ReturnCount / (rent.RentCount + ret.ReturnCount),
                n = (double)rent.RentCount + ret.ReturnCount
            })
            .Select(stat => new
            {
                stat.Month,
                stat.StationId,
                stat.RentCount,
                stat.ReturnCount,
                stat.p,
                stat.q,
                stat.n,
                U = (stat.p - 0.5) / Math.Sqrt(0.25 / stat.n)
            })
            .Where(stat => (stat.n * stat.p >= 5 && stat.q * stat.n >= 5 && stat.U >= criticalValue))
            .GroupBy(group => group.Month)
            .Select(month => new
            {
                Month = month.Key,
                Stations = month
                    .OrderByDescending(month => month.p)
                    .Take(3)
            })
            .OrderBy(x => x.Month);

        foreach (var month in statsFiltered)
        {
            Console.WriteLine($"Month: {CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month.Month)}");

            foreach (var station in month.Stations)
            {
                Station stationData = dataBase.Stations[station.StationId];
                Console.WriteLine(
                    $"{stationData.Name}, " +
                    $"p = {station.p:F3}, " +
                    $"U = {station.U:F3}, " +
                    $"rentals = {station.RentCount}, " +
                    $"returns = {station.ReturnCount}, " +
                    $"total = {station.n}"
                );
            }

            Console.WriteLine();
        }



    }    
}
