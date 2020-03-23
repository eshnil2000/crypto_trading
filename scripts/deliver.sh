#!/usr/bin/env sh

set -x
env FLASK_APP=main.py flask run --host 0.0.0.0
sleep 1
echo $! > .pidfile
set +x
