using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class PdfMergeHandler
{
    public byte[] JoinPdfs(params byte[][] pdfFiles)
    {
        using var outputStream = new MemoryStream();
        using var mergedDocument = new PdfDocument();

        foreach (var pdf in pdfFiles)
        {
            using var inputStream = new MemoryStream(pdf);
            using var inputDocument = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);
            for (int i = 0; i < inputDocument.PageCount; i++)
            {
                mergedDocument.AddPage(inputDocument.Pages[i]);
            }
        }

        mergedDocument.Save(outputStream, false);
        return outputStream.ToArray();
    }

}
