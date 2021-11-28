using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeclarativePM.Lib.Models;
using DeclarativePM.Lib.Models.LogModels;

namespace DeclarativePM.Lib.Import
{
    public static class ImportCsvLogs
    {
        public static ImportedEventLog LoadCsv(Stream stream, bool hasHeaders = true, string[] missing = null, char separator = ',')
        {
            var logs = new List<string[]>();
            string[] headers = null;
            missing ??= new[] {"none", "null", "nan", "na", "-"};
            using var csv = new StreamReader(stream);
            
            while (!csv.EndOfStream)
            {
                var line = csv.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var values = Regex.Split(line, $"{separator}(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); //strings in quotes wont be split
                if (hasHeaders && headers is null)
                {
                    headers = values;
                    continue;
                }
                //if no headers were defined we name each column by number from 0 to lenght - 1
                headers ??= Enumerable.Range(0, values.Length).Select(i => i.ToString()).ToArray();
                //if some values are missing we use null instead
                values = values.Select(v => missing.Contains(v.ToLower()) || string.IsNullOrWhiteSpace(v) ? string.Empty : v).ToArray();
                if (values.Length == headers.Length)
                    logs.Add(values);
            }
            
            return new ImportedEventLog(logs, headers);
        }

        public static ImportedEventLog LoadCsv(string path, bool hasHeaders = true, string[] missing = null,
            char separator = ',')
        {
            if (!File.Exists(path))
                return null;

            var stream = File.OpenRead(path);

            var result = LoadCsv(stream, hasHeaders, missing, separator);
            stream.Dispose();
            return result;
        }
    }
}