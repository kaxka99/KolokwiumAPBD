namespace KolokwiumCF.Model
{
    public class Payment
    {
        public int IdPayment { get; set; }
        public DateTime Date { get; set; }
        public int IdClient { get; set; }
        public int IdSubscription { get; set; }
        public decimal Value { get; set; }

        public Client Client { get; set; }
        public Subscription Subscription { get; set; }
    }
}
