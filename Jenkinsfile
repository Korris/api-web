pipeline {
    agent { label 'agent01u' }

    environment {
        HARBOR_REGISTRY = "http://harbor.local/v2/"
        GITOPS_REPO = "https://repo.ntadamedia.com/focfoc/argo.git"
    }

    stages {
        stage('Setup Environment & Variables') {
            steps {
                script {
                    // Get branch name
                    def BRANCH = env.GIT_BRANCH.tokenize('/').last()

                    // Detect environment
                    def ENV
                    switch (BRANCH) {
                        case 'dev-k8s':
                            ENV = 'dev'
                            break
                        case 'uat-k8s':
                            ENV = 'uat'
                            break
                        case 'pro-k8s':
                            ENV = 'pro'
                            break
                        default:
                            error "❌ Unknown branch ${BRANCH}. Expect dev-k8s, uat-k8s, or pro-k8s."
                    }

                    // Extract service name (comic) from JOB_NAME like Dev-Web-Focfoc.Comic.Api
                    def job = env.JOB_NAME.toLowerCase()
                    def match = job =~ /web-focfoc\.([a-z]+)\.api$/
                    if (!match) {
                        error "❌ Cannot extract service suffix from JOB_NAME: ${env.JOB_NAME}"
                    }

                    def SERVICE_SUFFIX = match[0][1] // e.g. 'comic'
                    def SERVICE = "api-web-${SERVICE_SUFFIX}"

                    def IMAGE_PREFIX = "harbor.local/focfoc/${SERVICE}"
                    def IMAGE_NAME = "${IMAGE_PREFIX}_${ENV}"
                    def DOCKER_IMAGE = "${IMAGE_NAME}:${BUILD_NUMBER}"

                    // Export to env for later stages
                    env.BRANCH = BRANCH
                    env.ENV = ENV
                    env.SERVICE = SERVICE
                    env.SERVICE_SUFFIX = SERVICE_SUFFIX
                    env.IMAGE_PREFIX = IMAGE_PREFIX
                    env.IMAGE_NAME = IMAGE_NAME
                    env.DOCKER_IMAGE = DOCKER_IMAGE

                    echo """
                    =============================
                    Job Name     : ${env.JOB_NAME}
                    Branch       : ${env.BRANCH}
                    Environment  : ${env.ENV}
                    SERVICE      : ${env.SERVICE}
                    Docker Image : ${env.DOCKER_IMAGE}
                    =============================
                    """
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    def dockerImage = docker.build(
                        env.DOCKER_IMAGE,
                        "-f Mcsg.${env.SERVICE_SUFFIX.capitalize()}.Api/Dockerfile ."
                    )
                    env.DOCKER_IMAGE_ID = dockerImage.id
                }
            }
        }

        stage('Push to Harbor') {
            steps {
                script {
                    docker.withRegistry(env.HARBOR_REGISTRY, 'hub_harbor') {
                        def img = docker.image(env.DOCKER_IMAGE)
                        img.push()
                        img.push('latest')
                    }
                }
            }
        }

        stage('Update GitOps') {
            steps {
                dir('gitops') {
                    deleteDir()
                    withCredentials([usernamePassword(credentialsId: 'git_toannguyen', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_PASS')]) {
                        sh """
                            echo ">>> Clone GitOps Repo"
                            git clone -b ops https://${GIT_USER}:${GIT_PASS}@repo.ntadamedia.com/focfoc/argo.git .

                            echo ">>> Update kustomization.yaml"
                            yq e '.images[] |= select(.name == "harbor.local/placeholder").newTag = "${BUILD_NUMBER}"' -i apps/${ENV}/${SERVICE}/kustomization.yaml

                            echo ">>> Git Commit"
                            git config user.email "jenkins@ci.com"
                            git config user.name "Jenkins CI"
                            git add apps/${ENV}/${SERVICE}/kustomization.yaml
                            git commit -m "Update ${SERVICE} image to ${DOCKER_IMAGE}" || echo "No changes"
                            git push origin ops
                        """
                    }
                }
            }
        }
    }

    post {
        success {
            echo "✅ Build and deploy successful: ${env.DOCKER_IMAGE}"
        }
        failure {
            echo "❌ Pipeline failed for ${env.SERVICE} on branch ${env.BRANCH}!"
        }
    }
}
