using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Omnae.ViewModels
{
    public class TaskOrderViewModel
    {
        public int TaskId { get; set; }
        public Order Order { get; set; }
    }
}