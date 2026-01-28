using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

public class PdfSplitHandler
{
    public byte[] SplitPdf(byte[] pdfFile, int startPage, int endPage){
        using var inputStream = new MemoryStream(pdfFile);
        using var inputDocument = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);
        using var outputStream = new MemoryStream();
        using var outputDocument = new PdfDocument();

        // Ensure the page range is valid
        if(startPage < 1 || endPage > inputDocument.PageCount || startPage > endPage){
            throw new ArgumentOutOfRangeException("Invalid page range specified.");
        }

        for(int i = startPage - 1; i < endPage; i++){
            outputDocument.AddPage(inputDocument.Pages[i]);
        }

        outputDocument.Save(outputStream, false);
        return outputStream.ToArray();
    }
}