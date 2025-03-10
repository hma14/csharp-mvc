using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataProtection;

namespace Omnae.BusinessLayer
{
    public static class Startup
    {
        public static IDataProtectionProvider DataProtectionProvider { get; set; }
    }
}
