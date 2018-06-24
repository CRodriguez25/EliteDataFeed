using Newtonsoft.Json;
using PanelDataFeed;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace EliteDangerousDataFeed
{
    public class EDDataFeed : DataFeed
    {
        private string _currentFile = null;
        private DateTime _lastSentEntry = DateTime.MinValue;
        private DateTime _lastStatusUpdate = DateTime.MinValue;
        private StatusGenerator _statusGenerator = new StatusGenerator();
        public override IEnumerable<DataFeedSettingDeclaration> RequiredSettings()
        {
            return new List<DataFeedSettingDeclaration>
            {
                new DataFeedSettingDeclaration
                {
                    DisplayRequestForSetting = "Please provide the location of your Elite Dangerous Journal Files",
                    SettingName = "JournalLocation",
                    DefaultValue = "%UserProfile%\\Saved Games\\Frontier Developments\\Elite Dangerous",
                    SettingType = SettingType.Directory
                }
            };
        }

        public override void Start(IDataFeedContext dataFeedContext, IPanelCommunicator panelCommunicator)
        {
            var journalLocation = dataFeedContext.GetSettings()["JournalLocation"].SettingValue;
            journalLocation = Environment.ExpandEnvironmentVariables(journalLocation);
            while(true)
            {
                try
                {
                    GetJournalUpdates(panelCommunicator, journalLocation);
                    GetStatusUpdates(panelCommunicator, journalLocation);
                }
                catch(Exception e) { }

                Thread.Sleep(250);
            }
        }

        private void GetStatusUpdates(IPanelCommunicator panelCommunicator, string journalLocation)
        {
            var fileName = Path.Combine(journalLocation, "Status.json");
            if (!File.Exists(fileName)) return;

            var lastWriteTime = File.GetLastWriteTime(fileName);
            if (lastWriteTime > _lastStatusUpdate)
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader fileReader = new StreamReader(fileStream))
                {
                    while (!fileReader.EndOfStream)
                    {
                        var text = fileReader.ReadToEnd();
                        var statusObject = JsonConvert.DeserializeObject<dynamic>(text);
                        Dictionary<string, bool> statusResult = _statusGenerator.GenerateStatus((int)statusObject.Flags);
                        var status = new StatusResult
                        {
                            Event = "Status",
                            CurrentStatus = statusResult
                        };

                        var serializedStatus = JsonConvert.SerializeObject(status);
                        panelCommunicator.SendMessageToPanel(serializedStatus);
                    }
                }

                _lastStatusUpdate = lastWriteTime;
            }
        }

        private void GetJournalUpdates(IPanelCommunicator panelCommunicator, string journalLocation)
        {
            var allJournalEntries = Directory.GetFiles(journalLocation, "Journal*").OrderByDescending(x => x);
            if (_currentFile == null)
            {
                _currentFile = allJournalEntries.First();
            }
            else CheckForNewJournalFile(allJournalEntries);

            var newEntries = GetNewEntries();
            foreach (var entry in newEntries)
            {
                panelCommunicator.SendMessageToPanel(entry.Raw.ToString());
                _lastSentEntry = entry.timestamp;
            }
        }

        private void CheckForNewJournalFile(IOrderedEnumerable<string> allJournalEntries)
        {
            if (allJournalEntries.First() == _currentFile) return;
            _currentFile = allJournalEntries.First();
        }

        private IEnumerable<dynamic> GetNewEntries()
        {
            string fileName = _currentFile;
            IEnumerable<string> fileLines = new List<string>();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader fileReader = new StreamReader(fileStream))
            {
                while (!fileReader.EndOfStream)
                {
                    fileLines = fileReader.ReadToEnd().Split('\n');
                }
            }

            var events = fileLines.Select(x => {
                var result = JsonConvert.DeserializeObject<dynamic>(x);
                if (result == null) return null;
                result.Raw = x;
                return result;
            }).Where(x => x != null).OrderByDescending(x => x.timestamp);

            if (!events.Any())
            {
                _lastSentEntry = DateTime.UtcNow;
                return new List<dynamic>();
            }

            if (_lastSentEntry == DateTime.MinValue)
            {
                _lastSentEntry = DateTime.UtcNow;
                return new List<dynamic>();
            }

            return events.Where(x => x.timestamp > _lastSentEntry).OrderByDescending(x => x.timestamp);
        }

        public override void DataFeedUnloaded()
        {

        }
    }
}
