# Matching engine

## Crypto trading engine. Base code adapted from jpelgrims.

## WEBINAR NOTES
### GO TO THIS URL TO BUILD FRONT END
*http://ether.chainapp.live:8082/
### Launch CI/CD at this URL
*http://proxy.chainapp.live:8085/    
### CHANGE THIS VARIABLE IN THE Webinar_Files/main.js
*var MY_URL="1584941087";

### Usage

### when on localhost subdomain
```
curl -H Host:crypto.localhost -d '{"number":"30", "midpoint": "100"}' -H "Content-Type:application/json" -XPOST localhost/orders/random
```
### generate new buy order
*#curl -d '{"side":"buy", "price": "100", "quantity":"1000"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new

### generate new sell order
*#curl -d '{"side":"sell", "price": "102", "quantity":"100"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new

### get the orderbook
*#curl -X GET http://localhost:5000/orderbook

### generate random orders
*#curl -d '{"number":"30", "midpoint": "100"}' -H "Content-Type:application/json" -X POST http://localhost:5000/orders/random

### to test, start the python app, then run the script
### launch app
*#python api.py

### run script
*# ./scripts/test_engine.sh

