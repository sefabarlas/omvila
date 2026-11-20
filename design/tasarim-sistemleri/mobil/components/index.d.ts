import type * as React from 'react';
export type Durum = 'onay' | 'kapora' | 'onaylandi' | 'geldi' | 'tamamlandi' | 'gelmedi' | 'iptal';
export interface LogoProps { tone?: 'acik' | 'ters'; showWordmark?: boolean; size?: number }
export interface IconProps { name: string; size?: number; filled?: boolean; strokeWidth?: number }
export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> { variant?: 'primary' | 'secondary' | 'ghost' | 'danger'; size?: 'sm' | 'md' | 'lg'; block?: boolean; icon?: string; loading?: boolean }
export interface IconButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> { icon: string; label: string; variant?: 'ghost' | 'secondary' | 'tonal' }
export interface TextFieldProps extends React.InputHTMLAttributes<HTMLInputElement> { label?: string; hint?: string; error?: string; icon?: string; prefix?: string }
export type PhoneFieldProps = TextFieldProps;
export interface OtpInputProps { length?: number; value?: string; error?: string; label?: string }
export interface SwitchProps extends React.InputHTMLAttributes<HTMLInputElement> { label: string }
export interface ListItemProps { leading?: string | React.ReactNode; title: React.ReactNode; subtitle?: React.ReactNode; value?: React.ReactNode; trailing?: 'chevron' | React.ReactNode; danger?: boolean; onClick?: () => void }
export interface ListGroupProps { title?: string; children: React.ReactNode }
export interface BadgeProps { tone?: 'notr' | 'marka' | 'vurgu' | 'basari' | 'uyari' | 'hata'; children: React.ReactNode }
export interface StatusBadgeProps { status: Durum; children?: React.ReactNode }
export interface AvatarProps { name: string; size?: 'sm' | 'md' | 'lg'; src?: string }
export interface ChipProps extends React.ButtonHTMLAttributes<HTMLButtonElement> { selected?: boolean; icon?: string }
export interface SegmentedControlProps { items: Array<{ id: string; label: string }>; value?: string }
export interface AppBarProps { title: string; large?: boolean; back?: boolean; actions?: React.ReactNode; subtitle?: string }
export interface TabBarProps { items: Array<{ id: string; label: string; icon: string; badge?: number }>; active: string }
export interface BottomSheetProps { title?: string; footer?: React.ReactNode; children?: React.ReactNode }
export interface DialogProps { title: string; message?: string; confirm?: string; cancel?: string; danger?: boolean }
export interface SnackbarProps { message: string; action?: string; tone?: 'basari' | 'hata' }
export interface EmptyStateProps { icon?: string; title: string; description?: string; action?: React.ReactNode }
export interface SkeletonProps { lines?: number; avatar?: boolean }
export interface BookingStepsProps { steps?: string[]; current: number }
export interface SalonHeaderProps { name: string; category?: string; address?: string; rating?: string; reviews?: number; open?: string }
export interface ServiceItemProps { name: string; duration: string; price: string; description?: string; selected?: boolean; onSelect?: () => void }
export interface StaffPickerProps { staff: Array<{ id: string; name: string }>; value?: string }
export interface DayStripProps { days: Array<{ id: string; weekday: string; day: number; dot?: boolean; closed?: boolean }>; value?: string }
export interface TimeSlotGridProps { slots: Array<{ time: string; available: boolean }>; value?: string }
export interface StickyFooterProps { summary: string; detail?: string; action: React.ReactNode }
export interface AppointmentListItemProps { appointment: { start: string; end: string; customer: string; service: string; status: Durum }; staff?: string; onCheckIn?: () => void }
export interface AppointmentTicketProps { salon: string; service: string; staff: string; date: string; time: string; duration: string; price: string; status?: Durum; past?: boolean }
export interface PackageCardProps { name: string; salon: string; used: number; total: number; expires: string }
export interface RatingInputProps { value?: number; label?: string }
export declare function Logo(props: LogoProps): React.ReactElement;
export declare function Icon(props: IconProps): React.ReactElement;
export declare function Button(props: ButtonProps): React.ReactElement;
export declare function IconButton(props: IconButtonProps): React.ReactElement;
export declare function TextField(props: TextFieldProps): React.ReactElement;
export declare function PhoneField(props: PhoneFieldProps): React.ReactElement;
export declare function OtpInput(props: OtpInputProps): React.ReactElement;
export declare function Switch(props: SwitchProps): React.ReactElement;
export declare function ListItem(props: ListItemProps): React.ReactElement;
export declare function ListGroup(props: ListGroupProps): React.ReactElement;
export declare function Badge(props: BadgeProps): React.ReactElement;
export declare function StatusBadge(props: StatusBadgeProps): React.ReactElement;
export declare function Avatar(props: AvatarProps): React.ReactElement;
export declare function Chip(props: ChipProps): React.ReactElement;
export declare function SegmentedControl(props: SegmentedControlProps): React.ReactElement;
export declare function AppBar(props: AppBarProps): React.ReactElement;
export declare function TabBar(props: TabBarProps): React.ReactElement;
export declare function BottomSheet(props: BottomSheetProps): React.ReactElement;
export declare function Dialog(props: DialogProps): React.ReactElement;
export declare function Snackbar(props: SnackbarProps): React.ReactElement;
export declare function EmptyState(props: EmptyStateProps): React.ReactElement;
export declare function Skeleton(props: SkeletonProps): React.ReactElement;
export declare function BookingSteps(props: BookingStepsProps): React.ReactElement;
export declare function SalonHeader(props: SalonHeaderProps): React.ReactElement;
export declare function ServiceItem(props: ServiceItemProps): React.ReactElement;
export declare function StaffPicker(props: StaffPickerProps): React.ReactElement;
export declare function DayStrip(props: DayStripProps): React.ReactElement;
export declare function TimeSlotGrid(props: TimeSlotGridProps): React.ReactElement;
export declare function StickyFooter(props: StickyFooterProps): React.ReactElement;
export declare function AppointmentListItem(props: AppointmentListItemProps): React.ReactElement;
export declare function AppointmentTicket(props: AppointmentTicketProps): React.ReactElement;
export declare function PackageCard(props: PackageCardProps): React.ReactElement;
export declare function RatingInput(props: RatingInputProps): React.ReactElement;
declare global { interface Window { OmvilaMobil: { Logo: typeof Logo; Icon: typeof Icon; Button: typeof Button; IconButton: typeof IconButton; TextField: typeof TextField; PhoneField: typeof PhoneField; OtpInput: typeof OtpInput; Switch: typeof Switch; ListItem: typeof ListItem; ListGroup: typeof ListGroup; Badge: typeof Badge; StatusBadge: typeof StatusBadge; Avatar: typeof Avatar; Chip: typeof Chip; SegmentedControl: typeof SegmentedControl; AppBar: typeof AppBar; TabBar: typeof TabBar; BottomSheet: typeof BottomSheet; Dialog: typeof Dialog; Snackbar: typeof Snackbar; EmptyState: typeof EmptyState; Skeleton: typeof Skeleton; BookingSteps: typeof BookingSteps; SalonHeader: typeof SalonHeader; ServiceItem: typeof ServiceItem; StaffPicker: typeof StaffPicker; DayStrip: typeof DayStrip; TimeSlotGrid: typeof TimeSlotGrid; StickyFooter: typeof StickyFooter; AppointmentListItem: typeof AppointmentListItem; AppointmentTicket: typeof AppointmentTicket; PackageCard: typeof PackageCard; RatingInput: typeof RatingInput } } }
