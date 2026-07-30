# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0.202 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["Users.Common/Users.Common.csproj", "Users.Common/"]
COPY ["src/SharedKernal/SharedKernal.csproj", "src/SharedKernal/"]
COPY ["src/Users.Domain/Users.Domain.csproj", "src/Users.Domain/"]
COPY ["src/Users.Application/Users.Application.csproj", "src/Users.Application/"]
COPY ["src/Users.Infrastracture/Users.Infrastracture.csproj", "src/Users.Infrastracture/"]
COPY ["src/Users.Presentation/Users.Presentation.csproj", "src/Users.Presentation/"]
COPY ["src/CleanModular.ServiceDefaults/CleanModular.ServiceDefaults.csproj", "src/CleanModular.ServiceDefaults/"]
COPY ["src/Host.WebApi/Host.WebApi.csproj", "src/Host.WebApi/"]
RUN dotnet restore "src/Host.WebApi/Host.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Host.WebApi"
RUN dotnet build "./Host.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Host.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.WebApi.dll"]
