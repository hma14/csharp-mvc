using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Omnae.Extensions
{
    public class MvcBootstrapModalOptions
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool UseSubmitAction { get; set; }
        public bool UseCancelAction { get; set; }
        public string SubmitActionText { get; set; }
        public string CancelActionText { get; set; }

        public MvcBootstrapModalOptions()
        {
            UseSubmitAction = true;
            UseCancelAction = true;
            SubmitActionText = "Submit";
            CancelActionText = "Cancel";
        }
    }

    public class MvcBootstrapModal : IDisposable
    {
        private readonly HtmlHelper _htmlHelper;
        private readonly MvcBootstrapModalOptions _options;

        public MvcBootstrapModal(
            HtmlHelper htmlHelper,
            MvcBootstrapModalOptions options
            )
        {
            _htmlHelper = htmlHelper;
            _options = options;
            GenerateBefore();
        }

        public void Dispose()
        {
            GenerateAfter();
        }

        public void GenerateBefore()
        {
            _htmlHelper.RenderPartial("Popup/_BootstrapModal",
                new ViewDataDictionary
                {
                    {"RenderingBegin", true},
                    {"RenderingEnd", false},
                    {"RenderingHeader", true},
                    {"RenderingFooter", false},
                    {"Options", _options},
                });
        }

        public void GenerateAfter()
        {
            _htmlHelper.RenderPartial("Popup/_BootstrapModal",
                new ViewDataDictionary
                {
                    {"RenderingBegin", false},
                    {"RenderingEnd", true},
                    {"RenderingHeader", false},
                    {"RenderingFooter", true},
                    {"Options", _options},
                });
        }
    }

    public static class HtmlHelperExtensionsForBootstrapModal
    {
        public static MvcBootstrapModal BeginBootstrapModal(this HtmlHelper htmlHelper, MvcBootstrapModalOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Id)) throw new ArgumentNullException("Id");
            if (string.IsNullOrWhiteSpace(options.Title)) throw new ArgumentNullException("Title");

            return new MvcBootstrapModal(htmlHelper, options);
        }
    }
}