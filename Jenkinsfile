	pipeline {
  		agent any
    		stages {
      			stage('gitea clone') {
        			steps {
          				echo "Cloning Repository" 
						git branch: 'main',
							credentialsId: 'access-gitea',
							url: 'http://localhost:3333/CHS/Korean_Wordle.git'
        			}
      			}
    		}
	}