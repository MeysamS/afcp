

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Annual_faculty_promotions.WebUI.Helpers
{
    public static class CustomHelpers
    {
        public class CustomSelectItem : SelectListItem
        {
            public string Class { get; set; }
            public string Disabled { get; set; }
            public string SelectedValue { get; set; }

            public string Level { get; set; }

            public CustomSelectItem(string _class,string _disabled,string _selectedValue)
            {
                Class = _class;
                Disabled = _disabled;
                SelectedValue = _selectedValue;
            }
        }

        public static MvcHtmlString CustomDropdownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<CustomSelectItem> list, string selectedValue, string optionLabel, object htmlAttributes = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string name = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            return CustomDropdownList(htmlHelper, metadata, name, optionLabel, list, selectedValue, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        private static MvcHtmlString CustomDropdownList(this HtmlHelper htmlHelper, ModelMetadata metadata, string name, string optionLabel, IEnumerable<CustomSelectItem> list, string selectedValue, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }

            TagBuilder dropdown = new TagBuilder("select");
            dropdown.Attributes.Add("name", fullName);
            dropdown.MergeAttribute("data-val", "true");
            dropdown.MergeAttribute("data-val-required", "Mandatory field.");
            dropdown.MergeAttribute("data-val-number", "The field must be a number.");
            dropdown.MergeAttributes(htmlAttributes); //dropdown.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            dropdown.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            StringBuilder options = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
                options = options.Append("<option value='" + String.Empty + "'>" + optionLabel + "</option>");

            foreach (var item in list)
            {
                if (item.SelectedValue == "selected" && item.Disabled == "disabled")
                    options = options.Append("<option value='" + item.Value + "' Level='" + item.Level + "' class='" + item.Class + "' selected='" + item.SelectedValue + "' disabled='" + item.Disabled + "'>" + item.Text + "</option>");
                else if (item.SelectedValue != "selected" && item.Disabled == "disabled")
                    options = options.Append("<option value='" + item.Value + "' Level='" + item.Level + "' class='" + item.Class + "' disabled='" + item.Disabled + "'>" + item.Text + "</option>");
                else if (item.SelectedValue == "selected" && item.Disabled != "disabled")
                    options = options.Append("<option value='" + item.Value + "' Level='" + item.Level + "' class='" + item.Class + "' selected='" + item.SelectedValue + "'>" + item.Text + "</option>");
                else
                    options = options.Append("<option value='" + item.Value + "' Level='" + item.Level + "' class='" + item.Class + "'>" + item.Text + "</option>");
            }
            dropdown.InnerHtml = options.ToString();
            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }
    }
}