
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

        if (args.Length == 0) {
            Console.WriteLine("Usage:"); 
            Console.WriteLine(" -split <infile> <outfile> <startPageNo> <endPageNo>"); 
            Console.WriteLine(" -merge <file1> <file2> ..."); 
            return; }

        switch (args[0].ToLower()) {
            case "-split": 
                if (args.Length < 2) {
                    Console.WriteLine("Error: You must provide at least one file for -split."); 
                    return;
                }
                if (args.Length != 5) {
                    Console.WriteLine("Error: Invalid number of arguments for -split.");
                    return; 
                }
                string inFile = args[1];
                string outFile = args[2];
                int startNo, endNo;
                try
                {
                    startNo = int.Parse(args[3]);
                    endNo = int.Parse(args[4]);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error: Invalid parameter for -split. {ex.Message}");
                    return;
                }
                
                SplitPdf(inFile, outFile, startNo, endNo); 
                break; 

            case "-merge": 
                if (args.Length < 3) { 
                    Console.WriteLine("Error: You must provide start and pageNumber for -merge."); 
                    return; 
                } 

                if (!int.TryParse(args[1], out int start) || !int.TryParse(args[2], out int pageNumber)) { 
                    Console.WriteLine("Error: Start and pageNumber must be integers."); 
                    return; 
                } 

                MergeFunction(new List<string>(args[2..]).ToArray());

                break; 

            default: Console.WriteLine("Unknown parameter. Use -split or -merge."); 
                break; 
        }
    }

    static void MergeFunction(string[] args){
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

    static void SplitPdf(string inFile, string outFile, int startNo, int endNo){
        var handler = new PdfSplitHandler();
        var outputPdf = handler.SplitPdf(File.ReadAllBytes(inFile), startNo, endNo);
        File.WriteAllBytes(outFile, outputPdf);
    }
}





