using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiaryParser
{
    public class HtmlParser
    {
        public IEnumerable<DiaryRow> Parse(string path)
        {
            var doc = new HtmlDocument();            
            doc.LoadHtml(File.ReadAllText(path));

            var rawRows= doc.DocumentNode.Descendants("div")
                .Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "body")
                .Select(x => new 
                {
                    Text = x.Descendants("div").FirstOrDefault(n=>n.Attributes.Contains("class") && n.Attributes["class"].Value=="text")?.InnerText,
                    When = x.Descendants("div").FirstOrDefault(n => n.Attributes.Contains("title"))?.Attributes["title"].Value,
                });

            List<DiaryRow> rows = new List<DiaryRow>();

            foreach (var rawRow in rawRows)
            {
                var diaryRow = new DiaryRow();

                DateTime when = DateTime.Parse(rawRow.When);

                var clearText = rawRow.Text;

                if (rawRow.Text.Contains("["))
                {
                    var additionalDateInfo = rawRow.Text.Substring(rawRow.Text.IndexOf("["))
                        .Trim()
                        .Trim('[')
                        .Trim(']');

                    if (additionalDateInfo.Contains("."))
                    {
                        when = DateTime.Parse(additionalDateInfo);
                    }
                    else
                    {
                        when = DateTime.Parse(string.Format("{0:dd.MM.yyyy}", when) + " " + additionalDateInfo);
                    }

                    clearText = clearText.Replace($"[{additionalDateInfo}]", "");
                }

                diaryRow.Type = clearText.Contains(".")
                    ? DiaryRowType.Food
                    : DiaryRowType.Action;

                clearText = clearText.Replace(".","");

                diaryRow.Date = when.ToString("dd.MM.yyyy");
                diaryRow.Time = when.ToString("HH:mm");
                diaryRow.Text = clearText;

                rows.Add(diaryRow);
            }

            return rows;
        }
    }
}
