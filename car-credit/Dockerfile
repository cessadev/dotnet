FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY car-credit.csproj .
RUN dotnet restore car-credit.csproj

COPY . .
RUN dotnet publish car-credit.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5099
EXPOSE 5099

ENTRYPOINT ["dotnet", "car-credit.dll"]