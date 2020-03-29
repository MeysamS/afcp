using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Helpers
{
    public class DateTimeBinder:IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            // ToDo: check if shamsi then do the conversion.
            var date = (DateTime)value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
            date = PersianCulture.PersianToGregorianUS(date);

            return date;
        }
        public class NullableDateTimeBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if ((value != null)&& (!string.IsNullOrWhiteSpace(value.AttemptedValue.Trim())))
                {
                    var date = (DateTime)value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
                    date = PersianCulture.PersianToGregorianUS(date);

                    return date;
                }
                return null;
            }
        }
    }
}