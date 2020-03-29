using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

public static class ImageHelper
{
    public static string ImageLink(this System.Web.Mvc.HtmlHelper htmlHelper, string imgSrc, string alt, string actionName, string controllerName, object routeValues, object htmlAttributes, object imgHtmlAttributes)
    {
        UrlHelper urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
        TagBuilder imgTag = new TagBuilder("img");
        imgTag.MergeAttribute("src", imgSrc);
        imgTag.MergeAttributes((IDictionary<string, string>)imgHtmlAttributes, true);
        string url = urlHelper.Action(actionName, controllerName, routeValues);

        TagBuilder imglink = new TagBuilder("a");
        imglink.MergeAttribute("href", url);
        imglink.InnerHtml = imgTag.ToString();
        imglink.MergeAttributes((IDictionary<string, string>)htmlAttributes, true);

        return imglink.ToString();

    }
    public static string Image(this HtmlHelper helper, string src, string alt = null)
    {
        TagBuilder tb = new TagBuilder("img");
        tb.Attributes.Add("src", helper.Encode(src));
        if (alt != null)
            tb.Attributes.Add("alt", helper.Encode(alt));
        return tb.ToString(TagRenderMode.SelfClosing);
    }
    public static string Image(this HtmlHelper helper, string src, object htmlAttributes = null)
    {
        TagBuilder tb = new TagBuilder("img");
        tb.Attributes.Add("src", helper.Encode(src));
        if (htmlAttributes != null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            tb.MergeAttributes(attributes);
        }
        return tb.ToString(TagRenderMode.SelfClosing);
    }
    public static MvcHtmlString ImageFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes, string noImage, params string[] folders)
    {
        var data = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
        string path = "~/";
        foreach (var folderName in folders)
        {
            path += folderName + "/";
        }
        path += (data.Model == null ? noImage : data.Model.ToString());

        TagBuilder img = new TagBuilder("img");
        img.Attributes.Add("src", UrlHelper.GenerateContentUrl(path, helper.ViewContext.HttpContext));

        if (htmlAttributes != null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            img.MergeAttributes(attributes);
        }

        return new MvcHtmlString(img.ToString());
    }
}