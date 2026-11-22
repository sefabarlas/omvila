# syntax=docker/dockerfile:1
# Tanıtım sitesi: Next.js statik çıktı (out/) + Caddy statik sunucu (salon paneliyle aynı desen).
# Derleme bağlamı depo köküdür (pnpm çalışma alanı):
#   docker build -f web/tanitim/Dockerfile \
#     --build-arg NEXT_PUBLIC_SITE_URL=https://www.test.omvila.com.tr \
#     --build-arg NEXT_PUBLIC_PANEL_URL=https://salon.test.omvila.com.tr \
#     --build-arg NEXT_PUBLIC_NOINDEX=true .
# NEXT_PUBLIC_* değerleri statik dosyalara derlemede gömülür; ortam başına ayrı imaj etiketi gerekir.

FROM node:22-alpine AS build
RUN corepack enable
WORKDIR /repo
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml tsconfig.base.json ./
COPY packages/api-client-ts/package.json packages/api-client-ts/
COPY packages/design-tokens/package.json packages/design-tokens/
COPY packages/ui-web/package.json packages/ui-web/
COPY web/tanitim/package.json web/tanitim/
RUN --mount=type=cache,id=pnpm-store,target=/root/.local/share/pnpm/store \
    pnpm install --frozen-lockfile --filter @omvila/tanitim...
COPY packages/api-client-ts packages/api-client-ts
COPY packages/design-tokens packages/design-tokens
COPY packages/ui-web packages/ui-web
COPY web/tanitim web/tanitim
ARG NEXT_PUBLIC_SITE_URL
ARG NEXT_PUBLIC_PANEL_URL
ARG NEXT_PUBLIC_NOINDEX
ENV NEXT_PUBLIC_SITE_URL=${NEXT_PUBLIC_SITE_URL} \
    NEXT_PUBLIC_PANEL_URL=${NEXT_PUBLIC_PANEL_URL} \
    NEXT_PUBLIC_NOINDEX=${NEXT_PUBLIC_NOINDEX} \
    NEXT_TELEMETRY_DISABLED=1
RUN test -n "$NEXT_PUBLIC_SITE_URL" && test -n "$NEXT_PUBLIC_PANEL_URL" \
    || (echo "NEXT_PUBLIC_SITE_URL ve NEXT_PUBLIC_PANEL_URL derleme argümanları zorunlu" >&2 && exit 1)
RUN pnpm --filter @omvila/tanitim build

FROM caddy:2-alpine AS runtime
COPY web/tanitim/Caddyfile /etc/caddy/Caddyfile
COPY --from=build /repo/web/tanitim/out /srv
# Kök olmayan kullanıcı; 8080 ayrıcalıksız port (TLS kenardaki Caddy'de).
RUN chown -R 65534:65534 /config /data
USER 65534:65534
EXPOSE 8080
HEALTHCHECK --interval=10s --timeout=3s --retries=3 \
  CMD wget -q --spider http://127.0.0.1:8080/ || exit 1
