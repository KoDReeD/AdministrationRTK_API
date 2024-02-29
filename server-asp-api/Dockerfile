FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["server-asp-api/server-asp-api.csproj", "server-asp-api/"]
RUN dotnet restore "server-asp-api/server-asp-api.csproj"
COPY . .
WORKDIR "/src/server-asp-api"
RUN dotnet build "server-asp-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "server-asp-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "server-asp-api.dll"]
