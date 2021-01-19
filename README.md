# Crypto Order Matching engine

## Traefik reverse proxy ready, Containerized Crypto trading engine in Python with frontend GUI in HTML/Javascript.

### Usage

### when on localhost subdomain
```
curl -H Host:crypto.localhost -d '{"number":"30", "midpoint": "100"}' -H "Content-Type:application/json" -XPOST localhost/orders/random
```
### generate new buy order (Standalone mode)
*#curl -d '{"side":"buy", "price": "100", "quantity":"1000"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new

### generate new buy order (Kafka mode)
*#curl -d '{"side":"buy", "price": "100", "quantity":"1000"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new-kafka

### generate new sell order
*#curl -d '{"side":"sell", "price": "102", "quantity":"100"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new

### get the orderbook
*#curl -X GET http://localhost:5000/orderbook

### generate random orders (Standalone mode only)
*#curl -d '{"number":"30", "midpoint": "100"}' -H "Content-Type:application/json" -X POST http://localhost:5000/orders/random


