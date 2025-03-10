using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Omnae.Common
{

    public enum MEASUREMENT_UNITS
    {
        PIECE = 0,

        // weight

        MILLI_GRAM = 1,
        GRAM = 2,
        KILO_GRAM = 3,
        
        TONNE = 4,  // metric ton
        US_TON = 5, // US ton
        UK_TON = 6, // UK ton

        // volume

        MILLI_LITER = 7,
        CENTI_LITER = 8,
        LITER = 9,

        POUND = 10,
        FLUID_OUNCE = 11,
        GALLON = 12,
        
        CUBIC_MILLI_METER = 13,
        CUBIC_CENTI_METER = 14,
        CUBIC_METER = 15,

        CUBIC_INCH = 16,
        CUBIC_FOOT = 17,
        CUBIC_YARD = 18,

        // Length

        MILLI_MITER = 19,
        CENTI_METER = 20,
        METER = 21,
        KILO_METER = 22,
        INCH = 23,
        FOOT = 24,
        YARD = 25,
        MILE = 26,

        // Time Period

        HOUR = 27,
    }

    public enum BUILD_TYPE
    {
        Prototype = 1,
        Production = 2,
        Both = 3,
        Assembly = 4,
        Process = 6,
        Unknown = 5,
    }



    public enum MATERIALS_TYPE
    {
        [Display(Name = "Precision Metals")]
        PrecisionMetals = 1,

        [Display(Name = "Precision Plastics")]
        PrecisionPlastics = 2,

        [Display(Name = "Membrane Switches")]
        MembraneSwitches = 3,

        [Display(Name = "Graphic Overlays")]
        GraphicOverlays = 4,

        Elastomers = 5,
        Labels = 6,

        [Display(Name = "Milled Stone")]
        MilledStone = 7,

        [Display(Name = "Milled Wood")]
        MilledWood = 8,

        [Display(Name = "Flex Circuits")]
        FlexCircuits = 9,

        [Display(Name = "Cable Assemblies")]
        CableAssemblies = 10,

        [Display(Name = "Tools")]
        Tools = 13,
        [Display(Name = "Machines")]
        Machines = 14,

        [Display(Name = "Flex Printing")]
        FlexPrinting = 15,

        [Display(Name = "Other Materials Type")]
        Other = 11,
        [Display(Name = "Unknown Materials Type")]
        Unknown = 12,

        

    }

    public enum Precision_Metal
    {
        Irons = 1,
        Steels = 2,
        Aluminums = 3,
        Coppers = 4,
        Titanium = 5,
        Zincs = 6,
        Others = 7,
    }
    public enum Precision_Plastics
    {
        ABS = 1,
        PLA = 2,
        Nylon = 3,
        Delron = 4,
        Polycarbonate = 5,
        Polyethelene = 6,
        Polyester = 7,
        PVC = 8,
        Acrylic = 9,
        Epoxy = 10,
        Other = 11,
    }


    public enum Metals_Processes
    {
        Machined = 1,
        Cast = 2,

        [Display(Name = "Cast And Machined")]
        CastAndMachined = 3,
        Extruded = 4,

        [Display(Name = "Extruded And Machined")]
        ExtrudedAndMachined = 5,

        [Display(Name = "Sheet Metal")]
        SheetMetal = 6,
    }
    public enum Plastics_Processes
    {
        [Display(Name = "One Shot Injection Mold")]
        OneShotInjectionMold = 1,

        [Display(Name = "Multi Shot Injection Mold")]
        MultiShotInjectionMold = 2,
        Overmold = 3,
        Machined = 4,

        [Display(Name = "3D Printed")]
        Printed3D = 5,
        Extruded = 6,

        [Display(Name = "Extruded And Machined")]
        ExtrudedAndMachined = 7,
    }

    public enum Metal_Type
    {
        Steel = 1,
        Iron = 2,
        Aluminium = 3,
        Zinc = 4,
        Titanium = 5,
        Copper = 6,
        Other = 7,
        Galvanneal = 8,

    }


    public enum Metals_Surface_Finish
    {
        [Display(Name = "Powder Coat")]
        PowderCoat = 1,
        Paint = 2,

        [Display(Name = "Anodized Aluminium")]
        AnodizedAluminium = 3,

        [Display(Name = "Hard Anodized Aluminium")]
        HardAnodizedAluminium = 4,
        Plated = 5
    }

    public enum Switches_Type
    {
        [Display(Name = "Membrane Switch")]
        MembraneSwitch = 1,

        [Display(Name = "Hybrid Keypad")]
        HybridKeypad = 2
    }

    public enum Print_Type
    {
        [Display(Name = "Silk Screen")]
        SilkScreen = 1,

        [Display(Name = "Digital Print")]
        DigitalPrint = 2
    }

    public enum Membrane_Switches_Attributes_Waterproof
    {
        No = 0,
        Yes = 1
    }
    public enum Membrane_Switches_Attributes_Embossing
    {
        No = 0,
        Yes = 1
    }
    public enum Membrane_Switches_Attributes_LEDLighting
    {
        No = 0,
        Yes = 1
    }
    public enum Membrane_Switches_Attributes_LED_EL_Backlighting
    {
        No = 0,
        Yes = 1
    }


    public enum Graphic_Overlays_Attributes_Embossing
    {
        No = 0,
        Yes = 1
    }
    public enum Graphic_Overlays_Attributes_SelectiveTexture
    {
        No = 0,
        Yes = 1
    }

    public enum USER_TYPE
    {
        Customer = 1,
        Vendor = 2,
        Admin = 3,
        Unknown
    }

    public enum FILE_PERMISSION
    {
        Admin = 1,
        User = 2,
        None = 3
    }

    public enum Process_Type
    {
        PowderCoating = 1,
        Painting = 2,
        Anodizing = 3,
        Plating = 4,
        Grinding = 5,
        Polishing = 6,
        Etching = 7,
        Inspection = 8,
        Calibration = 9,
        Printing_Graphics = 10,
        Packaging_Packout = 11,
    }

    public enum Anodizing_Type
    {
        Hard_Anodizing_Type_1 = 1,
        Hard_Anodizing_Type_2 = 2,
        Hard_Anodizing_Type_3 = 3,
    }

    public enum States
    {
        [Display(Name = "Create RFQ")]
        CreateRFQ = 1,

        [Display(Name = "Pending RFQ")]
        PendingRFQ = 2,

        [Display(Name = "Out For RFQ")]
        OutForRFQ = 3,

        [Display(Name = "Back From RFQ ")]
        BackFromRFQ = 4,

        [Display(Name = "RFQ Revision")]
        RFQRevision = 5,

        [Display(Name = "Vendor Cancelled RFQ")]
        VendorCancelledRFQ = 6,

        [Display(Name = "Quote Accepted")]
        QuoteAccepted = 7,

        [Display(Name = "Order Initiated")]
        OrderInitiated = 8,

        [Display(Name = "Payment Made")]
        PaymentMade = 9,

        [Display(Name = "Order Paid")]
        OrderPaid = 10,

        [Display(Name = "Proofing Started")]
        ProofingStarted = 11,

        [Display(Name = "Proofing Complete")]
        ProofingComplete = 12,

        [Display(Name = "Proof Approved")]
        ProofApproved = 13,

        [Display(Name = "Proof Rejected")]
        ProofRejected = 14,

        [Display(Name = "Tooling Started")]
        ToolingStarted = 15,

        [Display(Name = "Sample Started")]
        SampleStarted = 16,

        [Display(Name = "Sample Complete")]
        SampleComplete = 17,

        [Display(Name = "Sample Approved")]
        SampleApproved = 18,

        [Display(Name = "Sample Rejected")]
        SampleRejected = 19,

        [Display(Name = "Re-Order Initiated")]
        ReOrderInitiated = 20,

        [Display(Name = "Re-Order Payment Made")]
        ReOrderPaymentMade = 21,

        [Display(Name = "Re-Order Paid")]
        ReOrderPaid = 22,

        [Display(Name = "Production Started")]
        ProductionStarted = 23,

        [Display(Name = "Production Complete")]
        ProductionComplete = 24,


        // NCR States

        [Display(Name = "NCR Started")]
        NCRCustomerStarted = 25,

        [Display(Name = "NCR Root Cause Analysis")]
        NCRVendorRootCauseAnalysis = 26,

        [Display(Name = "NCR Approval")]
        NCRCustomerApproval = 27,

        [Display(Name = "NCR Reject Corrective Action")]
        NCRCustomerRejectCorrective = 28,

        [Display(Name = "NCR Reject Root Cause")]
        NCRCustomerRejectRootCause = 29,

        [Display(Name = "NCR Corrective Parts in Production")]
        NCRVendorCorrectivePartsInProduction = 30,

        [Display(Name = "NCR Corrective Parts Complete")]
        NCRVendorCorrectivePartsComplete = 31,

        [Display(Name = "NCR Customer Corrective Parts Received")]
        NCRCustomerCorrectivePartsReceived = 32,

        [Display(Name = "NCR Root Cause Disputes")]
        NCRRootCauseDisputes = 33,

        [Display(Name = "NCR Admin Disputes Intervention")]
        NCRAdminDisputesIntervention = 34,

        [Display(Name = "NCR Customer Revision Needed")]
        NCRCustomerRevisionNeeded = 35,

        [Display(Name = "NCR Closed")]
        NCRClosed = 36,

        // BID

        [Display(Name = "Bid for RFQ")]
        BidForRFQ = 37,

        [Display(Name = "RFQ Bid Review")]
        BidReview = 38,

        [Display(Name = "RFQ Bid Complete")]
        RFQBidComplete = 39,

        [Display(Name = "Pending RFQ Revision")]
        PendingRFQRevision = 40,

        [Display(Name = "Bid has timed out")]
        BidTimeout = 41,

        // Add Quantities

        [Display(Name = "Add Extra Quantities")]
        AddExtraQuantities = 42,

        VendorPendingInvoice = 43,


        // NCR Continue
        NCRCustomerCorrectivePartsAccepted = 44,
        NCRDamagedByCustomer = 45,

        //RFQ
        SetupMarkupExtraQty = 46,

        // Completed Bid Review and send notification to selected vendor
        BidReviewed = 47,

        // Customer cancel Order
        [Display(Name = "Customer Requests to Cancel Order")]
        CustomerCancelOrder = 48,

        [Display(Name = "Order Was Cancelled")]
        OrderCancelled = 49,

        [Display(Name = "Cancel Order Denied")]
        OrderCancelDenied = 50,

        // New RFQ Bid States

        [Display(Name = "Customer Cancelled RFQ")]
        CustomerCancelledRFQ = 51,

        [Display(Name = "Vendor Rejected RFQ")]
        VendorRejectedRFQ = 52,       

        [Display(Name = "Review RFQ")]
        ReviewRFQ = 53,

        [Display(Name = "Review RFQ Accepted")]
        ReviewRFQAccepted = 54,

        [Display(Name = "RFQ No Response")]
        RFQNoResponse = 55,

        [Display(Name = "RFQ Failed")]
        RFQFailed = 56,

        [Display(Name = "Update Quantities in RFQ Review")]
        RFQReviewUpdateQuantity = 57,

        [Display(Name = "Update Quantities in RFQ Bid")]
        RFQBidUpdateQuantity = 58,

        [Display(Name = "Keeps Current RFQ Revision")]
        KeepCurrentRFQRevision = 59,

    }

    public enum Triggers
    {

        PendingnRFQ = 1,

        AssignRFQ,
        RevisingRFQ,

        RevisionRequired,
        RFQCancelled,

        ReadyToOrder,
        PendingOrder,
        UnsuccessfulQuote,

        PendingPaymentMade,
        //PendingPaymentReceived,
        PendingProof,
        ReadyToProof,
        StartedProof,

        ProofApproval,
        ProofAcceptancePending,


        ApprovedProof,
        ReadyForTooling,

        CorrectingProof,
        ProofRejected,

        InTooling,
        ToolingStarted,
        ToolingComplete,

        CompleteSample,
        SampleAcceptancePending,

        ApproveSample,
        ReadyForProduction,

        CorrectingSample,
        RejectedSample,

        InProduction,
        CompleteProduction,
        PendingReorderPaymentMade,
        OrderPaid,
        PaidReOrder,

        // NCR

        NCRStarted,
        NCRAnalysisRootCause, //Vendor
        NCRApproval,
        NCRRejectCorrectiveAction,
        NCRRejectRootCause,
        NCRArbitrateDispute,

        NCRCorrectiveReceivedApproved,
        NCRCorrectiveReceivedRejected,
        NCRCorrectivePartsComplete, //Customer
        NCRCorrectivePartsReceival,

        NCRRootCauseOnCustomer,
        NCRAdminArbitrateDispute,
        NCRArbitrateVendorCause,
        NCRArbitrateCustomerCause,
        NCRArbitrateCustomerCauseDamage,
        NCRCustomerRevision,
        NCRCorrectiveAccepted,

        NCRClose,

        // BID
        BiddingRFQ,
        ReviewRFQBid,
        CompleteRFQBid,
        BidOverDeadline,

        // Extra Quantities
        SetupUnitPricesForExtraQuantities,

        CreateInvoiceForVendor,
        CompleteInvoiceForVendor,

        SetupMarkupForExtraQty,

        // Customer Cancel Order request
        CustomerCancelOrderRequest,
        VendorDenyCancelOrderRequest,
        VendorAcceptCancelOrderRequest,

        // for new RFQ Bid States
        VendorReviewRFQ,
        VendorAcceptRFQ,
        VendorRejectRFQ,
        UpdatedQuantities,
    }

    public enum COUNTRY
    {
        CANADA = 40,

        [Display(Name = "UNITED STATES")]
        UNITED_STATES = 236,

        OTHERS = 500
    }

    public enum CANADA_PROVINCES
    {
        Alberta = 58,

        [Display(Name = "British Columbia")]
        British_Columbia,

        Manitoba,

        [Display(Name = "New Brunswick")]
        New_Brunswick,

        [Display(Name = "Newfoundland and Labrador")]
        Newfoundland_and_Labrador,

        [Display(Name = "Northwest Territories")]
        Northwest_Territories,

        [Display(Name = "Nova Scotia")]
        Nova_Scotia,

        Nunavut,
        Ontario,

        [Display(Name = "Prince Edward Island")]
        Prince_Edward_Island,

        Québec,
        Saskatchewan,

        [Display(Name = "Yukon Territory")]
        Yukon_Territory
    }

    public enum DISTANCE_UNIT
    {
        cm,
        IN,
        ft,
        mm,
        m,
        yd
    }

    public enum MASS_UNIT
    {
        g,
        oz,
        lb,
        kg
    }

    /* 
     * Additional information: Invalid currency: cnd. Stripe currently supports these currencies: 
     * 
     * usd, aed, afn, all, amd, ang, aoa, ars, aud, awg, azn, bam, bbd, bdt, bgn, bif, bmd, bnd, 
     * bob, brl, bsd, bwp, bzd, cad, cdf, chf, clp, cny, cop, crc, cve, czk, djf, dkk, dop, dzd, 
     * egp, etb, eur, fjd, fkp, gbp, gel, gip, gmd, gnf, gtq, gyd, hkd, hnl, hrk, htg, huf, idr, 
     * ils, inr, isk, jmd, jpy, kes, kgs, khr, kmf, krw, kyd, kzt, lak, lbp, lkr, lrd, lsl, ltl, 
     * mad, mdl, mga, mkd, mnt, mop, mro, mur, mvr, mwk, mxn, myr, mzn, nad, ngn, nio, nok, npr, 
     * nzd, pab, pen, pgk, php, pkr, pln, pyg, qar, ron, rsd, rub, rwf, sar, sbd, scr, sek, sgd, 
     * shp, sll, sos, srd, std, svc, szl, thb, tjs, top, try, ttd, twd, tzs, uah, ugx, uyu, uzs, 
     * vnd, vuv, wst, xaf, xcd, xof, xpf, yer, zar, zmw, eek, lvl, vef
     * 
     * 
     */
    public class Currency
    {
        public const string USD = "USD";
        public const string CAD = "CAD";
    }

    public enum PAYMENT_METHODS
    {
        CreditCard = 1,
        Term,
        Cheque,
        Wire,
        Others
    }

    public enum DOCUMENT_TYPE
    {
        PRODUCT_2D_PDF = 1,
        PRODUCT_3D_STEP = 2,
        PO_PDF = 3,
        QUOTE_PDF = 4,
        PROOF_PDF = 5,
        REVISING_DOCS = 6,
        QBO_ESTIMATE_PDF = 7,
        QBO_INVOICE_PDF = 8,
        QBO_PURCHASEORDER_PDF = 9,
        QBO_BILL_PDF = 10,
        CORRESPOND_PROOF_REJECT_PDF = 11,
        CORRESPOND_SAMPLE_REJECT_PDF = 12,
        PACKING_SLIP_PDF = 13,
        PACKING_SLIP_INSPECTION_REPORT_PDF = 14,
        ENTERPRISE_VENDOR_INVOICE_PDF = 15,
        PAYMENT_PROOF = 16,
    }

    public enum CUSTOMER_MISSING_DOCUMENT_TYPE
    {
        PRODUCT_2D_PDF = 1,
        PRODUCT_3D_STEP = 2,
        PO_PDF = 3,
        PROOF_PDF = 5,
        REVISING_DOCS = 6,
        CORRESPOND_PROOF_REJECT_PDF = 11,
        CORRESPOND_SAMPLE_REJECT_PDF = 12,
        PRODUCT_AVATAR = 17,
    }

    public enum FILTERS
    {
        Tagged = 1,
        Alert,
        RevisonRequired,
        Proofing,
        Sampling,
        InProduction,
        RemoveFilter,
    }

    public enum NCR_FILTERS
    {
        NoFilter = 0,
        Year,
        Vendor,
        Customer,
        Administrator,
        Product
    }

    public enum INVOICE_FILTERS
    {
        NoFilter = 0,
        Year,
        Vendor,
        Customer,
        Administrator,
        Product
    }

    public enum NC_DETECTED_BY
    {
        OMNAE = 1,
        CUSTOMER,
        RMA_REQUESTED,
        AUDIT_FINDING
    }

    public class DETECTED_BY
    {
        public const string CUSTOMER = "Customer";
        public const string OMNAE = "Omnae";
        public const string RMA_REQUESTED = "RMA_Requested";
        public const string AUDIT_FINDING = "Audit_Finding";
    }

    public class DHL_SHIPPING_PRODUCT_NAME
    {
        public const string EXPRESS_WORLDWIDE_NONDOC = "EXPRESS WORLDWIDE NONDOC";
    }

    public enum NC_ROOT_CAUSE
    {
        VENDOR = 1,
        [Display(Name = "CUSTOMER (Engineering)")]
        CUSTOMER
    }

    public enum NC_DISPOSITION
    {
        REWORK = 1,
        REPLACE = 2,
        NO_ACTION_OR_SCRAP = 3
    }

    public enum COUNTRY_ID
    {
        CA = 40,
        CN = 46,
        US = 236,
    }

    public enum STATE_PROVINCE_CODE
    {
        US_STATE_CODE = 226,
        CA_PROVINCE_CODE = 38
    }

    public enum CUSTOMER_PRIORITY
    {
        FAST = 1,
        QUALITY,
        PRICE,
    }

    public enum NCR_IMAGE_TYPE
    {
        EVIDENCE = 1,
        VENDOR_CAUSE_REF,
        CUSTOMER_CAUSE_REF,
        ROOT_CAUSE_ON_CUSTOMER,
        ARBITRATE_VENDOR_CAUSE_REF,

        ARBITRATE_CUSTOMER_DAMAGE_REF,
        ARBITRATE_CUSTOMER_CAUSE_REF,

        ROOT_CAUSE_ON_VENDOR,

        PACKING_SLIP_INSPECTION_REPORT_PDF = 14,


    }

    public enum SHIPPING_CARRIER_TYPE
    {
        Air = 1,
        Ocean,
        Land,
        Unknown = -1,
    }

    public enum SHIPPING_CARRIER
    {
        FedEx = 1,
        Purolator,
        UPS,
        DHL,
        Evergreen,
        HamburSud,
        EMS,
        DanFoss,
        StraitExpress,
    }

    public enum CUSTOMER_TYPE
    {
        Subscriber = 1,
        Reseller = 2,
    }


    public class COMMON_MAX
    {
        public const int PRICE_BREAK_QUANTITY_MAX = 6;
        public const int PRICE_BREAK_QUANTITY_STEP = 100;
        public const float DEFAULT_MARKUP = 1.5f;
        public const int MAX_VENDORS_FOR_RFQ = 20;
    }

    public class TIMER_UNIT
    {
        public const string MINUTE = "minute";
        public const string HOUR = "hour";
        public const string DAY = "day";
        public const string MONTH = "month";
        public const string YEAR = "year";
    }

    public enum TimerUnit
    {
        [Description(TIMER_UNIT.MINUTE)]
        Minute,
        [Description(TIMER_UNIT.HOUR)]
        Hour,
        [Description(TIMER_UNIT.DAY)]
        Day,
        [Description(TIMER_UNIT.MONTH)]
        Month,
        [Description(TIMER_UNIT.YEAR)]
        Year
    }



    public enum TypeOfTimers
    {
        RFQRevisionTimer = 1,
        BidTimer = 2,
    }

    public enum ActionFlag
    {
        ExtendTimeLimit = 1,
        AssignNewVendors = 2,
    }


    public class IndicatingMessages
    {
        public const string CreateAccountForCustomerSuccess = "A new account has been created for customer";
        public const string CreateAccountForCustomerFailed = "Failed: ";
        public const string InvalidAccess = "Invalid access!";
        public const string ProductNotFound = "Product was not found!";
        public const string TaskNotFound = "Task was not found!";
        public const string OrderNotFound = "Order was not found!";
        public const string ParameterIsNull = "Input parameter is null!";
        public const string PartHasNotBeenOrderedYet = "Part Has Not Been Ordered Yet!";
        public const string NCRCausedByCustomer = "NCRs Caused by Customer";
        public const string NCRCausedByVendor = "NCRs Caused by Vendor";
        public const string TotalQuantity = "Total Quantity";
        public const string NotFound = "Object was not found in database";
        public const string SmsWarningMsg = "Since it is not a valid mobile phone number, notifying customer with SMS failed. With email succeeded, you may ignore this message and continue onward.";
        public const string NoCellPhoneWarningMsg = "Since you didn't provide you cell phone number during the registration, notifying with SMS failed. With email succeeded, you may ignore this message and continue onward.";
        public const string VendorHaseBeenSelected = "Vendor has already been selected";
        public const string VendorNotFound = "Vendor could not be found";
        public const string CustomerNotFound = "Customer could not be found";
        public const string PriceBreaksNotFound = "Price breaks could not be found.";
        public const string ForgotRevisionReason = "Forgot to enter Revision Reasons";
        public const string IndexOutOfRange = "Index is out of range";
        public const string DuplicateOrder = "Placing a duplicate order";
        public const string ForgotUploadFile = "Forgot to select file(s) to upload?";
        public const string UploadFileFailed = "Upload file failed";
        public const string NCRNotFound = "NCR not found";
        public const string MissingFormData = "Missing form data";
        public const string MissingShppingAddress = "Missing shipping address";
        public const string TaskStateMismatch = "Task state is mismatching";
        public const string Success = "Operation Succeeds";
        public const string InvalidDocumentType = "Invalid Document Type";
        public const string UploadDocOnInvalidState = "Uploading document on invalid state";
        public const string QuantityOrderHasNotBeenPaid = "Quantity order has not been paid, please make payment for your quatity order first then come back to continue";
        public const string YouMustWaitUntilCustomerCompleteRevision = "You must wait until customer completes revivision";
        public const string YouCannotCreateRevision = "You cannot create a part revision, because you are not owner of this part";
        public const string YouHaveNoPermissionToOrderThisPart = "You have no permission to order this part or you're sharing this part";
        public const string ThisProductIsNotInCorrectStateToAllowSharing = "This part of the task is not in correct state to allow sharing";
        public const string DontHavePrivilege = "You don't have privilege";
        public const string CompanyNotFound = "Company not found";
        public const string CompanyNotAuthorized = "Company not authorized for action";
        public const string ProductAlreadyShared = "Product already shared with this company";
        public const string AddExtraProductDocNotAllowed = "You're not allowed to add extra product documents if they are already existing.";
        public const string NoExpeditedShipmentRequested = "No expedited shipment request found for this order";
        public const string EntityAlreadyExists = "Entity already exists in database";
        public const string ForgotRejectReason = "Forgot to enter Reject Reasons";
        public const string UnknownType = "Unknown type";
        public const string TimerIntervalCouldnotBeFound = "Timer interval couldn't be found";
        public const string ProductNotAssignedYet = "Product hasn't assigned to any vendor yet";
        public const string DuplicatedRFQ = "Trying to create a RFQ which already exist in RFQ bidding stage";
        public const string DuplicatedProduct = "Trying to create a duplicated Product";

    }

    public class BidFailedReason
    {
        public static readonly string[] Reasons =
        {
            "-- Choose a failed reason if you didn't choose this vendor --",
            "Unit price is too high.",
            "Product lead time takes too long.",
            "Tooling charge is too high.",
            "Unit price is okay but product lead time takes too long.",
            "Product lead time is okay but unit price is too high.",
            "Shipping unit price is too high.",
            "Shipping takes too long time.",
            "Quality is more important."
        };
    }

    public class CustomerBidPreference
    {
        public static readonly string[] Preferences =
        {
            "-- Choose a Set of Preferences for Bid --",
            "1. FAST, 2. QUALITY, 3. PRICE",
            "1. FAST, 2. PRICE, 3. QUALITY",
            "1. QUALITY, 2. FAST, 3. PRICE",
            "1. QUALITY, 2. PRICE, 3. FAST",
            "1. PRICE, 2. QUALITY, 3. FAST",
            "1. PRICE, 2. FAST, 3. QUALITY",
        };
    }

    public class ChartColors
    {
        public static readonly string[] TotalCustomerNcrs =
                    {
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    "rgba(245, 127, 23, 0.5)",
                    };


        public static readonly string[] TotalVendorNcrs =
                    {
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    "rgba(210, 34, 87, 0.5)",
                    };

        public static readonly string[] TotalQuantity =
                    {
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    "rgba(102, 187, 106, 0.5)",
                    };

        public static readonly Dictionary<string, string> PieChartMonthColors = new Dictionary<string, string>
            {
                {"January", "#FF0000"},
                {"February", "#800000"},
                {"March", "#808000"},
                {"April", "#808080"},
                {"May", "#800080"},
                {"June", "#800080"},
                {"July", "#000080"},
                {"August", "#999999"},
                {"September", "#E9967A"},
                {"October", "#CD5C5C"},
                {"November", "#1A5276"},
                {"December", "#27AE60"},
            };
    }



    public class QBOReport
    {
        public const string VENDOR_BALANCE = "VendorBalance";
        public const string CUSTOMER_BALANCE = "CustomerBalance";
    }

    public class Administrator_Account
    {
        public const string Name = "OMNAE";
    }

    public class OMNAE_WEB
    {
        public const string Admin = "Administrator";
    }

    public class CANADA_PROVINCE_NAME
    {
        public const string AB = "AB";
        public const string BC = "BC";
        public const string MB = "MB";
        public const string NB = "NB";
        public const string NL = "NL";
        public const string NT = "NT";
        public const string NS = "NS";
        public const string NU = "NU";
        public const string ON = "ON";
        public const string PE = "PE";
        public const string QC = "QC";
        public const string SK = "SK";
        public const string YT = "YT";
    }

    public class COUNTRY_NAME
    {
        public const string Canada = "Canada";
        public const string USA = "United States";
    }

    public class TAX_TYPE
    {
        public const string GST = "GST";
        public const string HST_ON = "HST ON";
        public const string HST_NL = "HST NL 2016";
        public const string HST_NS = "HST NS";
        public const string HST_PE = "HST PE 2016";
        public const string PST_BC = "PST BC";
        public const string HST_NB = "HST NB 2016";


        // not existing in QBO
        public const string HST_MB = "HST MB";
        public const string HST_QC = "HST QC";
        public const string HST_SK = "HST SK";
        public const string Exempt = "Exempt";
    }

    public class TAX_RATE
    {
        public const double BC_TAX = 0.05;
        public const double ON_TAX = 0.13;
        public const double MB_TAX = 0.13;
        public const double NB_TAX = 0.15;
        public const double NL_TAX = 0.15;
        public const double NS_TAX = 0.15;
        public const double PE_TAX = 0.15;
        public const double QC_TAX = 0.14957;
        public const double SK_TAX = 0.11;
        public const double OTHER_TAX = 0.05;
        public const double EXEMPT_TAX = 0d;
    }

    public class TAX_RATE_PERCENTAGE
    {
        public const string BC_TAX = "5%";
        public const string ON_TAX = "13%";
        public const string MB_TAX = "13%";
        public const string NB_TAX = "15%";
        public const string NL_TAX = "15%";
        public const string NS_TAX = "15%";
        public const string PE_TAX = "15%";
        public const string QC_TAX = "14.957%";
        public const string SK_TAX = "11%";
        public const string OTHER_TAX = "5%";
    }

    public class CHART_CONFIG
    {
        public const int MaxVendorsToChoose = 16;
        public const int MaxDateRange = 16;
        public const int MaxTotalQuantity = 16;
        public const int MaxTotalCustomerNcrs = 16;
        public const int MaxTotalVendorNcrs = 16;
    }


    /// <summary>
    /// Enumeration of ISO 4217 currency codes, indexed with their respective ISO 4217 numeric currency codes. 
    /// Only codes support in .Net with RegionInfo objects are listed
    /// </summary>
    public enum CurrencyCodes
    {
        AFN = 971,
        EUR = 978,
        ALL = 008,
        DZD = 012,
        USD = 840,
        AOA = 973,
        XCD = 951,
        ARS = 032,
        AMD = 051,
        AWG = 533,
        AZN = 944,
        BSD = 044,
        BHD = 048,
        BDT = 050,
        BBD = 052,
        BYN = 933,
        BZD = 084,
        XOF = 952,
        BMD = 060,
        INR = 356,
        BTN = 064,
        BOB = 068,
        BOV = 984,
        BAM = 977,
        BWP = 072,
        NOK = 578,
        BRL = 986,
        BND = 096,
        BGN = 975,
        BIF = 108,
        CVE = 132,
        KHR = 116,
        XAF = 950,
        CAD = 124,
        KYD = 136,
        CLP = 152,
        CLF = 990,
        CNY = 156,
        AUD = 036,
        COP = 170,
        COU = 970,
        KMF = 174,
        CDF = 976,
        NZD = 554,
        CRC = 188,
        HRK = 191,
        CUP = 192,
        CUC = 931,
        ANG = 532,
        CZK = 203,
        DKK = 208,
        DJF = 262,
        DOP = 214,
        EGP = 818,
        SVC = 222,
        ERN = 232,
        SZL = 748,
        ETB = 230,
        FKP = 238,
        FJD = 242,
        XPF = 953,
        GMD = 270,
        GEL = 981,
        GHS = 936,
        GIP = 292,
        GTQ = 320,
        GBP = 826,
        GNF = 324,
        GYD = 328,
        HTG = 332,
        HNL = 340,
        HKD = 344,
        HUF = 348,
        ISK = 352,
        IDR = 360,
        XDR = 960,
        IRR = 364,
        IQD = 368,
        ILS = 376,
        JMD = 388,
        JPY = 392,
        JOD = 400,
        KZT = 398,
        KES = 404,
        KPW = 408,
        KRW = 410,
        KWD = 414,
        KGS = 417,
        LAK = 418,
        LBP = 422,
        LSL = 426,
        ZAR = 710,
        LRD = 430,
        LYD = 434,
        CHF = 756,
        MOP = 446,
        MKD = 807,
        MGA = 969,
        MWK = 454,
        MYR = 458,
        MVR = 462,
        MRU = 929,
        MUR = 480,
        XUA = 965,
        MXN = 484,
        MXV = 979,
        MDL = 498,
        MNT = 496,
        MAD = 504,
        MZN = 943,
        MMK = 104,
        NAD = 516,
        NPR = 524,
        NIO = 558,
        NGN = 566,
        OMR = 512,
        PKR = 586,
        PAB = 590,
        PGK = 598,
        PYG = 600,
        PEN = 604,
        PHP = 608,
        PLN = 985,
        QAR = 634,
        RON = 946,
        RUB = 643,
        RWF = 646,
        SHP = 654,
        WST = 882,
        STN = 930,
        SAR = 682,
        RSD = 941,
        SCR = 690,
        SLL = 694,
        SGD = 702,
        XSU = 994,
        SBD = 090,
        SOS = 706,
        SSP = 728,
        LKR = 144,
        SDG = 938,
        SRD = 968,
        SEK = 752,
        CHE = 947,
        CHW = 948,
        SYP = 760,
        TWD = 901,
        TJS = 972,
        TZS = 834,
        THB = 764,
        TOP = 776,
        TTD = 780,
        TND = 788,
        TRY = 949,
        TMT = 934,
        UGX = 800,
        UAH = 980,
        AED = 784,
        USN = 997,
        UYU = 858,
        UYI = 940,
        UYW = 927,
        UZS = 860,
        VUV = 548,
        VES = 928,
        VND = 704,
        YER = 886,
        ZMW = 967,
        ZWL = 932,
        XBA = 955,
        XBB = 956,
        XBC = 957,
        XBD = 958,
        XTS = 963,
        XXX = 999,
        XAU = 959,
        XPD = 964,
        XPT = 962,
        XAG = 961

    }

    public enum IS_TERM
    {
        NO = 0,
        YES,
    }

    public enum EXPEDITED_SHIPMENT_TYPE
    {
        PRIOR_TO = 1,
        DELAYED = 2,
        ON_TIME = 3,
    }

    public enum EXPEDITED_SHIPMENT_RESPONSE
    {
        DENY = 1,
        ACCEPT = 2,
    }

    public enum REASON_TYPE
    {
        RFQ_REVISION_REQUEST = 1,
        REJECT_RFQ = 2,
        CUSTOMER_CANCEL_RFQ = 3,

    }

}