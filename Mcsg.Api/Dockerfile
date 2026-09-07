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
COPY ["Mcsg.Api/Mcsg.Api.csproj", "Mcsg.Api/"]
COPY ["Common/Mcsg.Common.Domain/Mcsg.Common.Domain.csproj", "Common/Mcsg.Common.Domain/"]
COPY ["Common/Mcsg.Common/Mcsg.Common.csproj", "Common/Mcsg.Common/"]
COPY ["Common/Mcsg.Common.Core/Mcsg.Common.Core.csproj", "Common/Mcsg.Common.Core/"]
COPY ["Common/Mcsg.Common.SeedWork/Mcsg.Common.SeedWork.csproj", "Common/Mcsg.Common.SeedWork/"]
RUN dotnet restore "./Mcsg.Api/Mcsg.Api.csproj"
COPY . .
WORKDIR "/src/Mcsg.Api"
RUN dotnet build "./Mcsg.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mcsg.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# -------- Add environment mapping --------
ARG ENV=dev
RUN echo "Building for ENV=$ENV"

ENV ASPNETCORE_ENVIRONMENT=${ENV}

ARG ORIGINS=http://localhost:3000;https://focfoc-web-stg.nulyxa.dev
ENV Origins=${ORIGINS}

RUN if [ "$ENV" = "dev" ]; then export ASPNETCORE_ENVIRONMENT=Dev; fi && \
    if [ "$ENV" = "staging" ]; then export ASPNETCORE_ENVIRONMENT=Stg; fi && \
    if [ "$ENV" = "prod" ]; then export ASPNETCORE_ENVIRONMENT=Prod; fi && \
    echo "Final ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT" > /app/env-info.txt

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Mcsg.Api.dll"]
