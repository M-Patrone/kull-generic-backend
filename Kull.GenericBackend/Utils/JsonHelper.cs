using Kull.GenericBackend.Middleware;
using Microsoft.OpenApi;

#if NEWTONSOFTJSON
using Newtonsoft.Json.Serialization;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace Kull.GenericBackend.Utils;

internal class JsonHelper
{
    public static string SerializeObject(object obj, SPMiddlewareOptions? settings = null)
    {
#if NEWTONSOFTJSON
        if (settings == null)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = settings.NamingStrategy
        };
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings()
        {
            ContractResolver = contractResolver
        });
#else
        return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions()
        {
            PropertyNamingPolicy = settings == null ? System.Text.Json.JsonNamingPolicy.CamelCase: settings?.NamingStrategy
        });
#endif
    }
    public static object DeserializeObject(string json)
    {
#if NEWTONSOFTJSON
        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(json);
#else
        var obj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);
#endif
        return Config.DictionaryHelper.ConvertToDeepIDictionary(obj, StringComparer.InvariantCultureIgnoreCase)!;
    }

    ///Mapping for this: https://github.com/Kull-AG/kull-databasemetadata/blob/master/src/Kull.DatabaseMetadata/SqlType.cs#L131
    ///to this: https://github.com/microsoft/OpenAPI.NET/blob/main/src/Microsoft.OpenApi/Models/JsonSchemaType.cs#L12
    public static JsonSchemaType ConvertJsType2JsonSchemaType(string jsType)
    {

        if (string.IsNullOrWhiteSpace(jsType))
            jsType = "null";

        switch (jsType.ToLower().Trim())
        {
            case "null":
                {
                    return JsonSchemaType.Null;
                }
            case "table type":
                {
                    return JsonSchemaType.Array;
                }
            case var s when s.Contains("text"):
            case var s1 when s1.Contains("date") || s1.Contains("time"):
            case "varchar" or "nvarchar" or "sysname" or "nchar" or "char" or "uniqueidentifier":
                {
                    return JsonSchemaType.String;
                }
            case var s2 when s2.Contains("int"):
                {
                    return JsonSchemaType.Integer;
                }
            case "float" or "real" or "double" or "numeric" or "money" or "smallmoney" or "decimal":
                {
                    return JsonSchemaType.Number;
                }
            case "bit":
                {
                    return JsonSchemaType.Boolean;
                }
            case var s3 when s3.Contains("binary"):
            case var s4 when s4.Contains("blob"):
            case "image" or "timestamp" or "rowversion":
                {
                    return JsonSchemaType.String;
                }
            case "xml" or "geography" or "hierarchyid" or "geometry" or "sql_variant":
                {
                    return JsonSchemaType.Object;
                }
            default:
                {
                    throw new ArgumentException($"The type could not be mapped {jsType.ToLower().Trim()}");
                }
        }
    }
}
