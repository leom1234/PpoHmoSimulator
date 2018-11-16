using System;
using System.Collections.Generic;

namespace PPOSimulator
{
    class Program
    {
        static void Usage(string problem)
        {
            throw new ApplicationException(
                $"({problem}): Usage: pathToPeople.Csv pathToTrialEvents.csv pathToPlan1Features.csv [pathToPlan2Features.csv] ...");
        }

        static void Main(string[] args)
        {
            if (args.Length < 3 )
                Usage("Wrong number of arguments. Use pathToPeople.Csv pathToTrialEvents.csv and 1 or more pathToPlanFeatures.csv");

            try
            {
                int i = 0;
                var peopleCsvPath = args[i++];
                var trialEventsPath = args[i++];

                List<string> planFeaturesPaths = new List<string>();
                for (; i < args.Length; i++)
                {
                    planFeaturesPaths.Add(args[i]);
                }

                var sim = new HealthInsuranceSimulator(
                    peopleCsvPath,
                    trialEventsPath,
                    planFeaturesPaths.ToArray());

                sim.Run();
            }
            catch (Exception e)
            {
                Usage(e.Message);
            }
        }
    }

    public class HealthInsuranceSimulator
    {
        private string _peopleCsvPath;
        private string[] _planFeaturesCsvPaths;
        private string _trialEventsCsvPath;
        private const int Places = 2;

        public People People { get; }
        public TrialEventSet TrialEvents { get; }
        public List<SimStatusOnePlan> SimStatusPerPlan { get; }

        public HealthInsuranceSimulator(
            string peoplePath,
            string trialEventsPath,
            string[] planFeaturesPaths)
        {
            _trialEventsCsvPath = trialEventsPath;
            _planFeaturesCsvPaths = planFeaturesPaths;
            _peopleCsvPath = peoplePath;

            People = new People(peoplePath);
            TrialEvents = new TrialEventSet(trialEventsPath, People);
            SimStatusPerPlan = new List<SimStatusOnePlan>();

            foreach (var planFeaturesPath in planFeaturesPaths)
            {
                var planFeatures = new PlanFeatures(planFeaturesPath);
                var simStatusThisPlan = new SimStatusOnePlan(People, planFeatures, TrialEvents);
                SimStatusPerPlan.Add(simStatusThisPlan);
            }
        }

        public void Run()
        {
            // Write header
            Console.Write("Code,Description,Service Cost");

            foreach (var simStatus in SimStatusPerPlan)
            {
                var plan = simStatus.PlanFeatures.NameOfPlan;
                Console.Write($",{plan} Copay,{plan} Deductible Paid YTD,{plan} Out of Pocket YTD");
            }

            Console.WriteLine();

            bool firstPlan = true;
            foreach (var trialEvent in TrialEvents.TrialEvents)
            {
                var code = trialEvent.Code;

                foreach (var simStatus in SimStatusPerPlan)
                {
                    if (simStatus.PlanFeatures.ExpenseFeaturesDict.ContainsKey(code) == false)
                    {
                        throw new ApplicationException($"Invalid code '{code}': {_trialEventsCsvPath} uses code '{code}' which is not in {simStatus.PlanFeatures.PlanFeaturesCsvPath}");
                    }

                    int serviceCost;
                    simStatus.SimulateEvent(trialEvent, out serviceCost); // TODO: this should return SimResultRowOnePlan

                    if (firstPlan)
                    {
                        firstPlan = false;

                        var desc = simStatus.PlanFeatures.ExpenseFeaturesDict[code];
                        Console.Write($"{code},{desc},{serviceCost}");
                    }

                    // TODO: fix this and rest of calculations
                    int copay = 0;
                    int deductiblePaidYtd = 0;
                    int outOfPocketPaidYtd = 0;
                    Console.Write($",{copay},{deductiblePaidYtd},{outOfPocketPaidYtd}");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit: ");
            Console.ReadKey();
        }
    }
}



