FROM node:22-alpine AS build
WORKDIR /app

COPY package.json ./
RUN npm install

COPY . ./

ARG ANGULAR_API_URL=/api
ARG ANGULAR_APP_TITLE=Invoice Generator
ENV ANGULAR_API_URL=$ANGULAR_API_URL
ENV ANGULAR_APP_TITLE=$ANGULAR_APP_TITLE

RUN npm run build

FROM nginx:alpine
COPY nginx-frontend.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 80
