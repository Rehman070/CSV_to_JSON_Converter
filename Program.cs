using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Get the path of the CSV file from user input
        string csvFilePath = GetCsvFilePathFromUser();
        if (csvFilePath == null) return; // Exit if invalid path

        // Get the output JSON file path from user input
        string jsonFilePath = GetJsonFilePathFromUser();
        if (jsonFilePath == null) return; // Exit if invalid path

        try
        {
            // Convert CSV to JSON and save it to the specified file
            var jsonContent = ConvertCsvToJson(csvFilePath);
            File.WriteAllText(jsonFilePath, jsonContent);

            Console.WriteLine($"CSV to JSON conversion successful. The JSON file is saved at: {jsonFilePath}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {ex.FileName}. Please check the file path and try again.");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("You do not have permission to access this file or directory. Please check the file's permissions.");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("The directory specified for the output file does not exist. Please provide a valid directory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}. Please check the file and try again.");
        }

        // Pause before closing the console
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();  // Wait for the user to press Enter before closing
    }

    // Method to get CSV file path from user with validation
    static string GetCsvFilePathFromUser()
    {
        Console.WriteLine("Enter the path of the CSV file:");

        string csvFilePath = Console.ReadLine();

        // Validate if the file exists and if it's a CSV file
        if (string.IsNullOrEmpty(csvFilePath))
        {
            Console.WriteLine("The file path cannot be empty. Please enter a valid CSV file path.");
            return null;
        }

        if (!File.Exists(csvFilePath))
        {
            Console.WriteLine($"The file '{csvFilePath}' does not exist. Please check the file path and try again.");
            return null;
        }

        if (Path.GetExtension(csvFilePath).ToLower() != ".csv")
        {
            Console.WriteLine("Please provide a valid CSV file. Only CSV files are allowed.");
            return null;
        }

        return csvFilePath;
    }

    // Method to get JSON output file path from user with validation
    static string GetJsonFilePathFromUser()
    {
        Console.WriteLine("Enter the output JSON file path:");

        string jsonFilePath = Console.ReadLine();

        // Validate if the path is empty or if directory does not exist
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Console.WriteLine("The output file path cannot be empty. Please enter a valid path.");
            return null;
        }

        // Ensure directory exists, if not, inform the user
        string directoryPath = Path.GetDirectoryName(jsonFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"The directory '{directoryPath}' does not exist. Please provide a valid directory path.");
            return null;
        }

        // Check for valid file extension
        if (Path.GetExtension(jsonFilePath).ToLower() != ".json")
        {
            Console.WriteLine("Please provide a valid JSON file path with a '.json' extension.");
            return null;
        }

        return jsonFilePath;
    }

    // Method to convert CSV file content to JSON
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
            var jsonContent = JsonConvert.SerializeObject(records, Formatting.Indented);
            return jsonContent;
        }
        catch (CsvHelperException)
        {
            Console.WriteLine("Error reading the CSV file. Please ensure the CSV format is correct.");
            throw;
        }
    }
}
