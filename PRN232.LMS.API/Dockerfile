# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy các file .csproj trước để tận dụng Docker layer caching (chỉ restore khi .csproj thay đổi)
COPY ["PRN232.LMS.API/PRN232.LMS.API.csproj", "PRN232.LMS.API/"]
COPY ["PRN232.LMS.Services/PRN232.LMS.Services.csproj", "PRN232.LMS.Services/"]
COPY ["PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj", "PRN232.LMS.Repositories/"]

RUN dotnet restore "PRN232.LMS.API/PRN232.LMS.API.csproj"

# Copy toàn bộ source code
COPY . .

# Build project
WORKDIR "/src/PRN232.LMS.API"
RUN dotnet build "PRN232.LMS.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "PRN232.LMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime image (nhỏ gọn nhất)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy output đã publish từ stage 2
COPY --from=publish /app/publish .

# Expose cổng 8080 (HTTP)
EXPOSE 8080

ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]