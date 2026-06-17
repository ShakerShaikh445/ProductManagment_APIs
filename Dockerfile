FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore FMCO_APIs/FMCO_APIs.csproj
RUN dotnet publish FMCO_APIs/FMCO_APIs.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FMCO_APIs.dll"]