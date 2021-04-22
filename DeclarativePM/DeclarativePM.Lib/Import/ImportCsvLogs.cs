using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Import
{
    public class ImportCsvLogs
    {
        public IEnumerable<ImportedEventLog> LoadCsv(string path, bool hasHeaders = true, string[] missing = null, char separator = ',')
        {
            if (!File.Exists(path))
                return new List<ImportedEventLog>();

            var logs = new List<ImportedEventLog>();
            string[] headers = null;
            missing ??= new[] {"none", "null", "nan", "na", "-"};
            using var csv = new StreamReader(File.OpenRead(path));
            
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
                values = values.Select(v => missing.Contains(v) || string.IsNullOrWhiteSpace(v) ? null : v).ToArray();

                var labeledRow = headers.Zip(values).ToDictionary(k => k.First, v => v.Second);
                
                logs.Add(new ImportedEventLog(labeledRow));
            }

            return logs;
        }
    }
}