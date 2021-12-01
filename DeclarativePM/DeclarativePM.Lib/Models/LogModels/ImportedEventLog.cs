using System;
using System.Collections.Generic;
using System.Linq;
using DeclarativePM.Lib.Exceptions;

namespace DeclarativePM.Lib.Models.LogModels
{
    public class ImportedEventLog
    {
        
        private int _activity = 0;
        public string Activity()
        {
            return Headers[_activity];
        }

        private int _caseId = 1;
        public string CaseId()
        {
            return Headers[_caseId];
        }
        
        private int _timeStamp = -1;
        public string TimeStamp()
        {
            if (_timeStamp == -1)
                throw new LogValueNotSetException(
                        "TimeStamp property is not defined. You can define it by calling ChooseTokens method.");
            return Headers[_timeStamp];
        }
        
        private List<int> _resources;
        public List<string> Resources()
            => _resources.Select(r => Headers[r]).ToList();

        

        private List<string[]> rows;
        public string[] Headers { get; }

        public ImportedEventLog(List<string[]> rows, string[] headers)
        {
            if (headers.Length < 2)
                throw new Exception("Not enough columns");
            Headers = headers;
            if (rows.Any(r => r.Length != headers.Length))
                throw new Exception("Some rows were of different length then others.");
            this.rows = rows;
            _resources = Enumerable.Range(2, headers.Length - 2).ToList();
        }
        
        //TODO Add() and AddRange()
        

        public void ChooseTokens(string activity, string caseId, string timeStamp = null, params string[] resources)
        {
            if ((timeStamp is not null && !Headers.Contains(timeStamp)) ||
                !Headers.Contains(activity) || !Headers.Contains(caseId) ||
                resources.Any(v => !Headers.Contains(v)))
            //TODO better exception
                throw new Exception("One of the values in headers does not exist.");

            _activity = Array.IndexOf(Headers, activity);
            _caseId = Array.IndexOf(Headers, caseId);
            
            if (timeStamp is null) return;
            _timeStamp = Array.IndexOf(Headers, timeStamp);

            _resources = resources.Select(r => Array.IndexOf(Headers, r)).ToList();
            /*if (DateTime.TryParse(Data[timeStamp], out var time))
            {
                TimeStamp = time;
                _timeStampName = timeStamp;
            }*/
            //TODO RESET

        }

        public EventLog BuildEventLog(string name = null)
        {
            List<Event> events = new List<Event>(rows.Capacity);
            Func<string[], DateTime> converter = (row) => DateTime.TryParse(row[_timeStamp], out var time) 
                ? time : DateTime.MinValue;
            events.AddRange(rows
                .Select(row =>
                {
                    var e =  new Event(
                        row[_activity],
                        row[_caseId],
                        _resources.Select(r => row[r]).ToArray());
                    if (_timeStamp > 0)
                        e.TimeStamp = converter(row);
                    return e;
                }));
            return new (events, Headers.ToList(), name);
        }
    }
}