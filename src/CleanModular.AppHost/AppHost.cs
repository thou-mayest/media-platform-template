using Google.Protobuf.WellKnownTypes;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Host_WebApi>("host-webapi");

var frontend = builder.AddNpmApp("frontend", "../../AstroFrontend", "start")
    .WithExternalHttpEndpoints()
    .WithUrl("http://localhost:4321");

builder.Build().Run();