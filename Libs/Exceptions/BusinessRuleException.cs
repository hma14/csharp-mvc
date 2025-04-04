﻿using System;

namespace Libs.Exceptions
{
    public class BusinessRuleException: Exception
    {
        public BusinessRuleException() : base()
        {
        }

        public BusinessRuleException(string message) : base(message)
        {
        }

        public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
