#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Finbot.Service/Finbot.Service.csproj", "Finbot.Service/"]
COPY ["Finbot.Data/Finbot.Data.csproj", "Finbot.Data/"]
COPY ["Finbot.Models/Finbot.Models.csproj", "Finbot.Models/"]
COPY ["Finbot.Discord/Finbot.Discord.csproj", "Finbot.Discord/"]
COPY ["Finbot.MarketData/Finbot.MarketData.csproj", "Finbot.MarketData/"]
COPY ["Finbot.Trading/Finbot.Trading.csproj", "Finbot.Trading/"]
RUN dotnet restore "./Finbot.Service/./Finbot.Service.csproj"
COPY . .
WORKDIR "/src/Finbot.Service"
RUN dotnet build "./Finbot.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Finbot.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Finbot.Service.dll"]