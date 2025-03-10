using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omnae.BusinessLayer.Models;

namespace Omnae.ViewModels
{
    public class TaskDataModalViewModel
    {
        public TaskViewModel TaskVM { get; set; }
        public string ProofingDocUri { get; set; }
    }
}