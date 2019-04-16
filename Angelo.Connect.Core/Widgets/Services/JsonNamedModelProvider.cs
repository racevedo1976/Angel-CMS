using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Angelo.Connect.Widgets.Services
{
    public class JsonNamedModelProvider : IWidgetNamedModelProvider
    {
        public IWidgetModel GetModel(string widgetType, string modelName, Type modelType)
        {
            var assembly = Assembly.GetEntryAssembly();
            var jsonPath = FindJsonResource(assembly, widgetType);

            try
            {
                var stream = assembly.GetManifestResourceStream(jsonPath);
                string contents = "";

                using (var reader = new StreamReader(stream))
                {
                    contents = reader.ReadToEnd();
                }
                stream.Dispose();

                Type collectionType = typeof(List<>).MakeGenericType(modelType);

                var models = JsonConvert.DeserializeObject
                (
                    contents,
                    collectionType,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                );

                if (models != null)
                {
                    foreach (var entry in models as IList)
                    {
                        var model = entry as IWidgetModel;

                        if (model.Id == modelName)
                            return model;
                    }
                }
            }
            catch (Exception)
            {
                // do nothing
            }

            return null;
        }

        private string FindJsonResource(Assembly assembly, string modelType)
        {
            var resources = assembly.GetManifestResourceNames();
            var expectedName = "." + modelType.ToLower() + ".json";

            foreach (var resourceName in resources)
            {
                if (resourceName.ToLower().EndsWith(expectedName))
                    return resourceName;
            }

            return null;
        }
    }
}
