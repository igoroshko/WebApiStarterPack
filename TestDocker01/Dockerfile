FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY TestDocker01/TestDocker01.csproj TestDocker01/
RUN dotnet restore TestDocker01/TestDocker01.csproj
COPY . .
WORKDIR /src/TestDocker01
RUN dotnet build TestDocker01.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TestDocker01.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TestDocker01.dll"]
