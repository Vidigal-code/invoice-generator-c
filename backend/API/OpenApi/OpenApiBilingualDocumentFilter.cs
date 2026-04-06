using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InvoiceGenerator.Api.API.OpenApi;

/// <summary>Applies English vs pt-BR titles, tags, operation text, and localized 401/403 where security applies.</summary>
public sealed class OpenApiBilingualDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (context.DocumentName is not (OpenApiDocumentNames.V1En or OpenApiDocumentNames.V1Br))
            return;

        swaggerDoc.Info = OpenApiTranslationData.GetApiInfo(context.DocumentName);
        swaggerDoc.Tags = OpenApiTranslationData.GetTags(context.DocumentName);

        foreach (var pathItem in swaggerDoc.Paths.Values)
        {
            foreach (var (_, operation) in pathItem.Operations)
            {
                if (!string.IsNullOrEmpty(operation.OperationId)
                    && OpenApiTranslationData.TryGetOperation(context.DocumentName, operation.OperationId, out var summary, out var description))
                {
                    operation.Summary = summary;
                    operation.Description = description;
                }

                OpenApiTranslationData.ApplyAuthResponseDescriptions(operation, context.DocumentName);
            }
        }
    }
}
