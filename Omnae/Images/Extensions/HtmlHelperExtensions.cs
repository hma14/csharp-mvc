using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using InvestX.Abstractions;
using InvestX.Common;
using InvestX.Common.Extensions;
using InvestX.Platform.Abstractions.Managers;
using InvestX.ViewModels.Common;
using System.Web;

namespace InvestX.UI.Base.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GetControl<TModel, TType>(this HtmlHelper<TModel> htmlHelper, TModel model,
            Expression<Func<TModel, TType>> expression, int tabIndex, ControlType controlType = ControlType.Text,
            string cssClass = "textbox", string style = "", string groupName = "", object attributes = null,
            IEnumerable<SelectListItem> selectList = null)
            where TModel : IMultiModeViewModel
        {
            switch (model.FormMode)
            {
                case FormMode.Edit:
                    switch (controlType)
                    {
                        case ControlType.Text:
                            return GetEditControl(htmlHelper, expression, attributes ?? new
                            {
                                @tabindex = tabIndex,
                                style = style,
                                @type = controlType.ToString().ToLower(),
                                @class = cssClass
                            });
                        case ControlType.Checkbox:
                            return GetCheckboxControl(htmlHelper, expression, attributes ?? new
                            {
                                @tabindex = tabIndex,
                                style = style,
                                @class = cssClass,
                            });
                        case ControlType.RadioButton:
                            return GetRadioButtonControl(htmlHelper, expression, attributes ?? new
                            {
                                @tabindex = tabIndex,
                                style = style,
                                @class = cssClass
                            }, default(TType));
                        case ControlType.TextArea:
                            return GetBigEditControl(htmlHelper, expression, attributes ?? new
                            {
                                @tabindex = tabIndex,
                                style = style,
                                @class = cssClass
                            });
                        case ControlType.DropDownList:
                            if (selectList == null) throw new ArgumentNullException("selectList");
                            return htmlHelper.DropDownListFor(expression, selectList, attributes);
                        default:
                            return GetDisplayControl(htmlHelper, controlType, expression, cssClass);
                    }
                default:
                    return GetDisplayControl(htmlHelper, controlType, expression, cssClass);
            }
        }

        public static MvcHtmlString GetFormTextBoxFullWidth<TModel>(this HtmlHelper<TModel> htmlHelper,
            TModel model, SignableDocumentFormEditableText controlId, int tabIndex, int formTextBoxIndex, int rows, string defaultValue)
            where TModel : IMultiModeViewModel
        {
            return GetFormTextBox(htmlHelper, model, controlId, tabIndex, formTextBoxIndex, 120, rows, defaultValue);
        }


        public static MvcHtmlString GetFormTextBox<TModel>(this HtmlHelper<TModel> htmlHelper,
            TModel model, SignableDocumentFormEditableText controlId, int tabIndex, int formTextBoxIndex, int cols, int rows,string defaultValue)
            where TModel : IMultiModeViewModel
        {
            var index = model.GetIndexOfFormTextBoxViewModel(controlId,defaultValue);
            if (index == -1)
            {
                return new MvcHtmlString(defaultValue);
            }
            defaultValue = model.SetDefaultValueOfFormTextBoxViewModel(index, defaultValue, formTextBoxIndex);
            if (model.FormMode == FormMode.View)
            {
                index = model.GetIndexOfFormTextBoxViewModel(controlId, defaultValue);
                return new MvcHtmlString(model.GetTextForFormTextBoxViewModel(index));
            }
            if (!model.AllowTextEdit)
            {
                index = model.GetIndexOfFormTextBoxViewModel(controlId, defaultValue);
                return new MvcHtmlString(model.GetTextForFormTextBoxViewModel(index));
            }
            index = model.GetIndexOfFormTextBoxViewModel(controlId, defaultValue);
            return htmlHelper.Partial("/Views/Shared/SubscriptionAgreement/EditorTemplates/_FormTextBox.cshtml",
                model, new ViewDataDictionary() {{"index", index}, {"cols", cols}, {"rows", rows}});

        }

        private static MvcHtmlString GetCheckboxControl<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object attributes)
        {
            var typedExpression = (Expression<Func<TModel, bool>>)(object)expression;
            return htmlHelper.CheckBoxFor(typedExpression, attributes);
        }

        private static MvcHtmlString GetRadioButtonControl<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object attributes, TProperty value)
        {
            return htmlHelper.RadioButtonFor(expression, value, attributes);
        }

        private static MvcHtmlString GetEditControl<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object attributes)
        {
            return htmlHelper.TextBoxFor(expression, attributes);
        }

        private static MvcHtmlString GetBigEditControl<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object attributes)
        {
            return htmlHelper.TextAreaFor(expression, attributes);
        }

        private static MvcHtmlString GetDisplayControl<TModel, TType>(HtmlHelper<TModel> htmlHelper, ControlType controlType, Expression<Func<TModel, TType>> expression, string cssClass)
            where TModel : IMultiModeViewModel
        {
            switch (controlType)
            {
                case ControlType.DropDownList:
                {
                    var tmpValue = expression.Compile()(htmlHelper.ViewData.Model);
                    if (tmpValue != null)
                    {
                        var result = (string)Convert.ChangeType(tmpValue, (typeof(string)));
                        return new MvcHtmlString("<span>" + result + "</span>");
                    }
                    return new MvcHtmlString("<span></span>");
                }
                case ControlType.Checkbox:
                {
                    var tmpValue =
                        (bool) Convert.ChangeType(expression.Compile()(htmlHelper.ViewData.Model), (typeof (bool)));
                    string checkboxValue;
                    if (!tmpValue)
                    {
                        checkboxValue = string.Empty;
                        cssClass = "checkbox-unchecked";
                    }
                    else
                    {
                        checkboxValue = "X";
                        cssClass = "checkbox-checked";
                    }
                    return new MvcHtmlString("<span class=\"" + cssClass + "\">" + checkboxValue + "</span>");
                }
                default:
                {
                    var tmpValue = (string)Convert.ChangeType(expression.Compile()(htmlHelper.ViewData.Model), (typeof(string)));
                    if (string.IsNullOrEmpty(tmpValue))
                    {
                        return new MvcHtmlString("<span class=\""+cssClass+"\">&nbsp;&nbsp;&nbsp;</span>");
                    }
                    return new MvcHtmlString("<span class=\"" + cssClass + "\">" + tmpValue +"</span>");
                }
            }
        }

        private static MvcHtmlString GetDisplayControlForRadioButton<TModel, TType>(HtmlHelper<TModel> htmlHelper,
            ControlType controlType, Expression<Func<TModel, TType>> expression, object attributes, TType value)
            where TModel : IMultiModeViewModel
        {
            var tagBase = "<span class=\"{0}\">{1}</span>";
            var evaluatedValue = expression.Compile()(htmlHelper.ViewData.Model);

            if (evaluatedValue != null)
            {
                var tmpValue = evaluatedValue.ToString().ConvertToType<TType>();
                if (string.Compare(value.ToString(), tmpValue.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return new MvcHtmlString(string.Format(tagBase, "checkbox-checked", "X"));
                }
            }

            return new MvcHtmlString(string.Format(tagBase, "checkbox-unchecked", ""));
        }

        public static MvcHtmlString OptionalRadioButton<TModel>(
            this HtmlHelper<TModel> htmlHelper, TModel model, Expression<Func<TModel, bool?>> expression, bool? value, object attributes)
            where TModel : IMultiModeViewModel
        {
            switch (model.FormMode)
            {
                case FormMode.Edit:
                    return GetRadioButtonControl(htmlHelper, expression, attributes, value ?? false);
                default:
                    return GetDisplayControlForRadioButton(htmlHelper, ControlType.RadioButton, expression,
                        attributes, value);
            }
        }

        public static MvcHtmlString GetRadioButtonControlWithValidation<TModel, TType>(
            this HtmlHelper<TModel> htmlHelper, TModel model, Expression<Func<TModel, TType>> expression, int tabIndex,
            TType value, string cssClass = "radiobutton", string style = "", bool required = true,
            string errorMessage = null,
            string errorContainer = "error-container", string errorHandler = "error-container-message")
            where TModel : IMultiModeViewModel
        {
            switch (model.FormMode)
            {
                case FormMode.Edit:
                    return GetRadioButtonControl(htmlHelper, expression, new
                    {
                        @tabindex = tabIndex,
                        style = style,
                        @class = cssClass,
                        @data_parsley_required = required.ToString().ToLower(),
                        @data_parsley_error_message = errorMessage ?? "&nbsp;Required",
                        @data_parsley_errors_container = errorContainer
                    }, value);
                default:
                    return GetDisplayControlForRadioButton(htmlHelper, ControlType.RadioButton, expression,
                        new {@class = cssClass, style = style}, value);
            }
        }

        public static MvcHtmlString GetControlWithValidation<TModel, TType>(this HtmlHelper<TModel> htmlHelper,
            TModel model, Expression<Func<TModel, TType>> expression, int tabIndex, string style = "",
            string errorMessage = null, bool required = true, ControlType controlType = ControlType.Text,
            string cssClass = "textbox", string errorContainer = "error-container",
            string errorHandler = "error-container-message")
            where TModel : IMultiModeViewModel
        {
            return GetControl(htmlHelper, model, expression, tabIndex, controlType, cssClass, style, string.Empty, new
            {
                @tabindex = tabIndex,
                style = style,
                @type = controlType.ToString().ToLower(),
                @class = cssClass,
                @data_parsley_required = required.ToString().ToLower(),
                @data_parsley_error_message = errorMessage ?? "&nbsp;Required",
                @data_parsley_errors_container = errorContainer
            });
        }

        public static MvcHtmlString RequiredDropdownListFor<TModel, TType>(this HtmlHelper<TModel> htmlHelper,
            TModel model, Expression<Func<TModel, TType>> expression, IEnumerable<SelectListItem> selectList, 
            int tabIndex, string style = "", string cssClass = "", string errorContainer = "error-container")
            where TModel : IMultiModeViewModel
        {
            return GetControl(htmlHelper, model, expression, tabIndex, ControlType.DropDownList, cssClass, style, string.Empty, new
            {
                tabindex = tabIndex,
                style,
                type = ControlType.DropDownList.ToString().ToLower(),
                @class = cssClass,
                data_parsley_required = (true).ToString().ToLower(),
                data_parsley_error_message = "&nbsp;Required",
                data_parsley_errors_container = errorContainer
            }, selectList);
        }

        public static MvcHtmlString OptionalDropdownListFor<TModel, TType>(this HtmlHelper<TModel> htmlHelper,
            TModel model, Expression<Func<TModel, TType>> expression, IEnumerable<SelectListItem> selectList,
            int tabIndex, string style = "", string cssClass = "")
            where TModel : IMultiModeViewModel
        {
            return GetControl(htmlHelper, model, expression, tabIndex, ControlType.DropDownList, cssClass, style, string.Empty, new
            {
                tabindex = tabIndex,
                style,
                type = ControlType.DropDownList.ToString().ToLower(),
                @class = cssClass,
            }, selectList);
        }

        public static MvcHtmlString ConditionalTextBoxFor<TModel, TType, TCType>(this HtmlHelper<TModel> htmlHelper,
            TModel model, Expression<Func<TModel, TType>> expression, int tabIndex, ParsleyConditionType parsleyConditionType,
            Expression<Func<TModel, TCType>> conditionalExpression, string conditionalValue = null, string cssClass = "", string style = "")
            where TModel : IMultiModeViewModel
        {
            if (parsleyConditionType == ParsleyConditionType.IsEqualTo && string.IsNullOrWhiteSpace(conditionalValue))
                throw new ArgumentNullException("conditionalValue", "conditionalValue cannot be null when HasSpecificValue is passed");

            var condition = GetParslyeyCondition(parsleyConditionType, conditionalExpression, conditionalValue);
            return htmlHelper.ConditionalControlFor(model, expression, tabIndex, ControlType.Text, condition, cssClass, style);
        }

        public static MvcHtmlString ConditionalTextAreaFor<TModel, TType, TCType>(this HtmlHelper<TModel> htmlHelper,
            TModel model, Expression<Func<TModel, TType>> expression, int tabIndex, ParsleyConditionType parsleyConditionType,
            Expression<Func<TModel, TCType>> conditionalExpression, string conditionalValue = null, string cssClass = "", string style = "")
            where TModel : IMultiModeViewModel
        {
            if (parsleyConditionType == ParsleyConditionType.IsEqualTo && string.IsNullOrWhiteSpace(conditionalValue))
                throw new ArgumentNullException("conditionalValue", "conditionalValue cannot be null when HasSpecificValue is passed");

            var condition = GetParslyeyCondition(parsleyConditionType, conditionalExpression, conditionalValue);
            return htmlHelper.ConditionalControlFor(model, expression, tabIndex, ControlType.TextArea, condition, cssClass, style);
        }

        public static MvcHtmlString ConditionalControlFor<TModel, TType>(this HtmlHelper<TModel> htmlHelper,
            TModel model, Expression<Func<TModel, TType>> expression, int tabIndex, ControlType controlType,
            string parsleyConditon, string cssClass = "", string style = "")
            where TModel : IMultiModeViewModel
        {
            return GetControl(htmlHelper, model, expression, tabIndex, controlType, string.Empty, string.Empty, string.Empty,
                new
                {
                    @tabindex = tabIndex,
                    @type = controlType.ToString().ToLower(),
                    @class = cssClass,
                    style,
                    data_parsley_conditionalrequired = parsleyConditon,
                    data_parsley_conditionalrequired_message = "&nbsp;Required",
                    data_parsley_validate_if_empty = ""
                });
        }

        private static string GetParslyeyCondition<TModel, TCType>(ParsleyConditionType parsleyConditionType,
            Expression<Func<TModel, TCType>> expression, string value)
        {
            switch (parsleyConditionType)
            {
                case ParsleyConditionType.IsNotEmpty:
                    return string.Format("[\"{0}\", \"[name=\\\"{1}\\\"]\", \"{2}\"]",
                        parsleyConditionType, PropertyHelper.GetPropertyName(expression), "");
                case ParsleyConditionType.IsEqualTo:
                    return string.Format("[\"{0}\", \"[name=\\\"{1}\\\"]:checked\", \"{2}\"]",
                        parsleyConditionType, PropertyHelper.GetPropertyName(expression), value);
                default:
                    throw new NotImplementedException("parsleyConditionType");
            }
        }

        public static MvcHtmlString ReplaceAssetUrnWithPath(this HtmlHelper helper, string originalString, string urn)
        {
            return
                new MvcHtmlString(originalString.Replace(urn,
                    ApplicationSettings.Instance.GetProperty<string>("s3Site") +
                    ApplicationSettings.Instance.GetProperty<string>("s3AwsBucket") +
                    ApplicationSettings.Instance.GetProperty<string>("s3AwsAssetFolder")));
        }

        public static string GetAsset(this HtmlHelper helper, string path, string fileName)
        {
            return ApplicationSettings.Instance.GetProperty<string>("s3Site") + 
                   ApplicationSettings.Instance.GetProperty<string>("s3AwsBucket") + 
                   ApplicationSettings.Instance.GetProperty<string>("s3AwsAssetFolder") + path + fileName;
        }

        public static string GetRootAssetPath(this HtmlHelper helper, string urn)
        {
            return ApplicationSettings.Instance.GetProperty<string>("s3Site") +
                   ApplicationSettings.Instance.GetProperty<string>("s3AwsBucket") +
                   ApplicationSettings.Instance.GetProperty<string>("s3AwsAssetFolder");
        }
    }
}