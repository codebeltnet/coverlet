# Use .NET 11 as the base image
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS base

# Install .NET 10 SDK and shared frameworks side-by-side
COPY --from=mcr.microsoft.com/dotnet/sdk:10.0 /usr/share/dotnet/sdk /usr/share/dotnet/sdk
COPY --from=mcr.microsoft.com/dotnet/sdk:10.0 /usr/share/dotnet/shared /usr/share/dotnet/shared
