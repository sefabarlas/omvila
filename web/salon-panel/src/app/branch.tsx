import { createContext, useContext } from 'react';

export interface BranchSelection {
  branchId: string | undefined;
  setBranchId: (branchId: string) => void;
}

/** Kenar menüsündeki şube seçimi; şubeye bağlı sayfalar (hizmet fiyatları, saatler) okur. */
export const BranchContext = createContext<BranchSelection>({
  branchId: undefined,
  setBranchId: () => undefined,
});

export function useSelectedBranch(): BranchSelection {
  return useContext(BranchContext);
}
