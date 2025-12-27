FROM node:20-alpine AS build
WORKDIR /app

COPY src/Frontend/app/package*.json ./
RUN npm ci

COPY src/Frontend/app ./

# Встраиваем URL гейтвея на этапе сборки (для локального браузера)
ARG VITE_GATEWAY_URL=http://localhost:8080
ENV VITE_GATEWAY_URL=${VITE_GATEWAY_URL}
RUN npm run build

FROM nginx:1.27-alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY src/Frontend/Frontend.Host/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist ./

EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
