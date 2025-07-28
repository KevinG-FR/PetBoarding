FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PetBoarding_Api/PetBoarding_Api.csproj", "PetBoarding_Api/"]
COPY ["PetBoarding_Application/PetBoarding_Application.csproj", "PetBoarding_Application/"]
COPY ["PetBoarding_Domain/PetBoarding_Domain.csproj", "PetBoarding_Domain/"]
COPY ["PetBoarding_Infrastructure/PetBoarding_Infrastructure.csproj", "PetBoarding_Infrastructure/"]
COPY ["PetBoarding_Persistence/PetBoarding_Persistence.csproj", "PetBoarding_Persistence/"]
RUN dotnet restore "PetBoarding_Api/PetBoarding_Api.csproj"
COPY . .
WORKDIR "/src/PetBoarding_Api"
RUN dotnet build "PetBoarding_Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PetBoarding_Api.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UserAppHost=false
# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetBoarding_Api.dll"]