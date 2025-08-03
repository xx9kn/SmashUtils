using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

public class PdfSampleHandler
{
    public byte[] GenerateSamplePdf(string title, string content)
    {
        using var outputStream = new MemoryStream();

        return outputStream.ToArray();
    }
}
