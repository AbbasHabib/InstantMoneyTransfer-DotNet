# Use official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY InstantTransfers/*.csproj ./InstantTransfers/

RUN dotnet restore

# Copy the rest of the source code
COPY InstantTransfers/. ./InstantTransfers/

WORKDIR /src/InstantTransfers

# Build and publish the app
RUN dotnet publish -c Release -o /app --no-restore

# Use the ASP.NET runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app ./

# Expose port 80
EXPOSE 80


# Run the app
ENTRYPOINT ["dotnet", "InstantTransfers.dll"]
