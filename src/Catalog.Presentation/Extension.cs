using Catalog.Presentation.Catalog;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation;

public static class Extension
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCatalogApi();
        return app;
    }
}
