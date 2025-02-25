# 🚀 DeliveryService

DeliveryService er en microservice til håndtering af pakkeudlevering i et logistiksystem. Den modtager data fra en webshops ordre system via RabbitMQ og genererer en forsendelsesplan.

## 🏗 Teknologi
- .NET Core
- Docker & Docker Compose
- RabbitMQ
- CSV filhåndtering

## 📦 Funktionalitet
### 🔹 ShippingService
- **POST** `/api/shipping/request` - Modtager forsendelsesanmodninger
- **GET** `/api/shipping/deliveryplan` - Returnerer dagens forsendelsesplan
- Publicerer `ShipmentDelivery` beskeder til RabbitMQ

### 🔹 DeliveryService
- **Consumer** af `ShippingRequest` instanser
- **BackgroundService** der lytter på køen og skriver data til en CSV-fil i en Docker-volumen

## 🛠 Arkitektur
```plaintext
┌────────────────────┐        ┌──────────────────┐
│  ShippingService  │  →→→  │  RabbitMQ Queue  │
└────────────────────┘        └──────────────────┘
         ↓                              ↓
┌────────────────────┐        ┌────────────────────┐
│  DeliveryService  │  →→→  │  CSV Output File  │
└────────────────────┘        └────────────────────┘
```

