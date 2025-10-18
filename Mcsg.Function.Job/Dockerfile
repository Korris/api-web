# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mcsg.Function.Job/Mcsg.Function.Job.csproj", "Mcsg.Function.Job/"]
COPY ["Common/Mcsg.Common.Domain/Mcsg.Common.Domain.csproj", "Common/Mcsg.Common.Domain/"]
COPY ["Common/Mcsg.Common/Mcsg.Common.csproj", "Common/Mcsg.Common/"]
COPY ["Common/Mcsg.Common.Core/Mcsg.Common.Core.csproj", "Common/Mcsg.Common.Core/"]
COPY ["Common/Mcsg.Common.SeedWork/Mcsg.Common.SeedWork.csproj", "Common/Mcsg.Common.SeedWork/"]
RUN dotnet restore "./Mcsg.Function.Job/Mcsg.Function.Job.csproj"
COPY . .
WORKDIR "/src/Mcsg.Function.Job"
RUN dotnet build "./Mcsg.Function.Job.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mcsg.Function.Job.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mcsg.Function.Job.dll"]


ENV ASPNETCORE_ENVIRONMENT=Dev

CMD echo "[ENVIRONMENT] ${ASPNETCORE_ENVIRONMENT}" && dotnet Mcsg.Function.Job.dll