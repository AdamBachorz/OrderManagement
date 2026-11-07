FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderManagement.AppHost/OrderManagement.AppHost.csproj", "OrderManagement.AppHost/"]
COPY ["OrderManagement.Api/OrderManagement.Api.csproj", "OrderManagement.Api/"]
COPY ["OrderManagement.Application/OrderManagement.Application.csproj", "OrderManagement.Application/"]
COPY ["OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj", "OrderManagement.Infrastructure/"]
COPY ["OrderManagement.Domain/OrderManagement.Domain.csproj", "OrderManagement.Domain/"]
COPY ["OrderManagement.ServiceDefaults/OrderManagement.ServiceDefaults.csproj", "OrderManagement.ServiceDefaults/"]
ENV DOTNET_SERVER_GC=0
ENV DOTNET_GCHeapHardLimit=0x20000000
RUN dotnet restore "./OrderManagement.AppHost/OrderManagement.AppHost.csproj"

COPY . .
WORKDIR "/src/OrderManagement.AppHost"
RUN dotnet publish "./OrderManagement.AppHost.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:18888
EXPOSE 18888
COPY --from=build /publish /app
ENTRYPOINT ["dotnet", "OrderManagement.AppHost.dll"]
