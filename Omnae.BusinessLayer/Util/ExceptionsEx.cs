using System.Data.Entity.Validation;
using System.Linq;

namespace Omnae.BusinessLayer.Util
{
    public static class ExceptionsEx
    {
        public static string RetrieveDbEntityValidationException(this DbEntityValidationException ex)
        {
            // Retrieve the error messages as a list of strings.
            var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

            // Join the list to a single string.
            var fullErrorMessage = string.Join("; ", errorMessages);
            return fullErrorMessage;
        }
    }
}
