using Kull.GenericBackend.SwaggerGeneration;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kull.GenericBackend.Serialization;

public class ResponseDescriptor
{
    private readonly CodeConvention codeConvention;

    public ResponseDescriptor(CodeConvention codeConvention)
    {
        this.codeConvention = codeConvention;
    }


    public virtual OpenApiResponses GetDefaultResponse(
            OperationResponseContext context,
            bool firstOnly,
            bool wrapResult)
    {

        IOpenApiSchema schema;
        var outputObjectName = context.OutputObjectTypeName;
        var resultTypeName = context.ResultTypeName;
        if (wrapResult)
        {
            schema = GetWrappedSchema(resultTypeName, outputObjectName, firstOnly);
        }
        else
        {
            schema = firstOnly ? GetResultReference(resultTypeName) : GetArrayOfResult(resultTypeName);
        }

        OpenApiResponses responses = new OpenApiResponses();
        OpenApiResponse response = new OpenApiResponse();
        response.Description = $"OK"; // Required as per spec
        response.Content.Add("application/json", new OpenApiMediaType()
        {
            Schema = schema

        });
        responses.Add("200", response);
        return responses;
    }

    [Obsolete]

    public virtual OpenApiResponses GetDefaultResponse(
            OperationResponseContext context,
            bool firstOnly = false)
    {

        return GetDefaultResponse(context, firstOnly, context.OutputObjectTypeName != null || context.AlwaysWrapJson);
    }
    public virtual OpenApiSchema GetAdditionalItemsSchema() => new OpenApiSchema()
    {
        Type = JsonSchemaType.Array,
        Items = new OpenApiSchema()
        {
            Type = JsonSchemaType.Array,
            Items = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                AdditionalPropertiesAllowed = true
            }
        }
    };

    public virtual IOpenApiSchema GetWrappedSchema(
      IOpenApiSchema firstResultSchema,
      string? outputObjectName)
    {

        OpenApiSchema schema = new OpenApiSchema()
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string>(new string[] { codeConvention.FirstResultKey }),
            Properties = new Dictionary<string, IOpenApiSchema>()
                   {
                       {  codeConvention.FirstResultKey, firstResultSchema},
                        { codeConvention.OtherResultsKey,  GetAdditionalItemsSchema() }
                   }
        };
        if (outputObjectName != null)
        {
            schema.Properties.Add(codeConvention.OutputParametersKey, new OpenApiSchemaReference(outputObjectName));
            schema.Required.Add(codeConvention.OutputParametersKey);
        }

        return schema;
    }

    public virtual IOpenApiSchema GetWrappedSchema(
        string resultTypeName,
        string? outputObjectName,
            bool firstItemOnly)
    {
        return GetWrappedSchema(firstItemOnly ? GetResultReference(resultTypeName) : GetArrayOfResult(resultTypeName),
            outputObjectName);
    }

    public virtual IOpenApiSchema GetResultReference(string resultTypeName) => new OpenApiSchemaReference(resultTypeName);

    public virtual IOpenApiSchema GetArrayOfResult(string resultTypeName)
    {
        return new OpenApiSchema()
        {
            Type = JsonSchemaType.Array,
            Xml = new OpenApiXml()
            {
                Name = "table"
            },
            UniqueItems = false,
            Items = GetResultReference(resultTypeName)
        };
    }
}
