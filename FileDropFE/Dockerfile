FROM node:latest as build

WORKDIR /usr/local/app

COPY ./ /usr/local/app/

RUN npm install

RUN npm run build

FROM nginx:latest

COPY --from=build /usr/local/app/dist/FileDropFE /usr/share/nginx/html

COPY /Nginx/nginx.conf /etc/nginx/

EXPOSE 80