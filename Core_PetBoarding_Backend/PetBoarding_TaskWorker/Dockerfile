FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
EXPOSE 8080
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Core_PetBoarding_Backend/PetBoarding_TaskWorker/PetBoarding_TaskWorker.csproj", "PetBoarding_TaskWorker/"]
COPY ["Core_PetBoarding_Backend/PetBoarding_Application/PetBoarding_Application.csproj", "PetBoarding_Application/"]
COPY ["Core_PetBoarding_Backend/PetBoarding_Domain/PetBoarding_Domain.csproj", "PetBoarding_Domain/"]
COPY ["Core_PetBoarding_Backend/PetBoarding_Infrastructure/PetBoarding_Infrastructure.csproj", "PetBoarding_Infrastructure/"]
COPY ["Core_PetBoarding_Backend/PetBoarding_Persistence/PetBoarding_Persistence.csproj", "PetBoarding_Persistence/"]

# Restore dependencies
RUN dotnet restore "PetBoarding_TaskWorker/PetBoarding_TaskWorker.csproj"

# Copy source code
COPY Core_PetBoarding_Backend/ .
WORKDIR "/src/PetBoarding_TaskWorker"

# Build the application
RUN dotnet build "PetBoarding_TaskWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PetBoarding_TaskWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configure logging and environment
ENV DOTNET_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PetBoarding_TaskWorker.dll"]