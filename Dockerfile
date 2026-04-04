FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY backend/InvoiceGenerator.Api.csproj backend/
RUN dotnet restore backend/InvoiceGenerator.Api.csproj
COPY backend/ backend/
RUN dotnet publish backend/InvoiceGenerator.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "InvoiceGenerator.Api.dll"]
