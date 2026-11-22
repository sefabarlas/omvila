# syntax=docker/dockerfile:1
# Omvila.Host tek imaj; rol komut argümanıyla seçilir (docs/mimari/mimari.md § Genel resim):
#   --role=api | --role=worker | --role=migrate | --role=seed   (kullanım: deploy/staging/compose.yml)
# Derleme bağlamı api/ klasörüdür: docker build -f api/Dockerfile api

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Önce yalnız proje ve paket dosyaları: kaynak değişince restore katmanı önbellekten gelir.
COPY global.json Directory.Build.props Directory.Packages.props BannedSymbols.txt .editorconfig ./
COPY src/ ./src-all/
RUN mkdir src \
    && (cd src-all && find . -name '*.csproj' -exec cp --parents {} ../src/ \;) \
    && rm -rf src-all \
    && dotnet restore src/Omvila.Host/Omvila.Host.csproj
COPY src/ ./src/
RUN dotnet publish src/Omvila.Host/Omvila.Host.csproj -c Release --no-restore -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
# Uygulama 8080'de dinler; TLS ve dış port ters vekilde (Caddy).
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_NOLOGO=1
COPY --from=build /app ./
# Yüklenen dosyalar (dekont; FileStorage:RootPath, T-118): kök olmayan kullanıcı yazabilsin; kalıcılık için birim bağlanır.
RUN mkdir -p /data/files && chown "$APP_UID" /data/files
# Kök olmayan kullanıcı (aspnet imajındaki "app", UID 1654).
USER $APP_UID
EXPOSE 8080
# Sağlık denetimi imaja ek araç (curl) koymadan bash /dev/tcp ile: /health/ready 200 dönmeli.
HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=6 \
  CMD ["bash", "-c", "exec 3<>/dev/tcp/127.0.0.1/8080 && printf 'GET /health/ready HTTP/1.0\\r\\nHost: localhost\\r\\n\\r\\n' >&3 && head -n 1 <&3 | grep -q ' 200 '"]
ENTRYPOINT ["dotnet", "Omvila.Host.dll"]
CMD ["--role=api"]
