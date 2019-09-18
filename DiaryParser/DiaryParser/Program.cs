using System;
using System.IO;
using System.Linq;

namespace DiaryParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourcePath = args.ElementAtOrDefault(0);
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("path to source file not found");

            var destPath = args.ElementAtOrDefault(1);
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("path to destination file not found");

            var diaryRows = new HtmlParser().Parse(sourcePath);

            var orderedRows = diaryRows
                .OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}:00"))
                .GroupBy(x => x.Date);

            var result = new DiaryBuilder().Build(orderedRows);

            File.WriteAllText(destPath, result);
        }
    }
}