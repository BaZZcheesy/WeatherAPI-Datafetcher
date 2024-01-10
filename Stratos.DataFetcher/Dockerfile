FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["M347_Data_Fetcher/M347_Data_Fetcher.csproj", "M347_Data_Fetcher/"]
RUN dotnet restore "M347_Data_Fetcher/M347_Data_Fetcher.csproj"
COPY . .
WORKDIR "/src/M347_Data_Fetcher"
RUN dotnet build "M347_Data_Fetcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "M347_Data_Fetcher.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "M347_Data_Fetcher.dll"]
