import {
  BarChart3,
  Calendar,
  DoorOpen,
  Hourglass,
  LayoutGrid,
  Package,
  Settings,
  Star,
  Tag,
  User,
  Users,
  Wallet,
} from 'lucide-react';
import type { ReactNode } from 'react';

// Ekran tasarımlarındaki ikonlar (Lucide; salon-paneli/HizmetlerBos, Durumlar).
export const menuIcons: Record<string, ReactNode> = {
  genel: <LayoutGrid size={18} />,
  takvim: <Calendar size={18} />,
  musteriler: <Users size={18} />,
  bekleme: <Hourglass size={18} />,
  kasa: <Wallet size={18} />,
  degerlendirmeler: <Star size={18} />,
  hizmetler: <Tag size={18} />,
  personel: <User size={18} />,
  paketler: <Package size={18} />,
  raporlar: <BarChart3 size={18} />,
  ayarlar: <Settings size={18} />,
  takvimim: <Calendar size={18} />,
  kapida: <DoorOpen size={18} />,
  primim: <Wallet size={18} />,
};
