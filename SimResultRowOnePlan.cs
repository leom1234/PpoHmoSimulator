namespace PPOSimulator
{
    public class SimResultRowOnePlan
    {
        public int PaidNowDollars { get; set; }
        public int PaidYtdDollars { get; set; }

        public SimResultRowOnePlan(
            int paidNowDollars,
            int paidYtdDollars)
        {
            PaidNowDollars = paidNowDollars;
            PaidYtdDollars = paidYtdDollars;
        }
    }
}