using System.Globalization;
using System.Text;

namespace WebPage.Services;

public static class SimplePdf
{
    public static byte[] Create(IEnumerable<string> lines, string title)
    {
        var safeLines = new List<string> { title };
        safeLines.AddRange(lines);

        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 12 Tf");
        content.AppendLine("50 800 Td");
        content.AppendLine("14 TL");

        foreach (var raw in safeLines.Take(48))
        {
            var line = Escape(raw);
            content.Append('(').Append(line).AppendLine(") Tj");
            content.AppendLine("T*");
        }

        content.AppendLine("ET");
        var contentBytes = Encoding.ASCII.GetBytes(content.ToString());

        var objects = new List<byte[]>
        {
            Encoding.ASCII.GetBytes("<< /Type /Catalog /Pages 2 0 R >>"),
            Encoding.ASCII.GetBytes("<< /Type /Pages /Kids [3 0 R] /Count 1 >>"),
            Encoding.ASCII.GetBytes("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>"),
            Encoding.ASCII.GetBytes($"<< /Length {contentBytes.Length} >>\nstream\n{content}\nendstream"),
            Encoding.ASCII.GetBytes("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>")
        };

        using var output = new MemoryStream();
        Write(output, "%PDF-1.4\n");

        var offsets = new List<long> { 0 };
        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(output.Position);
            Write(output, $"{i + 1} 0 obj\n");
            output.Write(objects[i]);
            Write(output, "\nendobj\n");
        }

        var xref = output.Position;
        Write(output, $"xref\n0 {objects.Count + 1}\n");
        Write(output, "0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1))
            Write(output, $"{offset.ToString("D10", CultureInfo.InvariantCulture)} 00000 n \n");

        Write(output, $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return output.ToArray();
    }

    private static string Escape(string value) =>
        value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

    private static void Write(Stream stream, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }
}
