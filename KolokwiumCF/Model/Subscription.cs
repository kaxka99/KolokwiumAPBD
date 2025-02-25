﻿namespace KolokwiumCF.Model
{
    public class Subscription
    {
        public int IdSubscription { get; set; }
        public string Name { get; set; }
        public int RenewalPeriod { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<Sale> Sales { get; set; }
        
    }
}
