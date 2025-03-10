using Omnae.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class DispositionRootCauseViewModel
    {
        public int ProductId { get; set; }
        public NC_DISPOSITION? Disposition { get; set; }

        [Display(Name ="Root Cause")]
        public NC_ROOT_CAUSE? RootCause { get; set; }

        [Display(Name = "Further Detailed Root Cause")]
        public string DetailRootCause { get; set; }
    }
}