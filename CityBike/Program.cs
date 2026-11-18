namespace CityBike;

public class Program
{
    public static string StrPad(string text, int width, char padChar)
    {
        int padding = (width + text.Length) / 2;
        return text.PadLeft(padding, padChar).PadRight(width, padChar);
    }
    
    static void Main()
    {
        Console.WriteLine(StrPad("Analiza przejazdów rowerami CityBike w mieście Jersey City w 2020 roku.", 100, '='));
        Directory.SetCurrentDirectory("../../../");
        //string path = "Data/JC-202001-citibike-tripdata.csv";

        CityBikeData dataBase = CsvReader.LoadData("Data");
        List<BikeRide> rides = dataBase.Rides;

        Console.WriteLine();
        Console.WriteLine($"Wczytano {rides.Count} przejazdów");
        Console.WriteLine();

        Console.WriteLine("1. Średnia długość przejazdu z podziałem na płeć:");
        Console.WriteLine();
        Queries.AverageTripDurationByGender(dataBase);
        Console.WriteLine();

        Console.WriteLine("2. Procentowy udział przejazdów subskrybentów i jednorazowych klientów w weekendy i dni robocze:");
        Console.WriteLine();
        Queries.SubscriversVsCustomers(dataBase);
        Console.WriteLine();

        Console.WriteLine("3. Najpopularniejsze 10 tras w całym roku:");
        Console.WriteLine();
        Queries.TenMostPopularRoutes(dataBase);
        Console.WriteLine();

        Console.WriteLine("4. Najpopularniejsze rowery w każdym miesiącu i łączny czas przez jaki jeździły:");
        Console.WriteLine();
        Queries.MostPopularBikes(dataBase);
        Console.WriteLine();

        Console.WriteLine("5. Najdłuższe 3 trasy wg. wzoru Haversine i ich średni czas przejazdu w każdym miesiącu");
        Console.WriteLine();
        Queries.LongestRoutes(dataBase);
        Console.WriteLine();

        Console.WriteLine("6. Najpopularniejsze stacje wg. testu czy zmienna losowa (X - wypożyczenie roweru z danej stacji)");
        Console.WriteLine("   ma rozkład dwupuntowy, hipoteza alternatywna to p > p0 = 0.5. Dodatkowe założenia to pn >= 5 i qn >= 5");
        Console.WriteLine("   hipotezę weryfikuje na poziomie istotności 0.05, z tablicy kwartyli rozkładu normalnego zbiór krytyczny");
        Console.WriteLine("   wychodzi W = [1.64485, +nieskończoność]");
        Console.WriteLine();
        Queries.MostPopularStations(dataBase);
        Console.WriteLine();
    }
}
