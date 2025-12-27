**ФИО: Кравченко Игорь Александрович.  
БПИ: 249**

<br>

---

# Gozon Store — микросервисы (.NET 10 + React)

## Обзор
Интернет-магазин из нескольких сервисов: Accounts (профиль), Payments (баланс и списания), Orders (заказы), API Gateway (YARP) и фронтенд на React. Взаимодействие сервисов асинхронное через RabbitMQ; данные — SQLite в Docker volumes. Сборка и запуск — Docker Compose.

## Технологии и зависимости
- .NET 10, ASP.NET Core, Entity Framework Core 10.0.1, Swashbuckle (Swagger)
- YARP Reverse Proxy 2.3.0
- RabbitMQ (queues `orders.payment.requested`, `orders.payment.result`), паттерны Outbox/Inbox
- SQLite
- React 19 + Vite + TypeScript, Nginx для прод-статик
- Docker/Docker Compose

## Сервисы
- Accounts (`src/Accounts_Service`): создаёт и хранит аккаунты с профилем/статусом.
- Payments (`src/Payments_Service`): платёжные аккаунты и баланс, приём запросов на списание, публикация результатов.
- Orders (`src/Orders_Service`): создание заказов, запрос списания в RabbitMQ, обновление статуса по результату.
- Gateway (`src/GateWay`): YARP, маршруты `/accounts/**` → Accounts, `/orders/**` → Orders, `/payments/**` → Payments (префикс удаляется), CORS для фронта.
- Frontend (`src/Frontend/app`): React UI; `src/Frontend/Frontend.Host` — Dockerfile + nginx.conf.

## Эндпоинты (через Gateway http://localhost:8080)
### Accounts
- `POST /accounts` — создать аккаунт, body `{ firstName, lastName, description }`, 201 c `{ accountNumber }`.
- `GET /accounts/{accountNumber}` — получить аккаунт.
- `PUT /accounts/{accountNumber}/profile` — обновить описание, body `{ description }`.
- `POST /accounts/{accountNumber}/block` — блокировать.
- `POST /accounts/{accountNumber}/activate` — активировать.

### Payments
- `POST /payments/payment-accounts` — создать/идемпотентно вернуть платёжный аккаунт, body `{ accountNumber }`.
- `POST /payments/payment-accounts/topup` — пополнить баланс, body `{ accountNumber, amount }`.
- `GET /payments/payment-accounts/{accountNumber}/balance` — получить баланс.

### Orders
- `POST /orders` — создать заказ, body `{ accountNumber, amount, description }`, 201 c `{ publicId, status }`.
- `GET /orders?accountNumber=...` — список заказов по аккаунту.
- `GET /orders/{publicId}` — детали заказа.

## Очереди и надёжность
- RabbitMQ очереди: `orders.payment.requested` (Orders → Payments), `orders.payment.result` (Payments → Orders).
- Outbox/Inbox в Payments и Orders: сообщения сохраняются транзакционно и доставляются/обрабатываются идемпотентно.

## Данные
- SQLite базы в volumes: `accounts_data`, `orders_data`, `payments_data`.

## Сборка и запуск (Docker)
Требования: Docker 20+, Docker Compose, свободные порты 8080–8083, 5672, 15672.
```bash
docker compose up --build
```
Сервисы:
- Gateway: http://localhost:8080
- Frontend: http://localhost:8083
- Orders Swagger: http://localhost:8081/swagger
- Payments Swagger: http://localhost:8082/swagger
- RabbitMQ UI: http://localhost:15672 (guest/guest)

## Полезные команды
- Сборка фронта локально: `npm run build` в `src/Frontend/app`.
- Пересборка фронт-контейнера: `docker compose up --build frontend`.
