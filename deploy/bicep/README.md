# Infrastructure deployment of Sicar
## Resources to be deployed
- aks
- acr - should be in a SEPARATE resource group and we probably just need one. There's an argument for a dedicated acr for prod
- cosmos db (mongo api) - account, database and collections
- azure service bus - namespace, queues, topics
- vnet?
- key vault for dapr secrets
- 
## Pipeline for Deployment
An Azure DevOps Pipeline will be used to run the main, (non-application) deployment resources

# Notes
Dapr via Helm
Dapr HA
API Gateway - APIM (self-hosted) - better than nginx, gloo, traefik, envoy
cerificate?
checkout eShopOnDapr
.env (environment variables picked up by docker compose - so local only)
k8s deployment/service/configMap are in a single file per service. Helm would be good for full siccar install
/dapr (local?)
	/components
	/configuration

/deploy/k8s
	/components
	service yamls
	dapr yamls

/docs


# For Production
Dapr HA
Prod Cert
vNet for AKS et al - following Microsoft AKS Baseline Architecture
AKS security best practices
dedicated ACR?



# Siccar app from Helm charts for all microservices
k8s dapr yaml files - potentially a db and messaging for each service

# Order of deployment
1. aks
2. acr
3. role assignment of pull images on acr
4. dapr on aks
5. ingress on aks - use aks addon for now 
6. cert on aks as secret
7.  

# Azure Resources required
- AKS - parameterised
- ACR 
- Role Assignment for ACRto allow pull image from AKS 
- Service bus
- Cosmos DB (mongo api), Account, DB and Collection
- Cosmos DB (sql api), Account, DB and 4 named collections
- Key Vault + secrets (to store dapr secrets such as connection string for service bus)
- Role assignment to KV for aks msi
- 



# Infra settings required for yaml files
databases & collections
service bus settings
aks settings e.g. name, dns host from add-on
acr server