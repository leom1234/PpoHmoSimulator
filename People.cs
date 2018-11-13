using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PPOSimulator
{
    public class People
    {
        public List<string> Names { get; }
        public string PeopleCsvPath { get; }

        public People(string peopleCsvPath)
        {
            PeopleCsvPath = peopleCsvPath;

            try
            {
                Names = new List<string>();
                var names = File.ReadAllLines(peopleCsvPath).ToList();

                foreach (var name in names)
                {
                    var trimmedName = name.Trim();
                    if (trimmedName.Length >= 0)
                    {
                        Names.Add(trimmedName);
                    }
                }

            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error reading names from {peopleCsvPath}: {e.Message}");
            }

            if (Names.Count == 0)
            {
                throw new ApplicationException($"{peopleCsvPath} must contain at least one name");
            }
        }
    }
}
