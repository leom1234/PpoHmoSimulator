using System;
using System.Collections.Generic;
using System.IO;

namespace PPOSimulator
{
    public class TrialEventSet
    {
        private const int MinNumFields = 2; // code, person
        private const int MaxNumFields = 3; // code, person, price

        public string TrialEventsCsvPath { get; }
        public List<TrialEvent> TrialEvents { get; }
        public People People { get; }

        public TrialEventSet(string trialEventsCsvPath, People people)
        {
            People = people;
            TrialEventsCsvPath = trialEventsCsvPath;
            TrialEvents = Load(trialEventsCsvPath, people);
        }

        public static List<TrialEvent> Load(string trialEventsCsvPath, People people)
        {
            List<TrialEvent> trialEventsSet = new List<TrialEvent>();
            char[] delims = {','};
            var lines = File.ReadAllLines(trialEventsCsvPath);
            int lineNumber = 1;
            foreach (var line in lines)
            {
                var fields = line.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length > 0)
                {
                    if (fields.Length < MinNumFields || fields.Length > MaxNumFields)
                    {
                        throw new ApplicationException(
                            $"Line {lineNumber} of {trialEventsCsvPath} has {fields.Length} fields, must be {MinNumFields} to {MaxNumFields} fields on each line");
                    }

                    int i = 0;
                    string code = fields[i++].ToUpper().Trim();
                    string person = fields[i++].ToLower().Trim();
                    string serviceCostStr = (fields.Length == MaxNumFields) ? fields[i++].Trim() : "";

                    if (!people.Names.Contains(person))
                    {
                        throw new ApplicationException($"{trialEventsCsvPath} references '{person}', but that person is not in {people.PeopleCsvPath}");
                    }

                    int? serviceCost = null;

                    try
                    {
                        // if cost is present here, use it. Otherwise use null, which causes default cost to be used.
                        if (serviceCostStr.Length > 0)
                        {
                            serviceCost = int.Parse(serviceCostStr);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(
                            $"Line {lineNumber} of {trialEventsCsvPath} has an invalid integer field (field 2 must be integer)");
                    }

                    try
                    {
                        var trialEvent = new TrialEvent(code, person, serviceCost);
                        trialEventsSet.Add(trialEvent);
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(
                            $"Line {lineNumber} of {trialEventsCsvPath} is invalid: {e.Message}");
                    }

                }
            }

            return trialEventsSet;
        }
    }
}
