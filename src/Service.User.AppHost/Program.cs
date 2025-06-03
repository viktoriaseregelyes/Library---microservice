var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddContainer("postgres", "postgres:latest")
    .WithEnvironment("POSTGRES_DB", "postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "admin")
    .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
    .WithEndpoint(name: "postgres", targetPort: 5432);

var mongodb = builder.AddContainer("mongodb", "mongo:6.0")
    .WithEndpoint(name: "mongodb", targetPort: 27017);

var messaging = builder.AddRabbitMQ("library-messaging")
    .WithManagementPlugin();

var book = builder.AddProject<Projects.Service_Book>("library-book")
    .WithReference(messaging);

var user = builder.AddProject<Projects.Service_User>("library-user")
    .WithReference(book)
    .WithReference(messaging);

builder.Build().Run();