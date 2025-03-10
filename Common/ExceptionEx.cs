using System;

namespace Common
{
    public static class ExceptionEx
    {
        public static string RetrieveErrorMessage(this Exception ex)
        {
            if (ex.InnerException?.InnerException != null)
            {
                return ex.InnerException.InnerException.Message;
            }

            return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        }
    }
}