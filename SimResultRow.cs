using System;

namespace PPOSimulator
{
    public class SimResultRow
    {
        public string Code { get; }
        public string Description { get; }
        public int ServiceCostDollars { get; }
        public SimResultRowOnePlan[] SimResultPerPlan { get; }

        public SimResultRow(
            string code,
            string description,
            int serviceCostDollars,
            SimResultRowOnePlan[] simResultPerPlan)
        {
            Code = code;
            Description = description;
            ServiceCostDollars = serviceCostDollars;
            SimResultPerPlan = simResultPerPlan;
        }
    }
}