# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore
COPY GXRoomReservationAPI/*.csproj ./GXRoomReservationAPI/
RUN dotnet restore GXRoomReservationAPI/GXRoomReservationAPI.csproj

# copy everything else and build
COPY GXRoomReservationAPI/. ./GXRoomReservationAPI/
WORKDIR /src/GXRoomReservationAPI
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "GXRoomReservationAPI.dll"]
