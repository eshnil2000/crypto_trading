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


##################### DETAILED SETUP ######################
```

#Setup /etc/hosts to route to localhost subdomains (Firefox, Safari browsers)
#Note the "\t" is a tab
sudo echo -e "127.0.0.01\tcrypto.localhost" >>/etc/hosts
sudo echo -e "127.0.0.01\twhoami.localhost" >>/etc/hosts
sudo echo -e "127.0.0.01\tkafdrop.localhost" >>/etc/hosts

#If this doesn't work, manually edit /etc/hosts and add following lines:
sudo nano /etc/hosts

###Traefik
127.0.0.01      whoami.localhost
127.0.0.01      kafdrop.localhost
127.0.0.01      crypto.localhost

#Base Docker setup
docker swarm init
docker network create -d overlay --attachable traefik_default

#Pull requierd docker images
docker pull eshnil2000/crypto-trading
docker pull confluentinc/cp-zookeeper:latest
docker pull confluentinc/cp-kafka:latest
docker pull obsidiandynamics/kafdrop
docker pull traefik:v2.3
docker pull eshnil2000/generator
docker pull eshnil2000/detector


#Setup Traefik
docker stack deploy -c docker-compose.traefik-base.yml traefik

#check stack 
docker stack ls

#check traefik dashboard: Browser
http://localhost:8080
http://whoami.localhost

#Setup Kafka
docker stack deploy -c docker-compose.traefik.kafka.yml kafka

#check stack 
docker stack ls

#check kafka dashboard: Browser
http://kafdrop.localhost

#Setup Crypto trading engine
docker build -t crypto-trading .
docker stack deploy -c docker-compose.yml crypto
docker stack rm crypto

#Setup the transaction generator/detector
docker build -t detector .
docker build -t generator .
docker stack deploy -c docker-compose.yml detgen

#check kafka dashboard: Browser http://kafdrop.localhost

#Stop the transaction generator, so you can use the UI to generate transactions
docker service ls
docker service rm detgen_generator

#Setup Crypto trading engine with basic auth
docker stack deploy -c docker-compose.basic-auth.yml crypto

#Check you can login at http://crypto.localhost with "dappsuni", "dappsuni"
docker stack rm crypto

#Setup Crypto trading engine with parametrized environment variables support
env $(cat .env | grep ^[A-Z] | xargs) docker stack deploy -c docker-compose.basic-auth.env.yml crypto

docker stack rm crypto
```
##################### DETAILED SETUP ######################
