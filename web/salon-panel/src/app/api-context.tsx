import { createContext, useContext } from 'react';
import type { Api } from './api';

export const ApiContext = createContext<Api | null>(null);

/** Üretilmiş API istemcisi (yalnız @omvila/api-client-ts; elle fetch yok). */
export function useApi(): Api {
  const api = useContext(ApiContext);
  if (!api) {
    throw new Error('useApi, ApiContext içinde kullanılmalı.');
  }

  return api;
}
