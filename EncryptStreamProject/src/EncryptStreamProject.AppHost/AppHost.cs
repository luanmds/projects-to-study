var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApi>("api");

builder.AddProject<Projects.Validator>("validator");

builder.AddProject<Projects.Encryptor>("encryptor");

builder.Build().Run();
