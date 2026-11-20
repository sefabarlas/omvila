import { createFileRoute } from '@tanstack/react-router';
import { SignupScreen } from '@/features/auth/SignupScreen';

export const Route = createFileRoute('/kayit')({ component: SignupScreen });
