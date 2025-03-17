FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["ServiceWorker/ServiceWorker.csproj", "./"]
RUN dotnet restore

# Copy all files and build
COPY . .
RUN dotnet build "ServiceWorker/ServiceWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceWorker/ServiceWorker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for CSV files
RUN mkdir -p /app/data

VOLUME /app/data

ENTRYPOINT ["dotnet", "ServiceWorker.dll"]
