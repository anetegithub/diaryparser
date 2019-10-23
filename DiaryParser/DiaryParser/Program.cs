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

            var diaryRows = new HtmlParser().Parse(sourcePath, 
                new DateSpec[]{ new DatePartReplaceSpec("10.11.2019","11.10.2019") }}
            );

            var orderedRows = diaryRows
                .OrderBy(x => DateTime.Parse($"{x.Date} {x.Time}:00"))
                .GroupBy(x => x.Date);

            var result = new DiaryBuilder().Build(orderedRows);

            File.WriteAllText(destPath, result);
        }
    }

    public class TextReplacerSpec : TextSpec
    {
        string _from;
        string _to;

        public TextReplacerSpec(string from, string to)
        {
            _from = from;
            _to = to;
        }

        public override string Process(string original) => original.Replace(_from, _to);
    }

    public class DatePartReplaceSpec : DateSpec
    {
        string _from;
        string _to;

        public DatePartReplaceSpec(string from, string to)
        {
            _from = from;
            _to = to;
        }

        public override DateTime? Process(string additionalDateInfo, DateTime whenOriginal)
        {
            additionalDateInfo = additionalDateInfo.Replace(_from, _to);
            
            if (additionalDateInfo.Contains("."))
            {
                if (!DateTime.TryParse(additionalDateInfo, out var when))
                {
                    var parts = additionalDateInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var nextPattern = parts[1] + " " + parts[0];
                    return DateTime.Parse(nextPattern);
                }
                return when;
            }
            else
            {
                return DateTime.Parse(string.Format("{0:dd.MM.yyyy}", whenOriginal) + " " + additionalDateInfo);
            }
        }
    }
}