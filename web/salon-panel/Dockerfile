# syntax=docker/dockerfile:1
# Salon paneli: Vite derlemesi + statik sunucu. Derleme bağlamı depo köküdür (pnpm çalışma alanı):
#   docker build -f web/salon-panel/Dockerfile --build-arg VITE_API_URL=https://api.test.omvila.com.tr .
# Statik sunucu Caddy: kenardaki ters vekille aynı yapılandırma dili, tek ikili, SPA geri düşüşü için
# try_files yeterli; nginx'e göre ek modül veya betik gerekmez.

FROM node:22-alpine AS build
RUN corepack enable
WORKDIR /repo
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml tsconfig.base.json ./
COPY packages/api-client-ts/package.json packages/api-client-ts/
COPY packages/design-tokens/package.json packages/design-tokens/
COPY packages/ui-web/package.json packages/ui-web/
COPY web/salon-panel/package.json web/salon-panel/
RUN --mount=type=cache,id=pnpm-store,target=/root/.local/share/pnpm/store \
    pnpm install --frozen-lockfile --filter @omvila/salon-panel...
COPY packages/api-client-ts packages/api-client-ts
COPY packages/design-tokens packages/design-tokens
COPY packages/ui-web packages/ui-web
COPY web/salon-panel web/salon-panel
# API kökü derlemede gömülür (import.meta.env); ortam başına ayrı imaj etiketi gerekir.
ARG VITE_API_URL
ENV VITE_API_URL=${VITE_API_URL}
RUN test -n "$VITE_API_URL" || (echo "VITE_API_URL derleme argümanı zorunlu" >&2 && exit 1)
RUN pnpm --filter @omvila/salon-panel build

FROM caddy:2-alpine AS runtime
COPY web/salon-panel/Caddyfile /etc/caddy/Caddyfile
COPY --from=build /repo/web/salon-panel/dist /srv
# Kök olmayan kullanıcı; 8080 ayrıcalıksız port (TLS kenardaki Caddy'de).
RUN chown -R 65534:65534 /config /data
USER 65534:65534
EXPOSE 8080
HEALTHCHECK --interval=10s --timeout=3s --retries=3 \
  CMD wget -q --spider http://127.0.0.1:8080/ || exit 1
