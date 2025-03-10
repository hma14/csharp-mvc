using Omnae.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Omnae.Extensions
{
    public class BootstrapDropdownMenuActionLink
    {
        public MvcHtmlString ActionLink { get; set; }
        public MvcHtmlString SubMenuTitle { get; set; }
        public IEnumerable<MvcHtmlString> SubMenuActionLinks { get; set; }

        public BootstrapDropdownMenuActionLink()
        {
            SubMenuActionLinks = Enumerable.Empty<MvcHtmlString>();
        }
    }

    public static class HtmlHelperExtensionsForBootstrap
    {
        /// <summary>
        /// Creates a bootstrap-styled dropdown menu.
        /// </summary>
        public static MvcHtmlString BootstrapDropdownMenu(this HtmlHelper htmlHelper, string title, IEnumerable<MvcHtmlString> actionLinks, string btnClass = "btn-link")
        {
#if true
            var str = new MvcHtmlString(
                "<div class=\"btn-group\">" +
                    "<button class=\"btn small btn-primary " + btnClass + " dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                    title +
                    "</button>" +
                    "<ul class=\"dropdown-menu\" style=\"min-width:0px\">" +
                        string.Join("", actionLinks.Select(a => "<li>" + a + "</li>")) +
                    "</ul>" +
                "</div>");
#else

            var str = new MvcHtmlString(
               "<div class=\"btn-group\">" +
                   @"<a class=""nav-link dropdown-toggle dropdown-header"" id="" " + title.RemoveWhitespace() +@""" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">" + title + "</a>" +
                    @"<div class=""dropdown-menu"" aria-labelledby=""dropdownMenu1"">" +
                   
                       string.Join("", actionLinks.Select(a =>  a )) +
                   
                   "</div>" +
               "</div>");
#endif
            return str;
        }

        /// <summary>
        /// Creates a bootstrap-styled dropdown menu + sub menu.
        /// </summary>
        public static MvcHtmlString BootstrapDropdownMenu(this HtmlHelper htmlHelper, string title, IEnumerable<BootstrapDropdownMenuActionLink> actionLinks, string btnClass = "btn-link")
        {
            var str = new MvcHtmlString(
                "<div class=\"btn-group\">" +
                "<button class=\"btn " + btnClass +
                " dropdown-toggle\" type=\"button\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                title + "&nbsp;<span class=\"caret\"></span>" +
                "</button>" +
                "<ul class=\"dropdown-menu\" style=\"min-width:0px\">" +
                string.Join("", actionLinks.Select(a => a.SubMenuActionLinks.Any()
                    ? string.Format(
                        "<li class='dropdown-submenu'>\n" +
                            "<a data-toggle='dropdown'>{0}</a>\n" +
                            "<ul class='dropdown-menu blue-text'>{1}</ul>" +
                        "</li>",
                        a.SubMenuTitle,
                        string.Join("", a.SubMenuActionLinks.Select(s => "<li>" + s + "</li>\n")))
                    : string.Format("<li>{0}</li>\n", a.ActionLink))) +
                "</ul>\n" +
                "</div>");
            return str;
        }
    }
}