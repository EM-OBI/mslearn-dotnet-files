using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Call GenerateSalesSummary()
GenerateSalesSummary(salesFiles, salesTotal, salesTotalDir);


IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        if (file.Contains("sales"))
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, double salesTotal, string outputFolder)
{
    StringBuilder report = new();

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($"Total Sales: {salesTotal:C}");
    report.AppendLine();
    report.AppendLine("Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData>(salesJson);

        double total = data?.Total ?? 0;

        string folder = Path.GetFileName(Path.GetDirectoryName(file)!);
        string relativePath = Path.Combine(folder, Path.GetFileName(file));

        report.AppendLine(
            $"{relativePath}: {total:C}"
        );

        string reportPath = Path.Combine(outputFolder, "totals.txt");

        File.WriteAllText(reportPath, report.ToString());
    }
}

record SalesData(double Total);


