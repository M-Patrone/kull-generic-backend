#if NET48
using HttpContext = System.Web.HttpContextBase;
#else
using Microsoft.AspNetCore.Http;
#endif
using Microsoft.OpenApi;

namespace Kull.GenericBackend.Parameters;

public class FileDescriptionParameter : WebApiParameter
{
    public override bool RequiresFormData => true;

    // User cannot provide value anyway, SqlName is always null
    public override bool RequiresUserProvidedValue => false;

    public FileDescriptionParameter(string webApiNamewebApiName):base(null,webApiNamewebApiName)
    {
    }

    public override OpenApiSchema GetSchema()
    {
        return new OpenApiSchema()
        {
            //https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.0.md#considerations-for-file-uploads
            Type = JsonSchemaType.String,
            Format = "binary"
        };
    }

    public override object? GetValue(HttpContext? http, object? valueProvided, ApiParameterContext? parameterContext)
    {
        return null;
    }
}
