import type { FormEvent } from 'react';
import { useEffect, useMemo, useState } from 'react';
import './App.css';
import {
  createAccount,
  createOrder,
  ensurePaymentAccount,
  fetchAccount,
  fetchBalance,
  fetchOrders,
  gatewayBaseUrl,
  topUpBalance,
} from './api';
import type { AccountViewModel, OrderViewModel } from './types';

function App() {
  const [activeAccount, setActiveAccount] = useState<string>(() => {
    return localStorage.getItem('activeAccount') ?? '';
  });
  const [accountInput, setAccountInput] = useState(activeAccount);
  const [account, setAccount] = useState<AccountViewModel | null>(null);
  const [balance, setBalance] = useState<number | null>(null);
  const [orders, setOrders] = useState<OrderViewModel[]>([]);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loadingDashboard, setLoadingDashboard] = useState(false);

  const [createForm, setCreateForm] = useState({
    firstName: '',
    lastName: '',
    description: '',
  });
  const [creatingAccount, setCreatingAccount] = useState(false);

  const [topUpAmount, setTopUpAmount] = useState('500');
  const [toppingUp, setToppingUp] = useState(false);

  const [orderForm, setOrderForm] = useState({
    amount: '1200',
    description: '',
  });
  const [creatingOrder, setCreatingOrder] = useState(false);

  useEffect(() => {
    if (activeAccount) {
      localStorage.setItem('activeAccount', activeAccount);
      setAccountInput(activeAccount);
    } else {
      localStorage.removeItem('activeAccount');
    }
  }, [activeAccount]);

  useEffect(() => {
    if (!activeAccount) {
      setAccount(null);
      setBalance(null);
      setOrders([]);
      return;
    }

    loadDashboard(activeAccount);
  }, [activeAccount]);

  const loadDashboard = async (accountNumber: string) => {
    setLoadingDashboard(true);
    setError(null);
    setMessage(null);

    try {
      const profile = await fetchAccount(accountNumber);

      if (!profile) {
        setAccount(null);
        setOrders([]);
        setBalance(null);
        setError('Аккаунт не найден. Создайте его или выберите другой номер.');
        return;
      }

      await ensurePaymentAccount(accountNumber);

      const [bal, ordersList] = await Promise.all([
        fetchBalance(accountNumber),
        fetchOrders(accountNumber),
      ]);

      setAccount(profile);
      setBalance(bal);
      setOrders(ordersList);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoadingDashboard(false);
    }
  };

  const handleConnect = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    setMessage(null);

    const number = accountInput.trim();
    if (!number) {
      setError('Введите номер аккаунта, чтобы загрузить данные.');
      return;
    }

    setActiveAccount(number);
  };

  const handleCreateAccount = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);
    setMessage(null);
    setCreatingAccount(true);

    if (!createForm.firstName.trim() || !createForm.lastName.trim()) {
      setError('Имя и фамилия обязательны.');
      setCreatingAccount(false);
      return;
    }

    try {
      const result = await createAccount({
        firstName: createForm.firstName.trim(),
        lastName: createForm.lastName.trim(),
        description: createForm.description.trim(),
      });

      const number = result.accountNumber ?? result.AccountNumber ?? result.number ?? '';
      setCreateForm({ firstName: '', lastName: '', description: '' });
      setAccountInput(number);
      setMessage('Аккаунт создан. Подключаем профиль...');
      if (number) setActiveAccount(number);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setCreatingAccount(false);
    }
  };

  const handleTopUp = async (event: FormEvent) => {
    event.preventDefault();
    if (!activeAccount) {
      setError('Сначала выберите или создайте аккаунт.');
      return;
    }

    const amount = Number.parseFloat(topUpAmount);
    if (Number.isNaN(amount) || amount <= 0) {
      setError('Сумма должна быть положительным числом.');
      return;
    }

    setError(null);
    setMessage(null);
    setToppingUp(true);

    try {
      await topUpBalance({ accountNumber: activeAccount, amount });
      setMessage('Баланс пополнен.');
      const newBalance = await fetchBalance(activeAccount);
      setBalance(newBalance);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setToppingUp(false);
    }
  };

  const handleCreateOrder = async (event: FormEvent) => {
    event.preventDefault();
    if (!activeAccount) {
      setError('Сначала выберите или создайте аккаунт.');
      return;
    }

    const amount = Number.parseFloat(orderForm.amount);
    if (Number.isNaN(amount) || amount <= 0) {
      setError('Сумма заказа должна быть больше нуля.');
      return;
    }

    if (!orderForm.description.trim()) {
      setError('Добавьте описание заказа.');
      return;
    }

    setError(null);
    setMessage(null);
    setCreatingOrder(true);

    try {
      await createOrder({
        accountNumber: activeAccount,
        amount,
        description: orderForm.description.trim(),
      });
      setMessage('Заказ отправлен, статус появится после оплаты.');
      setOrderForm({ amount: '1200', description: '' });
      const freshOrders = await fetchOrders(activeAccount);
      setOrders(freshOrders);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setCreatingOrder(false);
    }
  };

  const refreshAll = async () => {
    if (!activeAccount) return;
    await loadDashboard(activeAccount);
  };

  const hasPaymentAccount = balance !== null;

  const headline = useMemo(() => {
    if (account) return `Аккаунт ${account.number}`;
    if (activeAccount) return `Работаем с номером ${activeAccount}`;
    return 'Подключите аккаунт';
  }, [account, activeAccount]);

  return (
    <div className="page">
      <header className="hero">
        <div className="hero__content">
          <p className="eyebrow">Gozon Store · React UI</p>
          <h1>Единая витрина для аккаунтов, баланса и заказов</h1>
          <p className="lead">
            Фронтенд общается только с API Gateway, собирая данные из Accounts, Payments и Orders.
            Пополните баланс, создайте заказ и посмотрите, как меняется статус.
          </p>
          <div className="hero__meta">
            <span className="pill">Gateway: {gatewayBaseUrl}</span>
            <button className="ghost" onClick={refreshAll} disabled={!activeAccount || loadingDashboard}>
              Обновить данные
            </button>
          </div>
        </div>
      </header>

      {(message || error) && (
        <div className="toast-stack">
          {message && <div className="toast success">{message}</div>}
          {error && <div className="toast danger">{error}</div>}
        </div>
      )}

      <main className="grid">
        <section className="card card--wide">
          <div className="card__header">
            <div>
              <p className="eyebrow">Профиль</p>
              <h2>{headline}</h2>
            </div>
            {activeAccount && (
              <div className="badge">
                {loadingDashboard ? 'Обновляем...' : account ? account.status : 'Не найдено'}
              </div>
            )}
          </div>

          <form className="form-inline" onSubmit={handleConnect}>
            <label className="label" htmlFor="account-number">
              Номер аккаунта
            </label>
            <div className="inline-group">
              <input
                id="account-number"
                value={accountInput}
                onChange={(e) => setAccountInput(e.target.value)}
                placeholder="ACC-XXXX..."
              />
              <button type="submit" disabled={!accountInput.trim()}>
                Подключить
              </button>
            </div>
            <p className="hint">
              Введите существующий номер или создайте новый ниже. Состояние сохраняется в браузере.
            </p>
          </form>

          <div className="profile-grid">
            <div className="profile-card">
              <p className="eyebrow">Имя</p>
              <p className="profile-value">{account?.fullName ?? '—'}</p>
            </div>
            <div className="profile-card">
              <p className="eyebrow">Описание</p>
              <p className="profile-value">{account?.description || 'Пока пусто'}</p>
            </div>
            <div className="profile-card">
              <p className="eyebrow">Статус</p>
              <p className={`status status--${(account?.status ?? 'unknown').toLowerCase()}`}>
                {account?.status ?? 'Не выбрано'}
              </p>
            </div>
            <div className="profile-card">
              <p className="eyebrow">Balance</p>
              <p className="profile-value">
                {hasPaymentAccount ? formatCurrency(balance ?? 0) : 'Нет платежного аккаунта'}
              </p>
            </div>
          </div>
        </section>

        <section className="card">
          <div className="card__header">
            <div>
              <p className="eyebrow">Регистрация</p>
              <h3>Создать новый аккаунт</h3>
            </div>
          </div>
          <form className="stack" onSubmit={handleCreateAccount}>
            <label className="label">Имя</label>
            <input
              value={createForm.firstName}
              onChange={(e) => setCreateForm({ ...createForm, firstName: e.target.value })}
              placeholder="Александр"
            />
            <label className="label">Фамилия</label>
            <input
              value={createForm.lastName}
              onChange={(e) => setCreateForm({ ...createForm, lastName: e.target.value })}
              placeholder="Иванов"
            />
            <label className="label">Описание</label>
            <textarea
              rows={3}
              value={createForm.description}
              onChange={(e) => setCreateForm({ ...createForm, description: e.target.value })}
              placeholder="Покупаю комиксы и мерч"
            />
            <button type="submit" disabled={creatingAccount}>
              {creatingAccount ? 'Создаём...' : 'Создать аккаунт'}
            </button>
          </form>
        </section>

        <section className="card">
          <div className="card__header">
            <div>
              <p className="eyebrow">Баланс</p>
              <h3>Пополнение</h3>
            </div>
            {hasPaymentAccount ? (
              <div className="pill pill--accent">{formatCurrency(balance ?? 0)}</div>
            ) : (
              <div className="pill">Баланс появится после первого пополнения</div>
            )}
          </div>
          <form className="stack" onSubmit={handleTopUp}>
            <label className="label">Сумма, ₽</label>
            <input
              inputMode="decimal"
              value={topUpAmount}
              onChange={(e) => setTopUpAmount(e.target.value)}
              placeholder="500"
            />
            <button type="submit" disabled={toppingUp || !activeAccount}>
              {toppingUp ? 'Отправляем...' : 'Пополнить баланс'}
            </button>
            <p className="hint">Платежный аккаунт создаётся автоматически при первом пополнении.</p>
          </form>
        </section>

        <section className="card">
          <div className="card__header">
            <div>
              <p className="eyebrow">Заказ</p>
              <h3>Создать заказ</h3>
            </div>
            <div className="pill">Асинхронная оплата через RabbitMQ</div>
          </div>
          <form className="stack" onSubmit={handleCreateOrder}>
            <label className="label">Сумма, ₽</label>
            <input
              inputMode="decimal"
              value={orderForm.amount}
              onChange={(e) => setOrderForm({ ...orderForm, amount: e.target.value })}
              placeholder="1200"
            />
            <label className="label">Описание</label>
            <textarea
              rows={3}
              value={orderForm.description}
              onChange={(e) => setOrderForm({ ...orderForm, description: e.target.value })}
              placeholder="Например, фигурка, книга, подписка"
            />
            <button type="submit" disabled={creatingOrder || !activeAccount}>
              {creatingOrder ? 'Отправляем...' : 'Создать заказ'}
            </button>
          </form>
        </section>

        <section className="card card--wide">
          <div className="card__header">
            <div>
              <p className="eyebrow">История</p>
              <h3>Заказы аккаунта</h3>
            </div>
            <div className="ghost muted">{orders.length} шт.</div>
          </div>
          {orders.length === 0 ? (
            <p className="empty">Пока нет заказов. Создайте первый, чтобы увидеть статус.</p>
          ) : (
            <div className="orders">
              {orders.map((order) => (
                <div key={order.publicId} className="order-row">
                  <div>
                    <p className="order-id">{order.publicId}</p>
                    <p className="order-amount">{formatCurrency(order.amount)}</p>
                  </div>
                  <div className={`status status--${order.status.toLowerCase()}`}>
                    {formatStatus(order.status)}
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

function formatStatus(status: string) {
  switch (status.toLowerCase()) {
    case 'paid':
      return 'Оплачен';
    case 'rejected':
      return 'Отклонён';
    case 'new':
      return 'Создан';
    default:
      return status;
  }
}

function formatCurrency(amount: number) {
  return amount.toLocaleString('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    maximumFractionDigits: 2,
  });
}

export default App;
