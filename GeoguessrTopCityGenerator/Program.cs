using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoguessrTopCityGenerator
{

    class CustomCoordinate
    {
        public CustomCoordinate(double lattitude, double longitude, string countryName, string cityName, int populationOrder)
        {
            this.lat = lattitude;
            this.lng = longitude;

            //tags
            List<string> tags = new List<string>();
            tags.Add(countryName);
            //tags.Add(cityName);
            tags.Add(Convert.ToString(populationOrder));
            Dictionary<string, List<string>> extrasContents = new Dictionary<string, List<string>>();
            extrasContents.Add("tags", tags);
            this.extra = extrasContents;

            //defaults, unspecified need to be left null.
            heading = 0;
            pitch = 0;
            zoom = 0;
        }

        // Unique
        [JsonInclude]
        public double lat;
        [JsonInclude]
        public double lng;


        // Set to defaults
        [JsonInclude]
        public double heading;
        [JsonInclude]
        public double pitch;
        [JsonInclude]
        public double zoom;
        [JsonInclude]
        public Nullable<int> panoId;
        [JsonInclude]
        public Nullable<int> countryCode;
        [JsonInclude]
        public Nullable<int> stateCode;

        // Unique
        [JsonInclude]
        public Dictionary<string, List<string>> extra;

    }

    internal class Program
    {
        static string citiesFileName = "\\worldcities.csv";
        static string countriesFileName = "\\validcountries.txt";
        static string coordinatesFileName = "\\coordinates.json";

        static void Main(string[] args)
        {
            // Change to getDevFilePath() if testing within visual studio and getAppFilePath() for compilation.
            string filePath = getAppFilePath();
            Console.WriteLine("Welcome! This program will generate a json file for importing into map-making.app that will contain the most populous cities of every geoguessr valid country. Make sure to put your csv file into the same directory as this program.");
            Console.WriteLine("How many cities per country do you want to generate?");
            int cityCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("What is the minimum population a city can have for it to be included? (Set to -1 to include all cities, including those with no population listed)");
            int minPopulation = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Generating " + cityCount + " cities for each country...");
            generateMapJson(filePath, cityCount, minPopulation);
            Console.WriteLine("Generation is complete, the json will now be in the application's directory.");
        }

        static private async void generateMapJson(string filePath, int cityCount, int minPopulation)
        {
            Dictionary<string, int> remainingCitiesPerCountry = generateCityCountDictionary(filePath, cityCount);
            List<CustomCoordinate> coordinates = new List<CustomCoordinate>();
            // Operates under the assumption that the CSV file is under the same format as the one from https://simplemaps.com/data/world-cities. This sheet is already sorted by population.
            bool pastFirstLine = false;
            foreach (string line in File.ReadLines(filePath + citiesFileName))
            {
                if (pastFirstLine)
                {
                    string[] cityInfo = line.Split(',');
                    Console.WriteLine(cityInfo[0] + " / " + cityInfo[1] + " / " + cityInfo[2] + " / " + cityInfo[3] + " / " + cityInfo[8] + cityInfo[9]);
                    string cityName = cityInfo[1]; // Get's cities ascii name for tags
                    string countryName = cityInfo[4];
                    // Remove quotation marks and convert to double
                    double cityLattitude = Convert.ToDouble(cityInfo[2]); //cityInfo[2].Substring(1, cityInfo[2].Length - 2)
                    double cityLongitude = Convert.ToDouble(cityInfo[3]);
                    int population;
                    // If no population is listed...
                    if (cityInfo[9].Equals(""))
                    {
                        population = -1;
                    }
                    else
                    {
                        population = Convert.ToInt32(cityInfo[9]);
                    }
                    int remainingCities;
                    if (remainingCitiesPerCountry.TryGetValue(countryName, out remainingCities))
                    {
                        if (remainingCities > 0)
                        {
                            if (population >= minPopulation)
                            {
                                coordinates.Add(new CustomCoordinate(cityLattitude, cityLongitude, countryName, cityName, cityCount - (remainingCities - 1)));
                                remainingCitiesPerCountry[countryName] = remainingCities - 1;
                            }
                            // Once a city's population is found that is below the minimum, stop searching, as all cities below it will have lower population.
                            else 
                            {
                                remainingCitiesPerCountry[countryName] = 0;
                            }
                        }
                        // Otherwise city excluded as max cities reached
                    }
                    // Otherwise city excluded as country is excluded
                }
                else
                {
                    pastFirstLine = true;
                }
            }

            // Thanks to https://stackoverflow.com/questions/16921652/how-to-write-a-json-file-in-c Bartosz Pierzchlewicz and Liam for this one!
            string json = JsonSerializer.Serialize(coordinates);
            File.WriteAllText(filePath + coordinatesFileName, json);
        }

        static private Dictionary<string, int> generateCityCountDictionary(string filePath, int cityCount)
        {
            Dictionary<string, int> cityPerCountry = new Dictionary<string, int>();
            foreach (string country in File.ReadAllText(filePath + countriesFileName).Split(','))
            {
                Console.WriteLine("Added country " + country + " to allowed list.");
                cityPerCountry[country] = cityCount;
            }
            return cityPerCountry;
        } 

        static private string getDevFilePath()
        {
            string filePath = Environment.CurrentDirectory;
            if (filePath == null)
            {
                filePath = "NOTFOUND";
            }
            return filePath;
        }

        static private string getAppFilePath()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory;
            if (filePath == null)
            {
                filePath = "NOTFOUND";
            }
            return filePath;
        }
    }
}