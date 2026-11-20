import { unwrap } from '@omvila/api-client-ts';
import { useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useApi } from '@/app/api-context';
import { auth } from '@/app/auth';
import { privacyNoticeVersion } from '@/app/config';
import { queryKeys } from '@/app/query-keys';

export type OtpPurpose = 'Login' | 'Signup';

export interface OtpChallenge {
  otpId: string;
  phoneMasked: string;
  resendAfterSeconds: number;
}

/** manage-auth uçları (ADR-0007). Web'de yenileme jetonu HttpOnly çerezde; erişim jetonu bellekte. */
export function useAuthApi() {
  const api = useApi();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const signedIn = async (accessToken: string | undefined, to: string) => {
    auth.setToken(accessToken ?? null);
    await queryClient.invalidateQueries({ queryKey: queryKeys.session() });
    await navigate({ to });
  };

  return {
    requestOtp: async (phone: string, purpose: OtpPurpose): Promise<OtpChallenge> => {
      const data = await unwrap(
        api.POST('/api/v1/manage/auth/otp/request', { body: { phone, purpose } }),
      );
      return {
        otpId: data.otpId,
        phoneMasked: data.phoneMasked,
        resendAfterSeconds: data.resendAfterSeconds,
      };
    },
    verifyOtp: (otpId: string, code: string, rememberDevice: boolean) =>
      unwrap(api.POST('/api/v1/manage/auth/otp/verify', { body: { otpId, code, rememberDevice } })),
    signup: (signupToken: string, businessName: string, industryCode: IndustryCode) =>
      unwrap(
        api.POST('/api/v1/manage/auth/signup', {
          body: {
            signupToken,
            businessName,
            industryCode,
            acceptedTerms: true,
            privacyNoticeVersion,
          },
        }),
      ),
    signedIn,
  };
}

export const industries = [
  { value: 'hair_salon', label: 'Kuaför' },
  { value: 'barber', label: 'Berber' },
  { value: 'beauty', label: 'Güzellik salonu' },
  { value: 'nail', label: 'Tırnak' },
  { value: 'spa', label: 'Spa ve masaj' },
  { value: 'dietitian', label: 'Diyetisyen' },
  { value: 'other', label: 'Diğer' },
] as const;

export type IndustryCode = (typeof industries)[number]['value'];
