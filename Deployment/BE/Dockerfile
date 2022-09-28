FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
COPY Output/ App/
WORKDIR /App

ENTRYPOINT ["dotnet", "FileDropBE.dll"]

EXPOSE 80