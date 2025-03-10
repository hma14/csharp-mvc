using Omnae.Common;

namespace Omnae.BusinessLayer.Models
{
    public class NewPartResultModel
    {
        public int NewProductId { get; }
        public int NewTaskId { get; }
        public States State { get; }
        public int? BidRequestRevisionId { get; set; }
        public int? PartRevisionId { get; set; }
        public int? BidRFQStatusId { get; set; }

        public NewPartResultModel(int newProductId, int newTaskId, States state,
                                  int? bidRequestRevisionId = null,
                                  int? partRevisionId = null,
                                  int? bidRFQStatusId = null)
        {
            NewProductId = newProductId;
            NewTaskId = newTaskId;
            State = state;

            BidRequestRevisionId = bidRequestRevisionId;
            PartRevisionId = partRevisionId;
            BidRFQStatusId = bidRFQStatusId;
        }
    }
}