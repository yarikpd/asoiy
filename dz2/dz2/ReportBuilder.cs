using System.Text;

namespace dz2;

internal class ReportBuilder(DatabaseManager db)
{
    private string _sql = "";
    private string _title = "";
    private string[] _headers = [];
    private int[] _widths = [];
    private string _footer = "";

    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    public ReportBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }

    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    public ReportBuilder Footer(string label)
    {
        _footer = label;
        return this;
    }

    private string Build()
    {
        var (columns, rows) = db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        if (_title.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"=== {_title} ===");
        }

        var displayHeaders = _headers.Length > 0 ? _headers : columns;

        var colCount = displayHeaders.Length;
        int[] widths;
        if (_widths.Length >= colCount)
        {
            widths = _widths;
        }
        else
        {
            widths = new int[colCount];
            for (var i = 0; i < colCount; i++)
                widths[i] = 20;
        }

        for (var i = 0; i < colCount; i++)
            sb.Append(displayHeaders[i].PadRight(widths[i]));
        sb.AppendLine();

        var totalWidth = 0;
        for (var i = 0; i < colCount; i++)
            totalWidth += widths[i];
        sb.AppendLine(new string('─', totalWidth));

        foreach (var t in rows)
        {
            for (var c = 0; c < t.Length && c < colCount; c++)
                sb.Append(t[c].PadRight(widths[c]));

            sb.AppendLine();
        }

        if (_footer.Length <= 0) return sb.ToString();
        sb.AppendLine(new string('─', totalWidth));
        sb.AppendLine($"{_footer}: {rows.Count}");

        return sb.ToString();
    }

    public void Print()
    {
        Console.Write(Build());
    }
}