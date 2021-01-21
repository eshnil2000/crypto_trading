# Crypto Order Matching engine

## Traefik reverse proxy ready, Containerized Crypto trading engine in Python with frontend GUI in HTML/Javascript & Kafka scale integration.

### Usage
* pre-requisites: Docker installed
* On localhost, modify /etc/hosts : echo -e "127.0.0.01\tcrypto.localhost" >>/etc/hosts

* docker network create -d overlay --attachable traefik_default
* docker build -t crypto-trading .
* docker swarm init
* docker stack deploy -c docker-compose.yml crypto
* To use kafka integration, kafka cluster needs to be up and running

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


