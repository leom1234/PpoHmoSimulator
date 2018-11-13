using System;
using System.Collections.Generic;

namespace PPOSimulator
{
    public class SimStatusOnePlan
    {
        public People People {  get; }
        public PlanFeatures PlanFeatures { get; }
        public TrialEventSet TrialEvents { get; }
        public Dictionary<string, int> DeductiblePaidYtd { get; }
        public Dictionary<string, int> OutOfPocketYtd { get; }

        public bool DeductibleMetForPerson(string person) => DeductiblePaidYtd[person] >= PlanFeatures.DeductiblePerPerson;

        public double TotalDeductiblePaidForFamily
        {
            get
            {
                double sumOfDeductiblesPaid = 0;
                foreach (var person in People.Names)
                {
                    sumOfDeductiblesPaid += DeductiblePaidYtd[person];
                }

                return sumOfDeductiblesPaid;
            }
        }

        public bool DeductibleMetForFamily => TotalDeductiblePaidForFamily >= PlanFeatures.DeductiblePerFamily;

        public SimStatusOnePlan(
            People people,
            PlanFeatures planFeatures,
            TrialEventSet trialEvents)
        {
            People = people;
            PlanFeatures = planFeatures;
            TrialEvents = trialEvents;
            DeductiblePaidYtd = new Dictionary<string, int>();
            OutOfPocketYtd = new Dictionary<string, int>();

            foreach (var person in People.Names)
            {
                DeductiblePaidYtd.Add(person, 0);
                OutOfPocketYtd.Add(person, 0);
            }
        }

        public void SimulateEvent(TrialEvent trialEvent)
        {
            if (PlanFeatures.ExpenseFeaturesDict.ContainsKey(trialEvent.Code) == false)
            {
                throw new ApplicationException($"Invalid code '{trialEvent.Code}': {TrialEvents.TrialEventsCsvPath} uses code '{trialEvent.Code}' which is not in {PlanFeatures.PlanFeaturesCsvPath}");
            }

            var expenseFeatures = PlanFeatures.ExpenseFeaturesDict[trialEvent.Code];
            int cost = expenseFeatures.DefaultServiceCostDollars;
            if (trialEvent.ServiceCostDollars.HasValue)
            {
                cost = trialEvent.ServiceCostDollars.Value;
            }

            float copayOrCoinsuranceDollars = (float)(expenseFeatures.IsCopay
                ? expenseFeatures.CopayDollars
                : cost * expenseFeatures.CoinsurancePercent / 100f);

            ProcessStdPPOExpense(trialEvent.Code, trialEvent.Person, expenseFeatures.IsCopay, cost, copayOrCoinsuranceDollars);

        }


        /// <summary>
        /// Calculate everything that happens when a medical expense occurs.
        /// Return the amount you pay out of pocket for this visit.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="isCopay">True for Rx, Visit, lab, etc., false for surgery, scan, etc.)</param>
        /// <param name="serviceCost"></param>
        /// <param name="copayOrCoinsuranceDollars"></param>
        /// <returns></returns>
        public float ProcessStdPPOExpense(string code, string person, bool isCopay, int serviceCost, float copayOrCoinsuranceDollars)
        {
            bool deductibleApplies = !isCopay;
            float deductiblePaidThisTime = 0;
            float outOfPocketThisTime = 0;

            if (deductibleApplies)
            {
                if (DeductibleMetForPerson(person) || DeductibleMetForFamily)
                {
                    outOfPocketThisTime = copayOrCoinsuranceDollars;
                }
                else
                {
                    deductiblePaidThisTime += serviceCost;
                }
            }
            else
            {
                outOfPocketThisTime = copayOrCoinsuranceDollars;
            }

            //TODO ... if ()
            Console.WriteLine($"");

            return outOfPocketThisTime;
        }

    }
}