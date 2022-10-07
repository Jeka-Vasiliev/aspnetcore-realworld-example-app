#build container
FROM mcr.microsoft.com/dotnet/sdk:6.0.401 as build

WORKDIR /build
COPY . .
RUN dotnet run -p build/build.csproj

#runtime container
FROM mcr.microsoft.com/dotnet/aspnet:6.0.9-alpine3.16
RUN apk add --no-cache tzdata

COPY --from=build /build/publish /app
WORKDIR /app

EXPOSE 5000

ENTRYPOINT ["dotnet", "Conduit.dll"]
