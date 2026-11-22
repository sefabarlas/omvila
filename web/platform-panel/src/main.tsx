import { QueryClient } from '@tanstack/react-query';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { createApi } from './app/api';
import { App } from './App';
import { createAppRouter } from './router';

const api = createApi();
const queryClient = new QueryClient();
const router = createAppRouter(api, queryClient);
const root = document.getElementById('root');
if (root) {
  createRoot(root).render(
    <StrictMode>
      <App api={api} queryClient={queryClient} router={router} />
    </StrictMode>,
  );
}
