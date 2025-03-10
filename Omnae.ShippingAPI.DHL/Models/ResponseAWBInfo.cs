namespace Omnae.ShippingAPI.DHL.Models
{
    public class ResponseAWBInfo 
    {
        public string AWBNumber { get; set; }
        public string TrackedBy_LPNumber { get; set; }
        public string Status_ActionStatus { get; set; }
        public ResponseShipmentInfo ShipmentInfo { get; set; }
        public ResponsePieceDetails PieceDetails { get; set; }
    }
}
