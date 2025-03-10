using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using FluentAssertions.Mvc;
using Moq;
using Omnae.BusinessLayer.Identity;
using Omnae.Common;
using Omnae.Context;
using Omnae.Controllers;
using Omnae.Data;
using Omnae.Model.Security;
using Omnae.Models;
using Omnae.ViewModels;
using TechTalk.SpecFlow;
using Unity;

namespace Omnae.Web.Tests.Steps
{
    [Binding]
    public class MultiCustomerLoginsSteps
    {
        public MultiCustomerLoginsSteps(LogedUserContext userContext, AccountController accountController, ApplicationUserManager userManager, OmnaeContext dbContext, IUnityContainer container, HomeController homeController)
        {
            UserContext = userContext;
            UserManager = userManager;
            DbContext = dbContext;
            Container = container;
            HomeController = homeController;
            AccountController = accountController;

            var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            request.SetupGet(x => x.ApplicationPath).Returns("/");
            request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/a", UriKind.Absolute));
            request.SetupGet(x => x.ServerVariables).Returns(new System.Collections.Specialized.NameValueCollection());

            var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            response.Setup(x => x.ApplyAppPathModifier("/post1")).Returns("http://localhost/post1");

            var context = new Mock<HttpContextBase>(MockBehavior.Strict);
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Response).Returns(response.Object);
            context.SetupGet(x => x.Session).Returns(new HttpSessionStateWrapper(HttpContext.Current.Session));
            context.SetupGet(x => x.User).Returns(HttpContext.Current.User);

            HomeController.ControllerContext = new ControllerContext(context.Object, new RouteData(), HomeController);
            AccountController.ControllerContext = new ControllerContext(context.Object, new RouteData(), AccountController);
        }

        private IUnityContainer Container { get; }

        private LogedUserContext UserContext { get; set; }
        private ApplicationUserManager UserManager { get; }
        private OmnaeContext DbContext { get; }
        private AccountController AccountController { get; }
        private HomeController HomeController { get; }

        //////////////////////////////////////////////////////////////

        private string LastUserIdCreated { get; set; } = null;
        private int LastCompanyIdCreated { get; set; }

        //////////////////////////////////////////////////////////////


        [Given(@"A new Customer is self creating a user in Omnae System")]
        public async Task WhenANewCustomerIsSelfCreatingAUserInOmnaeSystem()
        {
            //Preconditions:
            //No User shoud be Loged
            UserContext.Should().NotBeNull();
            UserContext.UserId.Should().BeNull();

            //Create a new User
            var model = new CreateAccountForCustomerViewModel()
            {
                Resgister = new RegisterViewModel()
                {
                    Email = "theFirstUser@newCompany.com",
                    FirstName = "FirtName",
                    LastName = "User",
                    PhoneNumber = "+15555555555",
                    Password = "thisIsAPWD123$",
                    ConfirmPassword = "thisIsAPWD123$"
                }
            };
            ActionResult result = await AccountController.Register(model);
            result.Should().BeViewResult().WithViewName("Info");

            //User must created
            LastUserIdCreated = AccountController.ViewBag?.UserId?.ToString();
            LastUserIdCreated.Should().NotBeNullOrEmpty();

            //Force the User Be Logged
            HttpContext.Current.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.NameIdentifier, LastUserIdCreated)}, "unitTest"));
            UserContext = Container.Resolve<LogedUserContext>();

            //Confirm the user
            var token = AccountController.ViewBag?.TokenCode?.ToString();
            ActionResult result2 = await AccountController.ConfirmEmail(LastUserIdCreated, token);
            result2.Should().BeRedirectToRouteResult().WithController("Home").WithAction("ContinueRegistration");

            //Create the Company
            var company = new ContinueRegistrationViewModel()
            {
                //UserId = LastUserIdCreated,
                CompanyName = "TestCompany",
                AddressLine1 = "Addr1",
                AddressLine2 = "Addr2",
                City = "TestCity",
                ZipCode = "ABC123",
                CountryId = 40,
                ProvinceId = 59,
                IsBilling = false,
                IsShipping = false,
                Attention = "AttentionTest",
            };
            ActionResult result3 = await HomeController.ContinueRegistration(company, null, null);
            result3.Should().BeRedirectToRouteResult().WithAction("Index");
            var companyId = (int) HomeController.ViewBag.CompanyId;
            companyId.Should().NotBe(0);

            UserContext = Container.Resolve<LogedUserContext>();

            LastCompanyIdCreated = companyId;
            //TODO Emails was send
        }


        [When(@"a new user is created for this Customer and the company data")]
        public void ThenANewUserIsCreatedForThisCustomerAndTheCompanyData()
        {
            LastUserIdCreated.Should().NotBeNullOrEmpty();

            UserContext.Should().NotBeNull();
            UserContext.UserId.Should().NotBeNull();
            UserContext.UserId.Should().Be(LastUserIdCreated);

            UserContext.UserType.Should().Be(USER_TYPE.Customer);

            UserContext.Company.Should().NotBeNull();
            UserContext.Company.Id.Should().Be(LastCompanyIdCreated);
        }
        
        [Then(@"this user should be a ""(.*)"" acount")]
        public async Task ThenThisUserShouldBeAAcount(string role)
        {
            var allUserRoles = await UserManager.GetRolesAsync(LastUserIdCreated);

            UserContext.Should().NotBeNull();
            UserContext.Roles.Should().NotBeNull();
            UserContext.Roles.Should().Contain(Roles.CompanyAdmin);
            UserContext.Roles.Should().Contain(allUserRoles);
        }
        
        [Then(@"this user should be associate to this Customer")]
        public void ThenThisUserShouldBeAssociateToThisCustomer()
        {
            UserContext.Should().NotBeNull();
            UserContext.Company.Should().NotBeNull();
            Debug.Assert(UserContext.Company != null, "UserContext.Company != null");

            UserContext.Company.Id.Should().Be(LastCompanyIdCreated);
        }
        
        ///////////////////////////////
        
        [Given(@"A customer with existing user account associated with email address or email domain")]
        public void GivenACustomerWithExistingUserAccountAssociatedWithEmailAddressOrEmailDomain()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"this customer attempts to create a new account")]
        public void WhenThisCustomerAttemptsToCreateANewAccount()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"The system will reject the new account creation")]
        public void ThenTheSystemWillRejectTheNewAccountCreation()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"Customer will be notified that the account already exists")]
        public void ThenCustomerWillBeNotifiedThatTheAccountAlreadyExists()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"associated account admin will be notified of the atempted account creation, given option to approve user")]
        public void ThenAssociatedAccountAdminWillBeNotifiedOfTheAtemptedAccountCreationGivenOptionToApproveUser()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"A customer with existing Company account associated")]
        public void GivenACustomerWithExistingCompanyAccountAssociated()
        {
            ScenarioContext.Current.Pending();
        }

    }
}
