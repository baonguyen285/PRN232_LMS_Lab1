FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY publish/ .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]