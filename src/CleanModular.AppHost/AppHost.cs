var builder = DistributedApplication.CreateBuilder(args);



// start your own container with: docker run -d --name postgres -e POSTGRES_PASSWORD=pg_db_password -p 5432:5432 -v ./postgres-data:/var/lib/postgresql/data postgres
var password = builder.AddParameter("password", secret: true);
var jwtSigningKey = builder.AddParameter("jwt-signing-key", secret: true);
var bootstrapAdminPassword = builder.AddParameter("bootstrap-admin-password", secret: true);

var postgres = builder.AddPostgres("postgres", password: password)
    .WithPgAdmin()
    .WithDataBindMount("./postgres-data")
    .WithHostPort(5432);

var database = postgres.AddDatabase("MainDb");

var api = builder.AddProject<Projects.Host_WebApi>("host-webapi")
    .WithReference(database)
    .WithEnvironment("Jwt__SigningKey", jwtSigningKey)
    .WithEnvironment("BootstrapAdmin__Password", bootstrapAdminPassword)
    .WithEnvironment("Cors__AllowedOrigins__0", "http://localhost:4321")
    .WithHttpEndpoint(port: 5099, targetPort: 8080, name: "public")
    .WithExternalHttpEndpoints()
    .WaitFor(database);

builder.AddNpmApp("frontend", "../../AstroFrontend", "start")
    .WithHttpEndpoint(port: 4321, targetPort: 4321, name: "http")
    .WithExternalHttpEndpoints()
    .WithEnvironment("PUBLIC_API_URL", api.GetEndpoint("public"))
    .WaitFor(api)
    .WithUrlForEndpoint("http", url => url.DisplayText = "Verso frontend");

builder.Build().Run();
