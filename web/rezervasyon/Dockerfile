# syntax=docker/dockerfile:1
# Rezervasyon sayfası: Next.js standalone çıktısı. Derleme bağlamı depo köküdür (pnpm çalışma alanı):
#   docker build -f web/rezervasyon/Dockerfile \
#     --build-arg NEXT_PUBLIC_API_URL=https://test.api.omvila.com.tr \
#     --build-arg NEXT_PUBLIC_SITE_URL=https://test.randevu.omvila.com.tr .
# NEXT_PUBLIC_* değerleri istemci paketine derlemede gömülür; ortam başına ayrı imaj etiketi gerekir.

FROM node:22-alpine AS build
RUN corepack enable
WORKDIR /repo
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml tsconfig.base.json ./
COPY packages/api-client-ts/package.json packages/api-client-ts/
COPY packages/design-tokens/package.json packages/design-tokens/
COPY packages/ui-web/package.json packages/ui-web/
COPY web/rezervasyon/package.json web/rezervasyon/
RUN --mount=type=cache,id=pnpm-store,target=/root/.local/share/pnpm/store \
    pnpm install --frozen-lockfile --filter @omvila/rezervasyon...
COPY packages/api-client-ts packages/api-client-ts
COPY packages/design-tokens packages/design-tokens
COPY packages/ui-web packages/ui-web
COPY web/rezervasyon web/rezervasyon
ARG NEXT_PUBLIC_API_URL
ARG NEXT_PUBLIC_SITE_URL
ARG NEXT_PUBLIC_PRIVACY_NOTICE_URL
ARG NEXT_PUBLIC_PRIVACY_NOTICE_VERSION
ENV NEXT_PUBLIC_API_URL=${NEXT_PUBLIC_API_URL} \
    NEXT_PUBLIC_SITE_URL=${NEXT_PUBLIC_SITE_URL} \
    NEXT_PUBLIC_PRIVACY_NOTICE_URL=${NEXT_PUBLIC_PRIVACY_NOTICE_URL} \
    NEXT_PUBLIC_PRIVACY_NOTICE_VERSION=${NEXT_PUBLIC_PRIVACY_NOTICE_VERSION} \
    NEXT_TELEMETRY_DISABLED=1
RUN test -n "$NEXT_PUBLIC_API_URL" && test -n "$NEXT_PUBLIC_SITE_URL" \
    || (echo "NEXT_PUBLIC_API_URL ve NEXT_PUBLIC_SITE_URL derleme argümanları zorunlu" >&2 && exit 1)
# Boş verilen isteğe bağlı değişken "tanımsız" sayılsın (kod `??` ile varsayılana düşer).
RUN [ -n "$NEXT_PUBLIC_PRIVACY_NOTICE_URL" ] || unset NEXT_PUBLIC_PRIVACY_NOTICE_URL; \
    [ -n "$NEXT_PUBLIC_PRIVACY_NOTICE_VERSION" ] || unset NEXT_PUBLIC_PRIVACY_NOTICE_VERSION; \
    pnpm --filter @omvila/rezervasyon build

FROM node:22-alpine AS runtime
WORKDIR /app
ENV NODE_ENV=production \
    NEXT_TELEMETRY_DISABLED=1 \
    HOSTNAME=0.0.0.0 \
    PORT=3000
# Standalone çıktı depo kökünü izler (outputFileTracingRoot): sunucu web/rezervasyon/server.js altında.
COPY --from=build --chown=node:node /repo/web/rezervasyon/.next/standalone ./
COPY --from=build --chown=node:node /repo/web/rezervasyon/.next/static ./web/rezervasyon/.next/static
# Kök olmayan kullanıcı (node imajındaki "node").
USER node
EXPOSE 3000
HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=3 \
  CMD wget -q --spider http://127.0.0.1:3000/ || exit 1
CMD ["node", "web/rezervasyon/server.js"]
