def UU_ID

node {
  UU_ID = sh(script: "date +%s",returnStdout: true).trim()
}

pipeline {
    agent none
    
    stages {
   
        
        stage('Deploy') { 
            agent {
                docker {
                    
                    image 'python:3.6-alpine' 
                    args '-p 5000 --network nginx-proxy --expose 5000 -e VIRTUAL_HOST='+UU_ID+'.proxy.chainapp.live -e VIRTUAL_PORT=5000  '
                }
            }
            steps {
                sh 'echo "${IMAGE}"'
                sh 'pip install -r requirements.txt'
                sh 'chmod +x ./scripts/deliver.sh'
                sh './scripts/deliver.sh'
                input message: 'Finished using the web site? (Click "Proceed" to continue)'
                sh 'chmod +x ./scripts/kill.sh'
                sh './scripts/kill.sh'
            }
        }        
    }
}