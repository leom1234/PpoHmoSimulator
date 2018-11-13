using System;

namespace PPOSimulator
{
    public class PlanFeaturesFields
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int DefaultServiceCostDollars { get; set; }
        public bool IsCopay => CopayDollars.HasValue;
        public int? CopayDollars { get; set; }
        public int? CoinsurancePercent { get; set; }

        public PlanFeaturesFields(
            string code,
            string description,
            int defaultServiceCostDollars,
            int? copayDollars,
            int? coinsurancePercent)
        {
            Code = code;
            Description = description;
            DefaultServiceCostDollars = defaultServiceCostDollars;
            CopayDollars = copayDollars;
            CoinsurancePercent = coinsurancePercent;

            if (CopayDollars.HasValue == coinsurancePercent.HasValue)
                throw new ArgumentException("Copay or Coinsurance must be set, not both, not neither");
        }
    }
}