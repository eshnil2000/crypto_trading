#!/usr/bin/env sh

set -x
python api.py &
sleep 1
echo $! > .pidfile
set +x