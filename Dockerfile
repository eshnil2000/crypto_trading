#Use this image to Run the code dockerized, standalone
# Command: docker build -t trading-engine .
FROM python:3.6
WORKDIR /usr/src/app

# Bundle app source
COPY . .
RUN pip install -r requirements.txt

EXPOSE 5000

CMD [ "env", "FLASK_APP=api.py", "flask", "run" ,"--host", "0.0.0.0", "--port", "5000"]