using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComposingMethods
{
    public class Practice
    {
        internal interface IEventService
        {
            DirectoryInfo CreateContent(string pubYear, DateTime? newsDate);
            void SaveAndPublish(DirectoryInfo yearFolder);
            void CreateYearFolders(Dictionary<string, int> yearDict);
        }

        internal class Event
        {
            public DateTime? NewsDate { get; set; }
            public Dictionary<string,string> Cache { get; set; }

        }

        internal void SaveEvents(IEventService service, List<Event> events, int yearId)
        {
            Dictionary<string, int> yearDict = new Dictionary<string, int>();

            foreach (var singleEvent in events)
            {
                SetTheNewsDate(singleEvent);


                if (yearId == 0)
                {
                    CreateYearNode(service, singleEvent);

                    AddAndSortYears(singleEvent, yearDict);
                }
            }
            //Create year folders
            service.CreateYearFolders(yearDict);
        }

        private static void AddAndSortYears(Event singleEvent, Dictionary<string, int> yearDict)
        {
            foreach (var yearNode in singleEvent.Cache)
            {
                if (yearNode.Key == "year")
                {
                    yearDict.Add(yearNode.Key, int.Parse(yearNode.Value));
                }
            }

            var yearList = yearDict.Keys.ToList();
            yearList.Sort();
            yearList.Reverse();
        }

        private static void CreateYearNode(IEventService service, Event singleEvent)
        {
            string pubYear = singleEvent.NewsDate.Value.Year.ToString(); //year in format like "2013"
            //No year node, create one
            var yearFolder = service.CreateContent(pubYear, singleEvent.NewsDate);
            service.SaveAndPublish(yearFolder);
        }

        private static void SetTheNewsDate(Event singleEvent)
        {
            //default to today if no date is specified, or if date is invalid (date comes back as year 1, month 1, etc
            if (!singleEvent.NewsDate.HasValue || singleEvent.NewsDate.Value.Year == 1)
            {
                singleEvent.NewsDate = DateTime.Now;
            }
        }
    }
}
