#!/usr/bin/env sh
number=500
midpoint=100
string='{"number":"'${number}'", "midpoint":"'${midpoint}'"}';echo $string
curl -d "$string" -H "Content-Type:application/json" -X POST http://localhost:5000/orders/random

sleep 3

for i in {1..10}
do
	midpoint=100; echo $r
	number=$(( $RANDOM % 5 )); echo $number
	
	string='{"number":"'${number}'", "midpoint":"'${midpoint}'"}';echo $string
	curl -d "$string" -H "Content-Type:application/json" -X POST http://localhost:5000/orders/random
	sleep 3
done
