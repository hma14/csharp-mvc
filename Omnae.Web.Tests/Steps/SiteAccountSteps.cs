using System;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Omnae.Web.Tests.Steps
{
    [Binding]
    public class SiteAccountSteps : IDisposable
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        public IWebDriver WebDriver { get; }
        public IWebElement FormElement { get; }
        public IWebElement LoginElement { get; }
        public IWebElement PasswordElement { get; }

        public SiteAccountSteps()
        {
            WebDriver = new FirefoxDriver {Url = "https://dev.omnae.com/Account/Login"};
            WebDriver.Manage().Timeouts().ImplicitWait = DefaultTimeout;
            
            FormElement = WebDriver.FindElement(By.Id("loginform"));
            LoginElement = WebDriver.FindElement(By.Id("Email"));
            PasswordElement = WebDriver.FindElement(By.Id("Password"));
        }

        public void Dispose()
        {
            WebDriver.Close();
            WebDriver?.Dispose();
        }

        [When(@"The user login in the system with the correct password")]
        public void WhenTheUserLoginInTheSystemWithTheCorrectPassword()
        {
            LoginElement.Displayed.Should().BeTrue();
            PasswordElement.Displayed.Should().BeTrue();

            var login = @"hma+10@padtech.com";
            var test = @"Test@12345";

            FillLoginForm(login, test);
        }
        
        [When(@"The user login in the system with the incorrect password")]
        public void WhenTheUserLoginInTheSystemWithTheIncorrectPassword()
        {
            LoginElement.Displayed.Should().BeTrue();
            PasswordElement.Displayed.Should().BeTrue();

            var login = @"hma+10@padtech.com";
            var test = @"12345";

            FillLoginForm(login, test);
        }

        private void FillLoginForm(string login, string test)
        {
            LoginElement.Clear();
            LoginElement.SendKeys(login);
            PasswordElement.Clear();
            PasswordElement.SendKeys(test);

            FormElement.Submit();
        }

        
        [Then(@"the system show login the user and show the dashboard page")]
        public void ThenTheSystemShowLoginTheUserAndShowTheDashboardPage()
        {
            System.Threading.Thread.Sleep(15000);

            var currentUrl = WebDriver.Url;
            currentUrl.Should().Be(@"https://dev.omnae.com/");
        }
        
        [Then(@"the system shoud show a error message")]
        public void ThenTheSystemShoudShowAErroeMessage()
        {
            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementExists(By.ClassName("validation-summary-errors")));

            var erroList = WebDriver.FindElement(By.ClassName("validation-summary-errors"));
            var text = erroList.Text;

            text.Should().NotBeNullOrWhiteSpace();
        }
    }
}
