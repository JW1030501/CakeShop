# 階段 1：使用 SDK 進行編譯
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 💡 修正：路徑要包含子資料夾 CakeShop/
COPY ["CakeShop/CakeShop.csproj", "CakeShop/"]
RUN dotnet restore "CakeShop/CakeShop.csproj"

# 複製所有內容到 /src
COPY . .

# 💡 修正：切換到正確的專案目錄進行發佈
WORKDIR "/src/CakeShop"
RUN dotnet publish "CakeShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 階段 2：使用 Runtime 執行環境
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 設定環境變數，讓 .NET 監聽 Render 分配的 Port
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# 請確保這裡的 dll 名稱與你的專案名稱完全一致
ENTRYPOINT ["dotnet", "CakeShop.dll"]