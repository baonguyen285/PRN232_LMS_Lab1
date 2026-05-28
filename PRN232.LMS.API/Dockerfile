# Stage 1: Build và Publish ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy các file dự án .csproj để restore dependencies trước
COPY ["PRN232.LMS.API/PRN232.LMS.API.csproj", "PRN232.LMS.API/"]
COPY ["PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj", "PRN232.LMS.Repositories/"]
COPY ["PRN232.LMS.Services/PRN232.LMS.Services.csproj", "PRN232.LMS.Services/"]
RUN dotnet restore "PRN232.LMS.API/PRN232.LMS.API.csproj"

# Copy toàn bộ mã nguồn còn lại và biên dịch
COPY . .
WORKDIR "/src/PRN232.LMS.API"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime Image (Chạy ứng dụng)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]