namespace Omnae.BusinessLayer.Models
{
    public class BidTimeLimitWillExpireEmailModel
    {
        public string UserName { get; set; }
        public string CustomerName { get; set; }

        public string ProductName { get; set; }
        public string ProductPartNumber { get; set; }
        public string ProductDescription { get; set; }

        public string TimeToExpire { get; set; }
        public string Url { get; set; }
    }
}