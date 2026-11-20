import { Checkbox } from '@omvila/ui-web/panel';
import type { Control, FieldPath, FieldValues } from 'react-hook-form';
import { Controller } from 'react-hook-form';

export interface CheckboxGroupProps<T extends FieldValues> {
  control: Control<T>;
  name: FieldPath<T>;
  options: readonly { value: string; label: string; description?: string }[];
}

/**
 * Çok seçimli onay kutuları, değer her zaman dizi. react-hook-form'da `register` ile tek seçenekli grup dizi yerine
 * true/false döndürür (tek personelli salonda "Kimler veriyor?" gibi); bu bileşen değeri kendisi yönetir.
 */
export function CheckboxGroup<T extends FieldValues>({
  control,
  name,
  options,
}: CheckboxGroupProps<T>) {
  return (
    <Controller
      control={control}
      name={name}
      render={({ field }) => {
        const selected = (field.value as string[] | undefined) ?? [];
        return (
          <>
            {options.map((option) => (
              <Checkbox
                key={option.value}
                label={option.label}
                {...(option.description ? { description: option.description } : {})}
                checked={selected.includes(option.value)}
                onChange={(event) => {
                  field.onChange(
                    event.target.checked
                      ? [...selected, option.value]
                      : selected.filter((value) => value !== option.value),
                  );
                }}
              />
            ))}
          </>
        );
      }}
    />
  );
}
