// Elle yazılan ince sarmalayıcı. Kurallar: packages/api-client-ts/AGENTS.md,
// docs/sozlesme/api-kurallari.md (Idempotency, eşzamanlı düzenleme, hatalar).
import createClient, { type Client } from 'openapi-fetch';

import type { components, paths } from './generated/schema';

export type ErrorCode = components['schemas']['ErrorCode'];
export type Problem = components['schemas']['Problem'];

// Sarmalayıcının kendisinin doldurduğu başlıklar: Idempotency-Key (POST'ta eksikse üretilir),
// X-Client-Type ve X-Client (ApiClientOptions.headers). Tiplerde isteğe bağlı olurlar;
// If-Match gibi çağıranın vermesi gerekenler zorunlu kalır.
type AutoHeader = 'Idempotency-Key' | 'X-Client-Type' | 'X-Client';
type RelaxHeader<H> = Omit<H, AutoHeader> & Partial<Pick<H, Extract<keyof H, AutoHeader>>>;
type RelaxParameters<Prm> = Prm extends { header: infer H }
  ? Omit<Prm, 'header'> &
      (object extends Omit<H, AutoHeader>
        ? { header?: RelaxHeader<H> }
        : { header: RelaxHeader<H> })
  : Prm;
type RelaxOperation<Op> = Op extends { parameters: infer Prm }
  ? Omit<Op, 'parameters'> & { parameters: RelaxParameters<Prm> }
  : Op;
type RelaxPath<Item> = { [Method in keyof Item]: RelaxOperation<Item[Method]> };

type PathsOf<Surface extends string> = {
  [K in keyof paths as K extends `/api/v1/${Surface}/${string}` ? K : never]: RelaxPath<paths[K]>;
};
export type ManagePaths = PathsOf<'manage'>;
export type PublicPaths = PathsOf<'public'>;
export type MePaths = PathsOf<'me'>;
// admin uçları Dilim 6'da sözleşmeye girer; o zamana kadar boş.
// eslint-disable-next-line @typescript-eslint/no-generated-empty-object-type
export type AdminPaths = PathsOf<'admin'>;

/** Sunucunun döndürdüğü RFC 9457 hatası. Kullanıcıya `title`/`detail` değil, `code`'a göre metin gösterilir. */
export class ApiProblem extends Error {
  readonly status: number;
  /** Bilinmeyen kod da gelebilir (yeni sürüm); istemci bunu genel hata olarak ele alır. */
  readonly code: ErrorCode | (string & {});
  readonly meta: Record<string, unknown>;
  readonly errors: Record<string, string[]>;
  readonly traceId: string | undefined;

  constructor(status: number, body: unknown) {
    const problem = isProblemLike(body) ? body : undefined;
    const code = problem?.code ?? (status >= 500 ? 'try_again' : 'unknown');
    super(code);
    this.name = 'ApiProblem';
    this.status = status;
    this.code = code;
    this.meta = problem?.meta ?? {};
    this.errors = problem?.errors ?? {};
    this.traceId = problem?.traceId;
  }
}

function isProblemLike(
  body: unknown,
): body is Partial<Pick<Problem, 'meta' | 'errors' | 'traceId'>> & { code: string } {
  return (
    typeof body === 'object' && body !== null && 'code' in body && typeof body.code === 'string'
  );
}

export interface ApiClientOptions {
  /** API kökü, ör. http://localhost:4010 (Prism) veya http://localhost:5080. */
  baseUrl: string;
  /** Her denemede okunur; yenilemeden sonra yeni jeton kullanılır. */
  getAccessToken?: () => string | null | undefined;
  /** `401 token_expired` sonrası bir kez çağrılır. Yenilendiyse `true` döner. Eşzamanlı 401'ler tek yenilemeyi paylaşır. */
  refreshAccessToken?: () => Promise<boolean>;
  /** Her isteğe eklenecek sabit başlıklar (`X-Client-Type`, `X-Client`, web'de `X-Requested-With`). */
  headers?: Record<string, string>;
  /** Web'de yenileme çerezi için `include`. */
  credentials?: RequestCredentials;
  /** Test ve sunucu tarafı için. Varsayılan: global fetch. */
  fetch?: (request: Request) => Promise<Response>;
  /** Varsayılan: crypto.randomUUID(). */
  newIdempotencyKey?: () => string;
}

/** Kaynak oluşturan POST'lar için `Idempotency-Key` başlığı (tip olarak zorunlu olan uçlarda kullanın). */
export function idempotencyKey(): { 'Idempotency-Key': string } {
  return { 'Idempotency-Key': crypto.randomUUID() };
}

/** Düzenleme ve durum eylemleri için `If-Match`: sorgu verisindeki `version` (ETag ile aynı değer). */
export function ifMatch(version: string): { 'If-Match': string } {
  return { 'If-Match': version };
}

async function problemCode(response: Response): Promise<string | undefined> {
  try {
    const body: unknown = await response.clone().json();
    return isProblemLike(body) ? body.code : undefined;
  } catch {
    return undefined;
  }
}

function createSender(options: ApiClientOptions): (request: Request) => Promise<Response> {
  const baseFetch = options.fetch ?? ((request: Request) => globalThis.fetch(request));
  const newKey = options.newIdempotencyKey ?? (() => crypto.randomUUID());
  let refreshing: Promise<boolean> | undefined;

  const refreshOnce = (): Promise<boolean> => {
    if (!options.refreshAccessToken) return Promise.resolve(false);
    refreshing ??= options.refreshAccessToken().finally(() => {
      refreshing = undefined;
    });
    return refreshing;
  };

  const attempt = (request: Request): Promise<Response> => {
    const sent = request.clone();
    const token = options.getAccessToken?.();
    if (token) sent.headers.set('Authorization', `Bearer ${token}`);
    return baseFetch(sent);
  };

  return async (request: Request): Promise<Response> => {
    // Anahtar bir kez üretilir; tekrar denemeler aynı anahtarı taşır.
    if (request.method === 'POST' && !request.headers.has('Idempotency-Key')) {
      request.headers.set('Idempotency-Key', newKey());
    }

    let response = await attempt(request);

    if (response.status === 401 && (await problemCode(response)) === 'token_expired') {
      if (await refreshOnce()) response = await attempt(request);
    }
    if (response.status === 503 && (await problemCode(response)) === 'try_again') {
      response = await attempt(request);
    }
    return response;
  };
}

function create<P extends object>(options: ApiClientOptions): Client<P> {
  return createClient<P>({
    baseUrl: options.baseUrl,
    fetch: createSender(options),
    ...(options.headers ? { headers: options.headers } : {}),
    ...(options.credentials ? { credentials: options.credentials } : {}),
  });
}

export const createManageClient = (options: ApiClientOptions): Client<ManagePaths> =>
  create<ManagePaths>(options);
export const createPublicClient = (options: ApiClientOptions): Client<PublicPaths> =>
  create<PublicPaths>(options);
export const createMeClient = (options: ApiClientOptions): Client<MePaths> =>
  create<MePaths>(options);
export const createAdminClient = (options: ApiClientOptions): Client<AdminPaths> =>
  create<AdminPaths>(options);

/**
 * openapi-fetch sonucunu veriye çevirir; hata yanıtında `ApiProblem` fırlatır (TanStack Query ile kullanım için).
 */
export async function unwrap<T>(
  call: Promise<{ data?: T; error?: unknown; response: Response }>,
): Promise<T> {
  const { data, error, response } = await call;
  if (!response.ok || error !== undefined) throw new ApiProblem(response.status, error);
  return data as T;
}
