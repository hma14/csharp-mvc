using System;
using System.Collections.Generic;

namespace Omnae.Notification.Model
{
    public class EmailMessage
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        public virtual string FromName { get; set; } = null;
        public virtual string FromEmail { get; set; } = null;

        public virtual string DestinationName { get; set; } = null;
        public virtual string DestinationEmail { get; set; } = null;
        public virtual string DestinationPhone { get; set; } = null;

        public virtual string Subject { get; set; }

        public virtual string Body { get; set; }

        public virtual bool ToAccounting { get; set; }
        public virtual List<string> ToEmails { get; set; }
        public virtual List<byte[]> AttachData { get; set; }
        public virtual List<string> AttachName { get; set; }

    }
}
