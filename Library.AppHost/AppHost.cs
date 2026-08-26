var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").AddDatabase("libraryDb");

builder.AddProject<Projects.Library_Api>("library-api")
    .WithReference(postgres);

builder.Build().Run();