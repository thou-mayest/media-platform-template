var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Host_WebApi>("host-webapi");

builder.Build().Run();
