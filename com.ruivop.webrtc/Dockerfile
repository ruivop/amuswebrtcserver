#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["com.ruivop.webrtc/com.ruivop.webrtc.csproj", "com.ruivop.webrtc/"]
RUN dotnet restore "com.ruivop.webrtc/com.ruivop.webrtc.csproj"
COPY . .
WORKDIR "/src/com.ruivop.webrtc"
RUN dotnet build "com.ruivop.webrtc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "com.ruivop.webrtc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "com.ruivop.webrtc.dll"]