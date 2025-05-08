using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class NewtonsoftModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) return null;

            if (context.Metadata.ModelType == typeof(object) || context.Metadata.ModelType.IsClass)
            {
                return new BinderTypeModelBinder(typeof(NewtonsoftModelBinder));
            }
            return null;
        }
    }

    public class NewtonsoftModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) return Task.CompletedTask;

            var json = bindingContext.HttpContext.Request.Body;
            using (var reader = new StreamReader(json))
            {
                var jsonString = reader.ReadToEnd();
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Converters = new List<JsonConverter> { new VietnamDateTimeConverter() },
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                };
                var model = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, settings);
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            return Task.CompletedTask;
        }
    }
}
