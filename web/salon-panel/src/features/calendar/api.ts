import { ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { queryOptions, useMutation, useQueryClient } from '@tanstack/react-query';
import type { QueryClient } from '@tanstack/react-query';
import type { Api } from '@/app/api';
import { useApi } from '@/app/api-context';
import { queryKeys } from '@/app/query-keys';

export type CalendarView = components['schemas']['CalendarView'];
export type CalendarAppointment = components['schemas']['CalendarAppointment'];
export type CalendarDay = CalendarView['days'][number];
export type AppointmentDetail = components['schemas']['AppointmentDetail'];
export type CancelRequest = components['schemas']['ManageCancelRequest'];
export type RescheduleRequest = components['schemas']['ManageRescheduleRequest'];
export type ApproveRequest = components['schemas']['ManageApproveRequest'];

export interface CalendarRange {
  branchId: string;
  from: string;
  to: string;
  /** Hafta görünümü: tek personel. */
  staffId?: string | undefined;
}

/** Takvim (manageGetCalendar). Personel rolünde sunucu yalnız kendi kolonunu döner. */
export function calendarQuery(api: Api, { branchId, from, to, staffId }: CalendarRange) {
  return queryOptions({
    queryKey: queryKeys.calendar(branchId, from, to, staffId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/calendar', {
          params: { query: { branchId, from, to, ...(staffId ? { staffIds: [staffId] } : {}) } },
        }),
      ),
  });
}

/** Randevu ayrıntısı (manageGetAppointment); If-Match için sürüm her açılışta tazelenir. */
export function appointmentQuery(api: Api, appointmentId: string) {
  return queryOptions({
    queryKey: queryKeys.appointment(appointmentId),
    queryFn: () =>
      unwrap(
        api.GET('/api/v1/manage/appointments/{appointmentId}', {
          params: { path: { appointmentId } },
        }),
      ),
    staleTime: 0,
  });
}

function invalidateCalendar(queryClient: QueryClient) {
  return queryClient.invalidateQueries({ queryKey: ['calendar'] });
}

export function useAppointmentAction<TInput>(
  send: (api: Api, detail: AppointmentDetail, input: TInput) => Promise<AppointmentDetail>,
) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ detail, input }: { detail: AppointmentDetail; input: TInput }) =>
      send(api, detail, input),
    onSuccess: (saved) => {
      queryClient.setQueryData(queryKeys.appointment(saved.id), saved);
    },
    // Durum eylemleri onay kuyruğunu da değiştirebilir (onay, red, onay bekleyen talebin iptali).
    onSettled: () =>
      Promise.all([
        invalidateCalendar(queryClient),
        queryClient.invalidateQueries({ queryKey: ['approval-queue'] }),
      ]),
  });
}

export const appointmentPath = (detail: AppointmentDetail) => ({
  path: { appointmentId: detail.id },
  header: ifMatch(detail.version),
});

/** "Geldi" (manageCheckInAppointment). */
export function useCheckIn() {
  return useAppointmentAction<undefined>((api, detail) =>
    unwrap(
      api.POST('/api/v1/manage/appointments/{appointmentId}/check-in', {
        params: appointmentPath(detail),
      }),
    ),
  );
}

/** "Geldiyse geri al" (manageUndoNoShow): yalnız aynı yerel gün. */
export function useUndoNoShow() {
  return useAppointmentAction<undefined>((api, detail) =>
    unwrap(
      api.POST('/api/v1/manage/appointments/{appointmentId}/undo-no-show', {
        params: appointmentPath(detail),
      }),
    ),
  );
}

/** "Randevuyu iptal et" (manageCancelAppointment); tekrarlayanda kapsam seçilir. */
export function useCancelAppointment() {
  return useAppointmentAction<CancelRequest>((api, detail, body) =>
    unwrap(
      api.POST('/api/v1/manage/appointments/{appointmentId}/cancel', {
        params: appointmentPath(detail),
        body,
      }),
    ),
  );
}

/**
 * "Onayla" (manageApproveAppointment, A2). Kapora bekleyen randevuda "Kaporasız onayla" waiveDeposit = true gönderir;
 * düz onay gövdesizdir (kapora bekleniyorken sunucu 409 deposit_required döner).
 */
export function useApproveAppointment() {
  return useAppointmentAction<ApproveRequest | undefined>((api, detail, body) =>
    unwrap(
      api.POST('/api/v1/manage/appointments/{appointmentId}/approve', {
        params: appointmentPath(detail),
        ...(body ? { body } : {}),
      }),
    ),
  );
}

