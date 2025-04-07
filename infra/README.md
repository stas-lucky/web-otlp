## Infrastructure Setup

### Vagrant set up

```shell
wget -O - https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list
sudo apt update && sudo apt install vagrant
```

#### Run Vagrant

```shell
# Start the cluster
vagrant up

# Get the join command
vagrant ssh k8s-control -c "sudo microk8s add-node --token-ttl 3600"

# SSH into worker nodes and join them using the command
vagrant ssh k8s-worker-1
# Run the join command here
microk8s join 192.168.56.10:25000/1c6469c4efdaa9e7238256d24eaf0729/fe8090f8c941 --worker --skip-verify
exit

vagrant ssh k8s-worker-2
# Run the join command here
exit

# Verify cluster status from control plane
vagrant ssh k8s-control
sudo microk8s kubectl get nodes
```

```shell
# Set up kubectl locally in order to access the cluster
mkdir -p ~/.kube
vagrant ssh k8s-control -c "sudo microk8s config" > ~/.kube/config
sudo snap install kubectl --classic

# Replace IP address in the config file to 192.168.56.10, you control plane IP

```
