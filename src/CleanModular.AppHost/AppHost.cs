using Google.Protobuf.WellKnownTypes;

var builder = DistributedApplication.CreateBuilder(args);



var frontend = builder.AddNpmApp("frontend", "../../AstroFrontend", "start")
    .WithExternalHttpEndpoints()
    .WithUrl("http://localhost:4321");

// start your own container with: docker run -d --name postgres -e POSTGRES_PASSWORD=pg_db_password -p 5432:5432 -v ./postgres-data:/var/lib/postgresql/data postgres
var password = builder.AddParameter("password", secret: true);

var postgres = builder.AddPostgres("postgres", password: password)
    .WithPgAdmin()
    .WithDataBindMount("./postgres-data")
    .WithHostPort(5432);

var database = postgres.AddDatabase("MainDb");

builder.AddProject<Projects.Host_WebApi>("host-webapi").WithReference(postgres)
    .WithUrl("scalar")
    .WaitFor(postgres);

builder.Build().Run();