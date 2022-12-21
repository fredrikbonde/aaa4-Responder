using System.Diagnostics.CodeAnalysis;
using NJsonSchema;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Security;


namespace aaa3.basic.WebApi.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class OpenApiDocumentGeneratorSettingsExtensions
    {
        public static void AddTripId(this AspNetCoreOpenApiDocumentGeneratorSettings configure)
        {
            configure.OperationProcessors.Add(new OperationProcessor(context =>
            {
                context.OperationDescription.Operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "TripId",
                        Kind = OpenApiParameterKind.Header,
                        IsRequired = true,
                        Type = JsonObjectType.String,
                        Default = Guid.NewGuid().ToString()
                    });

                return true;
            }));
        }

        public static void AddXapiKey(this AspNetCoreOpenApiDocumentGeneratorSettings configure)
        {
            configure.OperationProcessors.Add(new OperationSecurityScopeProcessor("X-Api-Key"));
            configure.DocumentProcessors.Add(new SecurityDefinitionAppender("X-Api-Key", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "X-Api-Key",
                In = OpenApiSecurityApiKeyLocation.Header
            }));
        }

        public static void AddBearerToken(this AspNetCoreOpenApiDocumentGeneratorSettings configure)
        {
            configure.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
            configure.DocumentProcessors.Add(new SecurityDefinitionAppender("Bearer", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header
            }));
        }
    }
}
