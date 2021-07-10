
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

#
# copy csproj and restore as distinct layers
COPY *.sln .
COPY gardenit-api-classes/*.csproj ./gardenit-api-classes/
COPY gardenit-webapi/*.csproj ./gardenit-webapi/
COPY gardenit-webapi.Tests/*.csproj ./gardenit-webapi.Tests/

#
RUN dotnet restore 
#
# copy everything else and build app
COPY gardenit-api-classes/. ./gardenit-api-classes/
COPY gardenit-webapi/. ./gardenit-webapi/ 
COPY gardenit-webapi.Tests/. ./gardenit-webapi.Tests/

#
WORKDIR /app
RUN dotnet publish -c Release -o ./publish
#
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app/gardenit-webapi
#
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "gardenit-webapi.dll"]




# # syntax=docker/dockerfile:1
# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# WORKDIR /app

# # Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore

# # Copy everything else and build
# COPY ./ ./
# RUN dotnet publish -c Release -o out

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:5.0
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "gardenit-webapi.dll"]