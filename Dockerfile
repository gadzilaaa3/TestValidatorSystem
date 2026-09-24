FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY TestValidatorSystem.Api/TestValidatorSystem.Api.csproj ./TestValidatorSystem.Api/
RUN dotnet restore ./TestValidatorSystem.Api/TestValidatorSystem.Api.csproj

COPY . .

RUN dotnet publish ./TestValidatorSystem.Api/TestValidatorSystem.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TestValidatorSystem.Api.dll"]