/** "Reddet" (manageDeclineAppointment, A4): gövdesiz; müşteriye "iptal" bildirimi gider. */
export function useDeclineAppointment() {
  return useAppointmentAction<undefined>((api, detail) =>
    unwrap(
      api.POST('/api/v1/manage/appointments/{appointmentId}/decline', {
        params: appointmentPath(detail),
      }),
    ),
  );
}

export interface MoveInput {
  appointmentId: string;
  itemId: string;
  /** Bloğun (kalemin) eski başlangıcı; randevunun yeni başlangıcı aynı farkla kayar. */
  fromUtc: string;
  startUtc: string;
  /** Personel değiştiyse yeni personel. */
  staffId?: string | undefined;
  allowOutsideWorkingHours?: boolean;
}

/**
 * Sürükle-bırak (manageRescheduleAppointment): sürüm için ayrıntı tazelenir, randevunun başlangıcı kalemin kaydığı
 * kadar kayar. Takvim önce iyimser güncellenir; hata olursa eski hâline döner (409 slot_taken → blok yerine döner).
 */
export function useMoveAppointment(range: CalendarRange) {
  const api = useApi();
  const queryClient = useQueryClient();
  const key = queryKeys.calendar(range.branchId, range.from, range.to, range.staffId);
  return useMutation({
    mutationFn: async (move: MoveInput) => {
      const detail = await queryClient.query(appointmentQuery(api, move.appointmentId));
      const shift = Date.parse(move.startUtc) - Date.parse(move.fromUtc);
      const body: RescheduleRequest = {
        startUtc: new Date(Date.parse(detail.startUtc) + shift).toISOString(),
        ...(move.staffId ? { items: [{ itemId: move.itemId, staffId: move.staffId }] } : {}),
        ...(detail.isRecurring ? { scope: 'ThisOnly' as const } : {}),
        ...(move.allowOutsideWorkingHours ? { allowOutsideWorkingHours: true } : {}),
      };
      return unwrap(
        api.POST('/api/v1/manage/appointments/{appointmentId}/reschedule', {
          params: appointmentPath(detail),
          body,
        }),
      );
    },
    onMutate: async (move) => {
      await queryClient.cancelQueries({ queryKey: key });
      const previous = queryClient.getQueryData<CalendarView>(key);
      if (previous) {
        queryClient.setQueryData<CalendarView>(key, applyMove(previous, move));
      }

      return { previous };
    },
    onError: (_error, _move, context) => {
      if (context?.previous) {
        queryClient.setQueryData(key, context.previous);
      }
    },
    onSuccess: (saved) => {
      queryClient.setQueryData(queryKeys.appointment(saved.id), saved);
    },
    onSettled: () => invalidateCalendar(queryClient),
  });
}

/** Bütün randevu kalemlerini aynı farkla kaydırır; taşınan kalemin personelini değiştirir. */
export function applyMove(view: CalendarView, move: MoveInput): CalendarView {
  const shift = Date.parse(move.startUtc) - Date.parse(move.fromUtc);
  const moveInstant = (iso: string) => new Date(Date.parse(iso) + shift).toISOString();
  return {
    ...view,
    appointments: view.appointments.map((item) =>
      item.appointmentId !== move.appointmentId
        ? item
        : {
            ...item,
            startUtc: moveInstant(item.startUtc),
            endUtc: moveInstant(item.endUtc),
            staffId: item.itemId === move.itemId && move.staffId ? move.staffId : item.staffId,
            segments: item.segments.map((segment) => ({
              ...segment,
              startUtc: moveInstant(segment.startUtc),
              endUtc: moveInstant(segment.endUtc),
            })),
          },
    ),
  };
}

/** "Yalnızca bu gün için aç" (manageCreateBranchOverride, ExtraOpening): haftalık düzen değişmez. */
export function useOpenDay(branchId: string) {
  const api = useApi();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ date, start, end }: { date: string; start: string; end: string }) =>
      unwrap(
        api.POST('/api/v1/manage/branches/{branchId}/overrides', {
          params: { path: { branchId } },
          body: {
            fromDate: date,
            kind: 'ExtraOpening',
            isClosed: false,
            intervals: [{ start, end }],
            note: null,
          },
        }),
      ),
    onSettled: () =>
      Promise.all([
        invalidateCalendar(queryClient),
        queryClient.invalidateQueries({ queryKey: [...queryKeys.branch(branchId), 'overrides'] }),
      ]),
  });
}
