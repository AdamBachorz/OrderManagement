FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderManagement.AppHost/OrderManagement.AppHost.csproj", "OrderManagement.AppHost/"]
COPY ["OrderManagement.Api/OrderManagement.Api.csproj", "OrderManagement.Api/"]
RUN dotnet restore "./OrderManagement.AppHost/OrderManagement.AppHost.csproj"

COPY . .
WORKDIR "/src/OrderManagement.AppHost"
RUN dotnet publish "./OrderManagement.AppHost.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:18888
EXPOSE 18888
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.AppHost.dll"]
