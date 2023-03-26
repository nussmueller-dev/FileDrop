#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FileDropBE.csproj", "."]
RUN dotnet restore "./FileDropBE.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FileDropBE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileDropBE.csproj" -c Release -o /app/publish

FROM base AS final
ENV DB_SERVER_NAME=FileDrop-DB
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileDropBE.dll"]