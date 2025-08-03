
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Main entry point for the PDF join console app.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        string[] fileNames = GetInputFileNames(args);
        Console.WriteLine("Merging PDFs...");
        string mergedFile = MergePdfs(fileNames);
        string mergedFilePath = Path.GetFullPath(mergedFile);
        Console.WriteLine($"Merged PDF saved at: {mergedFilePath}");

        PromptOpenFileExplorer(mergedFilePath);
    }

    /// <summary>
    /// Gets the list of PDF files to merge, either from user input or by generating samples.
    /// </summary>
    static string[] GetInputFileNames(string[] args)
    {
        if (args.Length > 0)
        {
            Console.WriteLine("Merging user-provided PDF files:");
            foreach (var file in args)
                Console.WriteLine($"- {file}");
            return args;
        }
        else
        {
            EnsureSamplePdfExists("a.pdf", "Sample A", "This is sample PDF A.");
            EnsureSamplePdfExists("b.pdf", "Sample B", "This is sample PDF B.");
            Console.WriteLine("No input files provided. Using sample PDFs: a.pdf, b.pdf");
            return new[] { "a.pdf", "b.pdf" };
        }
    }

    /// <summary>
    /// Prompts the user to open the containing folder of the merged PDF file.
    /// </summary>
    static void PromptOpenFileExplorer(string mergedFilePath)
    {
        Console.Write("Would you like to open the containing folder? (y/n): ");
        var response = Console.ReadLine();
        if (response?.Trim().ToLower() == "y")
        {
            OpenFileExplorer(mergedFilePath);
        }
    }

    /// <summary>
    /// Opens the file explorer at the location of the merged PDF file.
    /// </summary>
    static void OpenFileExplorer(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
#if OS_WINDOWS
        System.Diagnostics.Process.Start("explorer.exe", directory);
#elif OS_OSX
        System.Diagnostics.Process.Start("open", directory);
#elif OS_LINUX
        System.Diagnostics.Process.Start("xdg-open", directory);
#else
        Console.WriteLine($"Please open your file explorer and navigate to: {directory}");
#endif
    }

    /// <summary>
    /// Prints a welcome header.
    /// </summary>
    static void PrintHeader()
    {
        Console.WriteLine("==============================");
        Console.WriteLine(" PDF Join Console Application ");
        Console.WriteLine("==============================");
        Console.WriteLine();
    }

    /// <summary>
    /// Ensures a sample PDF exists, creating it if missing.
    /// </summary>
    static void EnsureSamplePdfExists(string fileName, string title, string content)
    {
        if (!File.Exists(fileName))
        {
            var sampleHandler = new PdfSampleHandler();
            var pdf = sampleHandler.GenerateSamplePdf(title, content);
            File.WriteAllBytes(fileName, pdf);
            Console.WriteLine($"Created sample PDF: {fileName}");
        }
    }

    /// <summary>
    /// Merges the specified PDF files and returns the output filename.
    /// </summary>
    static string MergePdfs(string[] fileNames)
    {
        var pdfFiles = fileNames.Select(File.ReadAllBytes).ToArray();
        var handler = new PdfMergeHandler();
        var joinedPdf = handler.JoinPdfs(pdfFiles);
        var outputFile = "merged.pdf";
        File.WriteAllBytes(outputFile, joinedPdf);
        return outputFile;
    }
}





