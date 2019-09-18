using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiaryParser
{
    public class DiaryBuilder
    {
        public string Build(IEnumerable<IGrouping<string, DiaryRow>> rowsByDays)
        {
            var General = ResourceLoader.LoadTemplate("General");
            var Day = ResourceLoader.LoadTemplate("Day");

            List<string> days = new List<string>();
            foreach (var group in rowsByDays)
            {
                var day = Day.Replace(nameof(Day), group.Key);

                List<string> notes = new List<string>();
                for (int i = 0; i < group.Count(); i++)
                {
                    var row = group.ElementAtOrDefault(i);
                    var prev = group.ElementAtOrDefault(i - 1);
                    notes.Add(GetNote(row, prev?.Type));
                }

                day = day.Replace("Content", string.Join(Environment.NewLine, notes));
                days.Add(day);
            }

            return General.Replace(nameof(General), string.Join(Environment.NewLine, days));
        }

        private const string EmptyRow = "<li class='day'>";

        string GetNote(DiaryRow row, DiaryRowType? prev)
        {
            var Row = ResourceLoader.LoadTemplate("Row");
            var result = string.Empty;
            if (prev == null)
            {
                result = RowByType(row, Row);
                if (row.Type == DiaryRowType.Action)
                {
                    result = EmptyRow + result;
                }
            }
            else
            {
                result = RowByType(row, Row);
                if (row.Type == prev)
                {
                    result = EmptyRow + result;
                }
            }

            return result;
        }

        private static string RowByType(DiaryRow row, string Row)
        {
            Row = row.Type == DiaryRowType.Action
                ? ActionRow(row, Row)
                : FoodRow(row, Row);
            return Row;
        }

        private static string ActionRow(DiaryRow row, string Row)
        {
            Row = Row.Replace(nameof(Row), row.Text + SpanTime(row));
            return Row;
        }

        private static string SpanTime(DiaryRow row)
        {
            return $"<b class='b{(row.Type== DiaryRowType.Action ? "a" : "f")}'>[{row.Time}]</b>";
        }

        private static string FoodRow(DiaryRow row, string Row)
        {
            Row = Row.Replace(nameof(Row), SpanTime(row) + row.Text);
            return Row;
        }
    }
}