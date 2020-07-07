namespace Healthcheck.Service.Utilities
{
    using Healthcheck.Service.Models;
    using Newtonsoft.Json;

    public static class JsonUtil
    {        
        /// <summary>
        /// Gets the error messages list in JSON.
        /// </summary>
        /// <param name="errorList">The error list.</param>
        /// <remarks>Since there are some Exception classes that can't be serialized / deserialized properly we have to apply a hack.</remarks>
        /// <returns>Serialized error messages.</returns>
        public static string GetErrorMessagesJson(ErrorList errorList)
        {
            return JsonConvert.SerializeObject(errorList).Replace("\"SafeSerializationManager\":", "\"_SafeSerializationManager\":");
        }
    }
}