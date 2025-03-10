using Omnae.Context;
using Omnae.Model.Context;

namespace Omnae.Controllers
{
    public interface IHaveUserContext
    { 
        ILogedUserContext UserContext { get; }
    }
}