namespace PPOSimulator
{
    public class TrialEvent
    {
        public string Code { get; set; }
        public string Person { get; set; }
        public int? ServiceCostDollars { get; set; }

        public TrialEvent(
            string code,
            string person,
            int? serviceCostDollars = null)
        {
            Code = code;
            Person = person;
            ServiceCostDollars = serviceCostDollars;
        }
    }
}