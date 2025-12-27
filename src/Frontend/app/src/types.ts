export interface CreateAccountPayload {
  firstName: string;
  lastName: string;
  description?: string;
}

export interface TopUpPayload {
  accountNumber: string;
  amount: number;
}

export interface CreateOrderPayload {
  accountNumber: string;
  amount: number;
  description: string;
}

export interface CreateAccountResponseDto {
  AccountNumber?: string;
  accountNumber?: string;
  number?: string;
}

export interface AccountResponseDto {
  Number?: string;
  number?: string;
  accountNumber?: string;
  FirstName?: string;
  firstName?: string;
  LastName?: string;
  lastName?: string;
  Description?: string;
  description?: string;
  Status?: string;
  status?: string;
}

export interface AccountViewModel {
  number: string;
  fullName: string;
  description: string;
  status: string;
}

export interface BalanceResponseDto {
  AccountNumber?: string;
  accountNumber?: string;
  Balance?: number;
  balance?: number;
}

export interface OrderListItemDto {
  PublicId?: string;
  publicId?: string;
  id?: string;
  Amount?: number;
  amount?: number;
  Status?: string;
  status?: string;
}

export interface OrderViewModel {
  publicId: string;
  amount: number;
  status: string;
}
