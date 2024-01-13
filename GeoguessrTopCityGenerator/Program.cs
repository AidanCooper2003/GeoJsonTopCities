namespace GeoguessrTopCityGenerator
{
    internal class Program
    {
        static string citiesFileName = "\\worldcities.csv";
        static string countriesFileName = "\\validcountries.txt";

        static void Main(string[] args)
        {
            string filePath = getDevFilePath();
            Console.WriteLine("Welcome! This program will generate a json file for importing into map-making.app that will contain the most populous cities of every geoguessr valid country. Make sure to put your csv file into the same directory as this program.");
            Console.WriteLine("How many cities per country do you want to generate?");
            int cityCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Generating " + cityCount + " cities for each country...");
            Console.WriteLine(filePath);
            generateMapJson(filePath);
            Console.WriteLine("Generation is complete, the json will now be in the application's directory.");
        }

        static private void generateMapJson(String filePath)
        {

            // Operates under the assumption that the CSV file is under the same format as the one from https://simplemaps.com/data/world-cities
            foreach (string line in File.ReadLines(filePath))
            {
                // Next make this go into an arraylist and find the lattitude, longitude, population, and country
                line.Split(',');
                Console.WriteLine(line);
            }
        }

        static private string getDevFilePath()
        {
            string filePath = Environment.CurrentDirectory + fileName;
            if (filePath == null)
            {
                filePath = "NOTFOUND";
            }
            return filePath;
        }

        static private string getAppFilePath()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (filePath == null)
            {
                filePath = "NOTFOUND";
            }
            return filePath;
        }
    }
}