# Manifest
#
#
# Test only - will be run by Az Pipeline
#Requires -Version 7.0
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)] [string] $NAMESPACE='siccar',
    [Parameter(Mandatory=$false)] [string] $INGRESS_HOST='dc4931334e134cb6add7.uksouth.aksapp.io'
)

az aks get-credentials

# Deploy Dapr
# initialize Dapr
dapr init -k
# # alternative via Helm 3
# helm repo add dapr https://dapr.github.io/helm-charts/
# helm repo update
# kubectl create namespace dapr-system
# helm install dapr dapr/dapr --namespace dapr-system

# # verify Dapr in AKS
# kubectl get pods -n dapr-system -w
# kubectl get services -n dapr-system -w
# dapr dashboard -k



#Create the namespace
kubectl Create namespace $NAMESPACE
#check this worked ok
#kubectl config set-context --current --namespace=$NAMESPACE
#kubectl config view --minify

# Deploy secrets
kubectl delete secret servicebus-secret --namespace $NAMESPACE
kubectl create secret generic servicebus-secret --from-literal=connectionString='Endpoint=sb://siccar-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OttEtwp+ebn1phuEIhfe30izXl6CN9C6P1mRadiPKGE=' --namespace $NAMESPACE
kubectl delete secret cosmosdb-secret --namespace $NAMESPACE
kubectl create secret generic cosmosdb-secret --from-literal=masterKey='OQZM3fwFxbvHn0uh3HlcQoIDLxS4XIR4o8Jqxez5RTaxuaMKNWUtsh8hxY8mxSbXigbVXrBQrBlEE4U8A9i9Xw==' --from-literal=url='https://siccar-dev.documents.azure.com:443/' --namespace $NAMESPACE
# 
kubectl apply --file dapr-app-token-secret.yaml --namespace $NAMESPACE

# Deploy Dapr components
kubectl apply -f .\k8s\component-pubsub.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\component-state-blueprint.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\component-state-tenant.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\component-state-validator.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\component-state-wallet.yaml --namespace $NAMESPACE

# uncomment and hardcode image version
# # Deploy the Dapr application
# kubectl apply -f .\k8s\deployment-microservice-action.yaml
# kubectl apply -f .\k8s\deployment-microservice-blueprint.yaml
# kubectl apply -f .\k8s\deployment-microservice-peer.yaml
# kubectl apply -f .\k8s\deployment-microservice-register.yaml
# kubectl apply -f .\k8s\deployment-microservice-tenant.yaml
# kubectl apply -f .\k8s\deployment-microservice-validator.yaml
# kubectl apply -f .\k8s\deployment-microservice-wallet.yaml

# # Deploy the services
# # Note: you don't need these if you 'daperize' nginx (but I haven't)
# kubectl apply -f .\k8s\service-microservice-action.yaml
# kubectl apply -f .\k8s\service-microservice-blueprint.yaml
# kubectl apply -f .\k8s\service-microservice-peer.yaml
# kubectl apply -f .\k8s\service-microservice-register.yaml
# kubectl apply -f .\k8s\service-microservice-tenant.yaml
# kubectl apply -f .\k8s\service-microservice-validator.yaml
# kubectl apply -f .\k8s\service-microservice-wallet.yaml

# Deploy the ingress
kubectl apply -f .\k8s\ingress-microservice-action.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\ingress-microservice-blueprint.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\ingress-microservice-peer.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\ingress-microservice-register.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\ingress-microservice-tenant.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\ingress-microservice-validator.yaml --namespace $NAMESPACE
kubectl apply -f .\k8s\ingress-microservice-wallet.yaml --namespace $NAMESPACE

#
kubectl get all  --namespace $NAMESPACE