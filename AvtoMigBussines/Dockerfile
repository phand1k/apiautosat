#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AvtoMigBussines/AvtoMigBussines.csproj", "AvtoMigBussines/"]
RUN dotnet restore "AvtoMigBussines/AvtoMigBussines.csproj"
COPY . .
WORKDIR "/src/AvtoMigBussines"
RUN dotnet build "AvtoMigBussines.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AvtoMigBussines.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AvtoMigBussines.dll"]