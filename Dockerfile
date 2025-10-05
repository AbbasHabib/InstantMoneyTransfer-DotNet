# Use official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies first (for caching)
COPY *.sln .
COPY InstantTransfers/*.csproj ./InstantTransfers/


RUN dotnet nuget locals all --clear
RUN dotnet restore "InstantTransfers/InstantTransfers.csproj"

# Copy the rest of the source code
COPY InstantTransfers/. ./InstantTransfers/

WORKDIR /src/InstantTransfers

# Build and publish the app
RUN dotnet publish -c Release -o /app

# Use the ASP.NET runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app ./

# # Environment variables (optional)
# ENV DOTNET_RUNNING_IN_CONTAINER=true
# ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false

# Run the app
ENTRYPOINT ["dotnet", "InstantTransfers.dll"]
