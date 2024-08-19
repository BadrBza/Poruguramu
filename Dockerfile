FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Puroguramu.App/Puroguramu.App.csproj", "Puroguramu.App/"]
RUN dotnet restore "Puroguramu.App/Puroguramu.App.csproj"
COPY . .
WORKDIR "/src/Puroguramu.App"
RUN dotnet build "Puroguramu.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Puroguramu.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

FROM base AS final
EXPOSE 80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Puroguramu.App.dll"]

