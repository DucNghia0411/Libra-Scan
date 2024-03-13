#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./ScanAPI/LIBRA.Scan.API/LIBRA.Scan.API.csproj", "API/ScanAPI/LIBRA.Scan.API/"]
COPY ["./ScanAPI/LIBRA.Scan.API.Data/LIBRA.Scan.API.Data.csproj", "API/ScanAPI/LIBRA.Scan.API.Data/"]
COPY ["./ScanAPI/LIBRA.Scan.API.Entities/LIBRA.Scan.API.Entities.csproj", "API/ScanAPI/LIBRA.Scan.API.Entities/"]
COPY ["./ScanAPI/LBIRA.Scan.API.Common/LIBRA.Scan.API.Common.csproj", "API/ScanAPI/LBIRA.Scan.API.Common/"]
COPY ["./ScanAPI/LIBRA.Scan.API.Service/LIBRA.Scan.API.Service.csproj", "API/ScanAPI/LIBRA.Scan.API.Service/"]
RUN dotnet restore "/src/API/ScanAPI/LIBRA.Scan.API/LIBRA.Scan.API.csproj"
COPY . .
WORKDIR "/src/./ScanAPI/LIBRA.Scan.API"
RUN dotnet build "LIBRA.Scan.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LIBRA.Scan.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LIBRA.Scan.API.dll"]