using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1 - Convert CSV to JSON");
            Console.WriteLine("2 - Remove Key-Value Pairs from JSON");
            Console.WriteLine("0 - Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ConvertCsvToJsonService();
                    break;
                case "2":
                    RemoveKeyValuePairsFromJsonService();
                    break;
                case "0":
                    Console.WriteLine("Exiting application. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void ConvertCsvToJsonService()
    {
        string csvFilePath = GetCsvFilePathFromUser();
        if (csvFilePath == null) return;

        string jsonFilePath = GetJsonFilePathFromUser();
        if (jsonFilePath == null) return;

        try
        {
            var jsonContent = ConvertCsvToJson(csvFilePath);
            File.WriteAllText(jsonFilePath, jsonContent);
            Console.WriteLine($"CSV to JSON conversion successful. The JSON file is saved at: {jsonFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void RemoveKeyValuePairsFromJsonService()
    {
        Console.WriteLine("Enter the path of the JSON file:");
        string jsonFilePath = Console.ReadLine();

        if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath) || Path.GetExtension(jsonFilePath).ToLower() != ".json")
        {
            Console.WriteLine("Invalid JSON file path. Please try again.");
            return;
        }

        try
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            var jsonObject = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent);

            if (jsonObject == null || jsonObject.Count == 0)
            {
                Console.WriteLine("The JSON file is empty or not in the expected format.");
                return;
            }

            Console.WriteLine("Enter the keys to remove (comma-separated):");
            string keysToRemoveInput = Console.ReadLine();

            if (string.IsNullOrEmpty(keysToRemoveInput))
            {
                Console.WriteLine("No keys provided. Operation canceled.");
                return;
            }

            var keysToRemove = keysToRemoveInput.Split(',').Select(k => k.Trim()).ToList();

            foreach (var obj in jsonObject)
            {
                foreach (var key in keysToRemove)
                {
                    obj.Remove(key);
                }
            }

            string updatedJsonContent = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText(jsonFilePath, updatedJsonContent);

            Console.WriteLine("Key-value pairs removed successfully. The updated JSON file is saved at the same location.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static string GetCsvFilePathFromUser()
    {
        Console.WriteLine("Enter the path of the CSV file:");
        string csvFilePath = Console.ReadLine();

        if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath) || Path.GetExtension(csvFilePath).ToLower() != ".csv")
        {
            Console.WriteLine("Invalid CSV file path. Please try again.");
            return null;
        }

        return csvFilePath;
    }

    static string GetJsonFilePathFromUser()
    {
        Console.WriteLine("Enter the output JSON file path:");
        string jsonFilePath = Console.ReadLine();

        string directoryPath = Path.GetDirectoryName(jsonFilePath);
        if (string.IsNullOrEmpty(jsonFilePath) || !Directory.Exists(directoryPath) || Path.GetExtension(jsonFilePath).ToLower() != ".json")
        {
            Console.WriteLine("Invalid JSON file path. Please try again.");
            return null;
        }

        return jsonFilePath;
    }

    static string ConvertCsvToJson(string csvFilePath)
    {
        using var reader = new StreamReader(csvFilePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });

        try
        {
            var records = csv.GetRecords<dynamic>().ToList();
            return JsonConvert.SerializeObject(records, Formatting.Indented);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading the CSV file: {ex.Message}");
            throw;
        }
    }
}
