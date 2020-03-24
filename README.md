# Matching engine

An order matching engine. This repo contains a prototype written in python and an improved version written in C#. The python version is already finished, the C# version is still a work in progress.

### adapted from
https://jellepelgrims.com/posts/matching_engines#limit-order-books

## WEBINAR NOTES
### GO TO THIS URL TO BUILD FRONT END
*http://ether.chainapp.live:8082/
### Launch CI/CD at this URL
*http://proxy.chainapp.live:8085/    
### CHANGE THIS VARIABLE IN THE Webinar_Files/main.js
*var MY_URL="1584941087";

### Usage
*#curl -d '{"side":"buy", "price": "100", "quantity":"1000"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new
*#curl -d '{"side":"sell", "price": "102", "quantity":"100"}' -H "Content-Type: application/json" -X POST http://localhost:5000/order/new
*#curl -X GET http://localhost:5000/orderbook
*#curl -d '{"number":"30", "midpoint": "100"}' -H "Content-Type:application/json" -X POST http://localhost:5000/orders/random
