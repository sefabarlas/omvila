import { unwrap } from '@omvila/api-client-ts';
import type { OtpChallenge } from '@omvila/ui-web/panel';
import { useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useApi } from '@/app/api-context';
import { auth } from '@/app/auth';
import { privacyNoticeVersion } from '@/app/config';
import { queryKeys } from '@/app/query-keys';
import type { IndustryCode } from '@/shared/industries';

export type OtpPurpose = 'Login' | 'Signup';

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
