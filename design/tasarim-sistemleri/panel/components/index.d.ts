import type * as React from 'react';
export type Durum = 'onay' | 'kapora' | 'onaylandi' | 'geldi' | 'tamamlandi' | 'gelmedi' | 'iptal';
export interface Randevu { id: string | number; start: string; end: string; customer: string; service: string; status: Durum; staffId?: string; dayId?: string }
export interface LogoProps { tone?: 'acik' | 'ters'; showWordmark?: boolean; size?: number }
export interface IconProps extends React.SVGAttributes<SVGElement> { name: string; size?: number; strokeWidth?: number }
export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> { variant?: 'primary' | 'secondary' | 'ghost' | 'danger'; size?: 'sm' | 'md' | 'lg'; icon?: string; iconRight?: string; loading?: boolean }
export interface IconButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> { icon: string; label: string; variant?: 'primary' | 'secondary' | 'ghost' | 'danger'; size?: 'sm' | 'md' | 'lg' }
export interface TextFieldProps extends React.InputHTMLAttributes<HTMLInputElement> { label?: string; hint?: string; error?: string; icon?: string; suffix?: React.ReactNode }
export interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> { label?: string; hint?: string; options: Array<string | { value: string; label: string }> }
export interface CheckboxProps extends React.InputHTMLAttributes<HTMLInputElement> { label?: React.ReactNode; description?: React.ReactNode }
export type RadioProps = CheckboxProps;
export type SwitchProps = CheckboxProps;
export interface BadgeProps { tone?: 'notr' | 'marka' | 'vurgu' | 'basari' | 'uyari' | 'hata' | 'bilgi'; dot?: boolean; icon?: string; children?: React.ReactNode }
export interface StatusBadgeProps { status: Durum; children?: React.ReactNode }
export interface AvatarProps { name: string; src?: string; size?: 'sm' | 'md' | 'lg' }
export interface TooltipProps { text: string; open?: boolean; children: React.ReactNode }
export interface TabsProps { items: Array<{ id: string; label: string; count?: number }>; value?: string; onChange?: (id: string) => void }
export interface ModalProps { title: string; description?: string; open?: boolean; inline?: boolean; onClose?: () => void; footer?: React.ReactNode; size?: 'md' | 'lg'; children?: React.ReactNode }
export interface ToastProps { tone?: 'basari' | 'hata' | 'uyari' | 'bilgi'; title: string; description?: string; action?: React.ReactNode; onClose?: () => void }
export interface BannerProps { tone?: 'basari' | 'hata' | 'uyari' | 'bilgi'; title?: string; action?: React.ReactNode; children?: React.ReactNode }
export interface EmptyStateProps { icon?: string; title: string; description?: string; action?: React.ReactNode }
export interface SidebarProps { items: Array<{ title?: string; items: Array<{ id: string; label: string; icon: string; count?: number }> }>; active?: string; branch?: string; user?: { name: string; role?: string }; collapsed?: boolean }
export interface TopbarProps { title: string; subtitle?: string; actions?: React.ReactNode; search?: boolean; notifications?: number }
export interface FilterBarProps { filters: Array<{ id: string; label: string; active?: boolean }>; onRemove?: (id: string) => void; children?: React.ReactNode }
export interface StatCardProps { label: string; value: React.ReactNode; delta?: string; trend?: 'up' | 'down'; hint?: string; icon?: string }
export interface DataTableProps<R = any> { columns: Array<{ key: string; label: string; align?: 'left' | 'right' | 'center'; numeric?: boolean; width?: string; render?: (row: R) => React.ReactNode }>; rows: R[]; selectable?: boolean; footer?: React.ReactNode }
export interface CustomerRowProps { name: string; phone: string; meta?: string; tags?: Array<{ label: string; tone?: BadgeProps['tone'] }>; action?: React.ReactNode }
export interface AppointmentBlockProps { appointment: Randevu; compact?: boolean; style?: React.CSSProperties }
export interface AppointmentCardProps { appointment: Randevu; staff?: string; phone?: string; price?: string; channel?: 'Online' | 'Panel' | 'Phone' | 'WalkIn'; onCheckIn?: () => void }
export interface CalendarToolbarProps { dateLabel: string; view?: 'gun' | 'hafta'; onView?: (v: 'gun' | 'hafta') => void; staffFilter?: React.ReactNode }
export interface DayCalendarProps { staff: Array<{ id: string; name: string; color?: string }>; appointments: Randevu[]; startHour?: number; endHour?: number; now?: string }
export interface WeekCalendarProps { days: Array<{ id: string; weekday: string; day: number }>; appointments: Randevu[]; startHour?: number; endHour?: number; now?: string; today?: string }
export interface TimeSlotPickerProps { groups: Array<{ label: string; slots: Array<{ time: string; available: boolean }> }>; value?: string; onChange?: (t: string) => void }
export declare function Logo(props: LogoProps): React.ReactElement;
export declare function Icon(props: IconProps): React.ReactElement;
export declare function Button(props: ButtonProps): React.ReactElement;
export declare function IconButton(props: IconButtonProps): React.ReactElement;
export declare function TextField(props: TextFieldProps): React.ReactElement;
export declare function Select(props: SelectProps): React.ReactElement;
export declare function Checkbox(props: CheckboxProps): React.ReactElement;
export declare function Radio(props: RadioProps): React.ReactElement;
export declare function Switch(props: SwitchProps): React.ReactElement;
export declare function Badge(props: BadgeProps): React.ReactElement;
export declare function StatusBadge(props: StatusBadgeProps): React.ReactElement;
export declare function Avatar(props: AvatarProps): React.ReactElement;
export declare function Tooltip(props: TooltipProps): React.ReactElement;
export declare function Tabs(props: TabsProps): React.ReactElement;
export declare function Modal(props: ModalProps): React.ReactElement;
export declare function Toast(props: ToastProps): React.ReactElement;
export declare function Banner(props: BannerProps): React.ReactElement;
export declare function EmptyState(props: EmptyStateProps): React.ReactElement;
export declare function Sidebar(props: SidebarProps): React.ReactElement;
export declare function Topbar(props: TopbarProps): React.ReactElement;
export declare function FilterBar(props: FilterBarProps): React.ReactElement;
export declare function StatCard(props: StatCardProps): React.ReactElement;
export declare function DataTable(props: DataTableProps): React.ReactElement;
export declare function CustomerRow(props: CustomerRowProps): React.ReactElement;
export declare function AppointmentBlock(props: AppointmentBlockProps): React.ReactElement;
export declare function AppointmentCard(props: AppointmentCardProps): React.ReactElement;
export declare function CalendarToolbar(props: CalendarToolbarProps): React.ReactElement;
export declare function DayCalendar(props: DayCalendarProps): React.ReactElement;
export declare function WeekCalendar(props: WeekCalendarProps): React.ReactElement;
export declare function TimeSlotPicker(props: TimeSlotPickerProps): React.ReactElement;
declare global { interface Window { OmvilaPanel: { Logo: typeof Logo; Icon: typeof Icon; Button: typeof Button; IconButton: typeof IconButton; TextField: typeof TextField; Select: typeof Select; Checkbox: typeof Checkbox; Radio: typeof Radio; Switch: typeof Switch; Badge: typeof Badge; StatusBadge: typeof StatusBadge; Avatar: typeof Avatar; Tooltip: typeof Tooltip; Tabs: typeof Tabs; Modal: typeof Modal; Toast: typeof Toast; Banner: typeof Banner; EmptyState: typeof EmptyState; Sidebar: typeof Sidebar; Topbar: typeof Topbar; FilterBar: typeof FilterBar; StatCard: typeof StatCard; DataTable: typeof DataTable; CustomerRow: typeof CustomerRow; AppointmentBlock: typeof AppointmentBlock; AppointmentCard: typeof AppointmentCard; CalendarToolbar: typeof CalendarToolbar; DayCalendar: typeof DayCalendar; WeekCalendar: typeof WeekCalendar; TimeSlotPicker: typeof TimeSlotPicker } } }
