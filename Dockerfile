# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY Authly/Authly.csproj ./Authly/
RUN dotnet restore ./Authly/Authly.csproj

# Copy everything else and build
COPY Authly/ ./Authly/
WORKDIR /source/Authly
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Expose port 5180
EXPOSE 5180
ENV ASPNETCORE_URLS=http://+:5180
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Authly.dll"]
