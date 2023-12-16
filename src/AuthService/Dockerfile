#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/AuthService/AuthService.csproj", "src/AuthService/"]
COPY ["src/AuthService.Business/AuthService.Business.csproj", "src/AuthService.Business/"]
COPY ["src/AuthService.Repository/AuthService.Repository.csproj", "src/AuthService.Repository/"]
COPY ["src/AuthService.Shared/AuthService.Shared.csproj", "src/AuthService.Shared/"]
RUN dotnet restore "src/AuthService/AuthService.csproj"
COPY . .
WORKDIR "/src/src/AuthService"
RUN dotnet build "AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]