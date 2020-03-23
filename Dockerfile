#image @ eshnil2000/python-numpy
FROM python:3.6
WORKDIR /usr/src/app

# Bundle app source
COPY . .
RUN pip install -r requirements.txt