// Girişte kayıtlı olmayan numara (SignupRequired, kabul kriteri 3): doğrulanmış telefonun kayıt jetonu kayıt
// ekranına bellekte taşınır; telefon ve kod yeniden sorulmaz. Jeton URL'ye yazılmaz.

interface PendingSignup {
  signupToken: string;
  phoneMasked: string;
}

let pending: PendingSignup | null = null;

export const signupHandoff = {
  set: (value: PendingSignup): void => {
    pending = value;
  },
  peek: (): PendingSignup | null => pending,
  clear: (): void => {
    pending = null;
  },
};
