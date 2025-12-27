import type {
  AccountViewModel,
  CreateAccountPayload,
  CreateAccountResponseDto,
  CreateOrderPayload,
  OrderViewModel,
  TopUpPayload,
} from './types';

const envUrl = (import.meta.env.VITE_GATEWAY_URL as string | undefined) ?? '';
const rawBaseUrl = typeof envUrl === 'string' && envUrl.trim().length > 0 ? envUrl.trim() : '';

export const gatewayBaseUrl = rawBaseUrl.endsWith('/')
  ? rawBaseUrl.slice(0, -1)
  : rawBaseUrl || 'http://localhost:8080';

const jsonHeaders = {
  'Content-Type': 'application/json',
};

const getErrorMessage = async (response: Response) => {
  const text = await response.text();
  return text || `Request failed with status ${response.status}`;
};

export async function createAccount(payload: CreateAccountPayload): Promise<CreateAccountResponseDto> {
  const response = await fetch(`${gatewayBaseUrl}/accounts`, {
    method: 'POST',
    headers: jsonHeaders,
    body: JSON.stringify({
      firstName: payload.firstName,
      lastName: payload.lastName,
      description: payload.description,
    }),
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  const data = (await response.json()) as Record<string, unknown>;
  const accountNumber = (data.AccountNumber ?? data.accountNumber ?? data.number ?? '') as string;

  return { accountNumber };
}

export async function fetchAccount(accountNumber: string): Promise<AccountViewModel | null> {
  const response = await fetch(`${gatewayBaseUrl}/accounts/${encodeURIComponent(accountNumber)}`);

  if (response.status === 404) return null;
  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  const data = (await response.json()) as Record<string, unknown>;
  const number = (data.Number ?? data.number ?? data.accountNumber ?? '') as string;
  const firstName = (data.FirstName ?? data.firstName ?? '') as string;
  const lastName = (data.LastName ?? data.lastName ?? '') as string;
  const description = (data.Description ?? data.description ?? '') as string;
  const status = (data.Status ?? data.status ?? '') as string;

  return {
    number,
    fullName: `${firstName} ${lastName}`.trim(),
    description,
    status,
  };
}

export async function fetchBalance(accountNumber: string): Promise<number | null> {
  const response = await fetch(
    `${gatewayBaseUrl}/payments/payment-accounts/${encodeURIComponent(accountNumber)}/balance`,
  );

  if (response.status === 404) return null;
  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  const data = (await response.json()) as Record<string, unknown>;
  return (data.Balance ?? data.balance ?? null) as number | null;
}

export async function topUpBalance(payload: TopUpPayload): Promise<void> {
  const response = await fetch(`${gatewayBaseUrl}/payments/payment-accounts/topup`, {
    method: 'POST',
    headers: jsonHeaders,
    body: JSON.stringify({
      accountNumber: payload.accountNumber,
      amount: payload.amount,
    }),
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }
}

export async function ensurePaymentAccount(accountNumber: string): Promise<void> {
  if (!accountNumber) return;

  const response = await fetch(`${gatewayBaseUrl}/payments/payment-accounts`, {
    method: 'POST',
    headers: jsonHeaders,
    body: JSON.stringify({ accountNumber }),
  });

  if (!response.ok && response.status !== 409) {
    throw new Error(await getErrorMessage(response));
  }
}

export async function createOrder(payload: CreateOrderPayload): Promise<void> {
  const response = await fetch(`${gatewayBaseUrl}/orders`, {
    method: 'POST',
    headers: jsonHeaders,
    body: JSON.stringify({
      accountNumber: payload.accountNumber,
      amount: payload.amount,
      description: payload.description,
    }),
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }
}

export async function fetchOrders(accountNumber: string): Promise<OrderViewModel[]> {
  const response = await fetch(
    `${gatewayBaseUrl}/orders?accountNumber=${encodeURIComponent(accountNumber)}`,
  );

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  const data = (await response.json()) as Record<string, unknown>[];
  return data.map((order) => ({
    publicId: (order.PublicId ?? order.publicId ?? order.id ?? '') as string,
    amount: Number(order.Amount ?? order.amount ?? 0),
    status: String(order.Status ?? order.status ?? 'Unknown'),
  }));
}
