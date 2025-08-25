# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore
COPY GXReservationAPI/*.csproj ./GXReservationAPI/
RUN dotnet restore GXReservationAPI/GXReservationAPI.csproj

# copy everything else and build
COPY GXReservationAPI/. ./GXReservationAPI/
WORKDIR /src/GXReservationAPI
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "GXReservationAPI.dll"]
