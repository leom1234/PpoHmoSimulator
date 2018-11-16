using System;
using System.Collections.Generic;
using System.IO;

namespace PPOSimulator
{
    public class PlanFeatures
    {
        private const int MinNumFields = 4; // code, description, default price, copay
        private const int MaxNumFields = 5; // code, description, default price, copay, co-insurance% (must end with "%")

        public string PlanFeaturesCsvPath { get; }
        public string NameOfPlan { get; }
        public int DeductiblePerPerson { get; }
        public int DeductiblePerFamily { get; }
        public int MaxPerPerson { get; }
        public int MaxPerFamily { get; }

        public Dictionary<string, PlanFeaturesFields> ExpenseFeaturesDict { get; }



        public PlanFeatures(string planFeaturesCsvPath)
        {
            PlanFeaturesCsvPath = planFeaturesCsvPath;
            int? deductiblePerPerson = null;
            int? deductiblePerFamily = null;
            int? maxPerPerson = null;
            int? maxPerFamily = null;

            ExpenseFeaturesDict = new Dictionary<string, PlanFeaturesFields>();
            char[] delims = {','};
            var lines = File.ReadAllLines(planFeaturesCsvPath);
            int lineNumber = 0;
            foreach (var line in lines)
            {
                lineNumber++;

                const string CommentPrefix = ";";
                int commentIndex = line.IndexOf(CommentPrefix);

                var trimmedLine = (commentIndex == -1) ? line : line.Substring(0, commentIndex);
                var fields = trimmedLine.Split(delims, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length == 0)
                {
                    continue;
                }

                if (fields.Length == 2)
                {
                    try
                    {
                        var key = fields[0].Trim().ToLower(); // TODO: refactor out Trim()
                        var val = fields[1].Trim();
                        switch (key)
                        {
                            case "nameofplan":
                                NameOfPlan = val;
                                break;
                            case "deductibleperperson":
                                deductiblePerPerson = int.Parse(val);
                                break;
                            case "deductibleperfamily":
                                deductiblePerFamily = int.Parse(val);
                                break;
                            case "maxoutofpocketperperson":
                                maxPerPerson = int.Parse(val);
                                break;
                            case "maxoutofpocketperfamily":
                                maxPerFamily = int.Parse(val);
                                break;
                            default:
                                throw new ApplicationException(
                                    $"Invalid key / value pair in line {line} of {planFeaturesCsvPath}");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(
                            $"Line {lineNumber} ('{line}') of {planFeaturesCsvPath} has an invalid integer field (field 3 and field 4 or 5 must be integer)");
                    }
                }
                else if (fields.Length == MinNumFields || fields.Length == MaxNumFields)
                {
                    int i = 0;
                    string code = fields[i++].Trim().ToUpper();
                    string description = fields[i++].Trim();
                    string defaultServiceCostStr = fields[i++].Trim();
                    string copayDollarsStr = fields[i++].Trim();

                    string coinsurancePercentStr = "";

                    if (fields.Length == MaxNumFields)
                    {
                        coinsurancePercentStr = fields[i++].Trim();
                        if (!coinsurancePercentStr.EndsWith("%"))
                        {
                            throw new ApplicationException(
                                $"Line {lineNumber} ('{line}') of {planFeaturesCsvPath} field 5 (co-insurance) must end with '%'. If item is a copay instead, omit entire field 5");
                        }

                        coinsurancePercentStr = coinsurancePercentStr.Substring(0, coinsurancePercentStr.Length - 1);
                    }

                    int defaultServiceCost;
                    int? copayDollars = null;
                    int? coinsurancePercent = null;

                    try
                    {
                        defaultServiceCost = int.Parse(defaultServiceCostStr);
                        if (copayDollarsStr.Length > 0)
                        {
                            copayDollars = int.Parse(copayDollarsStr);
                        }

                        if (coinsurancePercentStr.Length > 0)
                        {
                            coinsurancePercent = int.Parse(coinsurancePercentStr);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(
                            $"Line {lineNumber} ('{line}') of {planFeaturesCsvPath} has an invalid integer field (field 3 and field 4 or 5 must be integer)");
                    }

                    try
                    {
                        var features = new PlanFeaturesFields(code.ToUpper(), description, defaultServiceCost,
                            copayDollars, coinsurancePercent);
                        ExpenseFeaturesDict.Add(code, features);
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException(
                            $"Line {lineNumber} ('{line}') of {planFeaturesCsvPath} is invalid: {e.Message}");
                    }

                    if (NameOfPlan.Length == 0)
                        throw new ApplicationException($"NameOfPlan is not defined in {planFeaturesCsvPath}");

                    if (deductiblePerPerson == null)
                        throw new ApplicationException($"DeductiblePerPerson is not defined in {planFeaturesCsvPath}");

                    if (deductiblePerFamily == null)
                        throw new ApplicationException($"DeductiblePerFamily is not defined in {planFeaturesCsvPath}");

                    if (maxPerPerson == null)
                        throw new ApplicationException($"MaxPerPerson is not defined in {planFeaturesCsvPath}");

                    if (maxPerFamily == null)
                        throw new ApplicationException($"MaxPerFamily is not defined in {planFeaturesCsvPath}");

                    DeductiblePerPerson = deductiblePerPerson.Value;
                    DeductiblePerFamily = deductiblePerFamily.Value;
                    MaxPerPerson = maxPerPerson.Value;
                    MaxPerFamily = maxPerPerson.Value;
                }
                else
                {
                    throw new ApplicationException(
                        $"Line {lineNumber} of {planFeaturesCsvPath} ('{line}') has {fields.Length} fields, must be 2, {MinNumFields} or {MaxNumFields} fields on each line");
                }

            }

        }
    }
}
