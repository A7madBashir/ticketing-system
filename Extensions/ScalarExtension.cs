namespace TicketingSystem.Extensions;

public static class OpenApiEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapScalarApiReference(
        this IEndpointRouteBuilder endpoints
    )
    {
        return endpoints
            .MapGet(
                "/scalar/",
                () =>
                {
                    var title = $"Scalar API Reference";
                    return Results.Content(
                        $$"""
                        <!doctype html>
                        <html>
                        <head>
                            <title>{{title}}</title>
                            <meta charset="utf-8" />
                            <meta name="viewport" content="width=device-width, initial-scale=1" />
                        </head>
                        <body>
                            <script id="api-reference" data-url="/swagger/v1/swagger.json"></script>
                            <script>
                            var configuration = {
                                theme: 'kepler-11e',
                            }
                            document.getElementById('api-reference').dataset.configuration =
                                JSON.stringify(configuration)
                            </script>
                            <script src="/lib/scalar/api-reference.js"></script>
                        </body>
                        </html>
                        """,
                        "text/html"
                    );
                }
            )
            .ExcludeFromDescription();
    }
}
