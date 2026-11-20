import type { StoryDefault } from '@ladle/react';
import { useState } from 'react';
import { Button } from './Button';
import { Drawer, Modal } from './Modal';
import { previewRow } from './preview';
import { TextField } from './TextField';

export default { title: 'Panel/Katman' } satisfies StoryDefault;

export const ModalVeYanPanel = () => {
  const [modal, setModal] = useState(false);
  const [drawer, setDrawer] = useState(false);
  return (
    <div style={previewRow}>
      <Button
        variant="primary"
        onClick={() => {
          setModal(true);
        }}
      >
        Modal aç
      </Button>
      <Button
        onClick={() => {
          setDrawer(true);
        }}
      >
        Yan panel aç
      </Button>
      <Modal
        open={modal}
        onClose={() => {
          setModal(false);
        }}
        title="Randevuyu iptal et"
        description="Salı 25 Kasım 14:30 · Ayşe Yılmaz · Saç kesimi"
        footer={
          <>
            <Button
              onClick={() => {
                setModal(false);
              }}
            >
              Vazgeç
            </Button>
            <Button variant="danger">Randevuyu iptal et</Button>
          </>
        }
      >
        Müşteriye iptal SMS'i gider. Bu işlem geri alınamaz.
      </Modal>
      <Drawer
        open={drawer}
        onClose={() => {
          setDrawer(false);
        }}
        title="Personel ekle"
        description="Kadıköy"
        footer={<Button variant="primary">Personeli kaydet</Button>}
      >
        <TextField label="Ad soyad" placeholder="Deniz Kara" />
        <TextField label="Telefon" placeholder="0538 274 19 05" />
      </Drawer>
    </div>
  );
};

export const AcikModal = () => (
  <Modal
    open
    onClose={() => undefined}
    title="Yeni kategori"
    footer={<Button variant="primary">Kategoriyi kaydet</Button>}
  >
    <TextField label="Kategori adı" placeholder="Saç" />
  </Modal>
);
