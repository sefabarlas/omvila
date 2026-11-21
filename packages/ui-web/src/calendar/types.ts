// Takvimin sunum verisi. Sözleşmedeki CalendarView (contracts/openapi.yaml) ile hizalıdır; çağıran tarafı
// API yanıtını buna çevirir (durum → StatusKey: status/displayStatus). Bileşenler API çağırmaz.
import type { StatusKey } from '../status';

/** Personel (gün görünümü kolonu). */
export interface CalendarStaff {
  id: string;
  name: string;
  /** Staff.CalendarColor ("#14857A"); yalnız kolon başlığındaki avatar halkasında kullanılır. */
  color: string;
  photoUrl?: string | null;
  /** Başlık alt satırı, veriden: "5 randevu · bekleyen var". Verilmezse gösterilmez. */
  subtitle?: string;
}

/** Blok içindeki dilim (SegmentView); staffBusy = false olan dilim "Bekleme · müsait" şerididir. */
export interface CalendarSegment {
  startUtc: string;
  endUtc: string;
  staffBusy: boolean;
}

/** Takvim bloğu (bir randevu kalemi = bir blok; CalendarAppointment). */
export interface CalendarBlock {
  appointmentId: string;
  itemId: string;
  staffId: string | null;
  startUtc: string;
  endUtc: string;
  status: StatusKey;
  customerName: string;
  serviceName: string;
  isRecurring?: boolean;
  segments?: readonly CalendarSegment[];
  /** false: sürüklenemez (geçmiş, tamamlanmış randevu gibi; karar çağıranındır). Varsayılan true. */
  draggable?: boolean;
}

/** Gölgeli aralık: izin, mola, çalışma saati dışı (TimeOff ve staffHours'tan çağıran üretir). */
export interface CalendarUnavailableRange {
  staffId: string;
  startUtc: string;
  endUtc: string;
  /** Kısa etiket: "İzin", "Mola". */
  label?: string;
}

/** Boş alana tıklama: o personel ve 15 dakikaya yuvarlanmış başlangıç anı. */
export interface CalendarSlot {
  staffId: string;
  /** Şubenin yerel takvim günü. */
  date: string;
  /** UTC an (ISO). */
  startUtc: string;
}

/** Sürükleme sırasında ve bırakınca hedef. */
export interface CalendarDropTarget {
  appointmentId: string;
  itemId: string;
  staffId: string;
  date: string;
  /** Yerel saat "14:00" ve bitiş "14:45" (süre korunur). */
  startTime: string;
  endTime: string;
  startUtc: string;
  endUtc: string;
  /** Hedef personel veya gün değişti mi / saat değişti mi. */
  staffChanged: boolean;
  timeChanged: boolean;
}

/** onMove yükü; sonucu çağıran belirler (409 slot_taken → veri değişmez, blok eski yerinde kalır). */
export interface CalendarMove {
  appointmentId: string;
  itemId: string;
  staffId: string;
  startUtc: string;
}

/** Ortak ızgara davranışı (gün ve hafta). */
export interface CalendarCommonProps {
  /** Şubenin saat dilimi (IANA). Cihaz saat dilimi kullanılmaz. */
  timeZone: string;
  /** Izgaranın başladığı ve bittiği yerel saat ("09:00", "19:00"); tam saate genişletilir. */
  openTime: string;
  closeTime: string;
  blocks: readonly CalendarBlock[];
  unavailable?: readonly CalendarUnavailableRange[];
  /** Enjekte edilen şimdiki an; verilmezse şu anki saat çizgisi çizilmez. */
  now?: Date;
  onSlotClick?: (slot: CalendarSlot) => void;
  onBlockClick?: (appointmentId: string, block: CalendarBlock) => void;
  /** Verilirse bloklar sürüklenebilir. */
  onMove?: (move: CalendarMove) => void;
  /** Sürükleme ipucu: "Zeynep Aksoy'a taşı · 14:00–14:45 · müşteriye SMS gider". */
  describeDrop?: (target: CalendarDropTarget) => string;
  /** Bölge adı (ekran okuyucu). */
  label?: string;
  className?: string;
}
