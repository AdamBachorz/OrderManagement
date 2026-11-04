var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OrderManagement_Api>("ordermanagement-api");

builder.Build().Run();
