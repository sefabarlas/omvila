import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useCreateCategory } from './api';
import type { ServiceCategory } from './api';

/** salon-paneli/KategoriEkle. Aynı adda kategori varsa sunucu 409 döner. Açıkken bağlanır. */
export function CategoryModal({
  onClose,
  onCreated,
}: {
  onClose: () => void;
  onCreated: (category: ServiceCategory) => void;
}) {
  const toast = useToast();
  const create = useCreateCategory();
  const [name, setName] = useState('');
  const [error, setError] = useState<string>();

  const onSubmit = async (event: SyntheticEvent) => {
    event.preventDefault();
    const trimmed = name.trim();
    if (trimmed === '') {
      setError('Kategori adını girin.');
      return;
    }

    try {
      const category = await create.mutateAsync(trimmed);
      toast.show({ tone: 'basari', title: `${category.name} kategorisi eklendi` });
      onCreated(category);
      onClose();
    } catch (problem) {
      if (problem instanceof ApiProblem && problem.status === 409) {
        setError('Bu adda bir kategori zaten var.');
        return;
      }

      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Kategori ekle"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="kategori-ekle"
            loading={create.isPending}
            loadingLabel="Ekleniyor…"
          >
            Kategoriyi ekle
          </Button>
        </>
      }
    >
      <form id="kategori-ekle" noValidate onSubmit={(e) => void onSubmit(e)}>
        <TextField
          label="Kategori adı"
          placeholder="Makyaj"
          maxLength={60}
          hint="Kategoriler rezervasyon sayfasında hizmet sekmeleri olarak görünür."
          error={error}
          value={name}
          onChange={(event) => {
            setName(event.target.value);
            setError(undefined);
          }}
        />
      </form>
    </Modal>
  );
}
