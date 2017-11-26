# Lab 0 - Getting started

This lab is going to let you prepare your development environment for the rest of the labs in the workshop. Not all steps are required. The more you can prepare, the better the experience during the workshop.

Goals for this lab: 
- [Prepare development laptop](#1)
- [Download required and optional tooling](#2)
- [Clone Git repository for lab code](#3)
- [Run and inspect lab application](#4)
- [Create Docker cluster on Microsoft Azure](#5)
 
## <a name="1"></a>1. Prepare your development laptop
Make sure that your laptop is up-to-date with the latest security patches. This workshop is specific towards Windows as the operating system for your machine. The labs can also be done on Linux, although this can be a bit more challenging.

## <a name="2"></a>2. Download required and optional tooling
First, you will need to have a development IDE installed. The most preferable IDE is [Visual Studio 2017](https://www.visualstudio.com/vs/) if you are running the Windows operating system.

You may want to consider installing [Visual Studio Code](https://code.visualstudio.com/) in the following cases:
- Your development machine is running OSX or a Linux distribution as your operating system.
- You want to have an light-weight IDE or use an alternative to Visual Studio 2017.

> Download and install [Visual Studio 2017 or Code](https://www.visualstudio.com/downloads/)

Second, you are going to need the Docker Community Edition tooling on your development machine. Depending on your operating system you need to choose the correct version of the tooling. Instructions for installing the tooling can be found [here](https://docs.docker.com/engine/installation/). You can choose either the stable or edge channel.

> Download and install Docker Community Edition:
> - [Docker for Windows](https://docs.docker.com/docker-for-windows/install/)
> - [Docker for Mac](https://docs.docker.com/docker-for-mac/install/)

The following optional tools are recommended, but not required.

- [GitHub Desktop](https://desktop.github.com/) for Git Shell and Git repository utilities
- [PuTTY](http://www.putty.org/) for `PuTTY.exe` and `PuTTYgen.exe`

## <a name="3"></a>3. Clone Git repository for lab code
The workshop uses an example to get you started with Dockerizing a typical ASP.NET Core application. 
The application is themed around high-scores of retro video games. It consists of web front end and a Web API and stores high-scores in a relational database.

Clone the repository to your development machine:
- Create a folder for the source code, e.g. `C:\Sources\SELA`.
- Open a command prompt from that folder
- Clone the Git repository for the workshop files:

```
// Git address will be made available soon!
git clone https://github.com/alexthissen/dockerworkshop2017.git
```
- Set an environment variable to the root of the cloned repository from PowerShell:
```
$env:workshop = 'C:\Sources\SELA\dockerworkshop2017
'
```

## <a name="4"></a>4. Compile and inspect demo application
Start Visual Studio and open the solution you cloned in the previous step. 
Build the application and fix any issues that might occur. 
Take a look at the solution and inspect the source code. In particular, pay attention to:
- Web API and the Entity Framework code to store data
- Web frontend with proxy code to call Web API

## <a name="5"></a>5. (Optional) Create a Docker Swarm Mode cluster in Azure

This part requires you have an active Azure subscription. If you do not, you can create a trial account at [Microsoft Azure](https://azure.microsoft.com/en-us/free/). It will require access to a credit card, even though it will not be charged.

Visit [https://docs.docker.com/docker-for-azure](https://docs.docker.com/docker-for-azure/#docker-community-edition-ce-for-azure) which contains instructions on how to create the cluster and its prerequisites. For now, just follow along and specify the details. You will understand better what you have created at a later time.

> ##### Important note
> Make sure that you choose a resource group name with lower-case characters.

The short version of provisioning a Docker Swarm cluster is to execute the following commands, replacing the `sdp2017` to something unique to you:
```
docker pull docker4x/create-sp-azure:latest
docker run -ti docker4x/create-sp-azure sp_sdp2017 sdp2017 WestEurope
```

Make sure you store the information given by running the `docker run` command to create the resource group with a service principal. It will look like this:
```
Your access credentials ==================================================
AD ServicePrincipal App ID:       65b6aa52-3e5a-4c87-b7cc-7d312340ea86
AD ServicePrincipal App Secret:   tuP3ltsmdjmD8ICuWS3dHq1Hd2214Bga
AD ServicePrincipal Tenant ID:    d2931234-3c29-4575-b1cd-1aaf1e228995
Resource Group Name:              sdp2017
Resource Group Location:          WestEurope
```

Create a public/private keypair in your working folder by running either `ssh-keygen.exe` from the Git Shell. Alternatively, download [Putty](http://www.putty.org/) and run the `PUTTYGEN.EXE` tool. Click the `Generate` button and move your mouse over the blank area above the button. Save both the public and private key files, as `dockerswarm_publickey.pub` and `dockerswarm_privatekey.ppk` respectively. Place these files under the `$env:workshop` folder.

On the [web page](https://docs.docker.com/docker-for-azure/#docker-community-edition-ce-for-azure), follow the link to either the Stable or Edge setup for a cluster. 
- [Stable channel](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fdownload.docker.com%2Fazure%2Fstable%2FDocker.tmpl): if you want to go for safe. 
- [Edge channel](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fdownload.docker.com%2Fazure%2Fedge%2FDocker.tmpl): when you are more of an adventurous type

You will be redirected to the [Azure Portal](https://portal.azure.com) and need to fill in the details to create a new Docker Swarm Mode cluster. 

Use the values from the AD Service Principal in the Azure portal blade for the Docker Swarm ARM template. Copy the contents of the public keyfile into the in the textbox called `SSH Public Key` for stable channel and `Linux SSH Public Key` (for edge channel). The value should resemble something like this:
```
ssh-rsa
  AAAAB3NzaC1yc2EAAAABJQAAAQEA5gCzKhk9mj2NyOVZqcAhCCLsqppbk3jkb8t6HXx6xWK
  iJuHuGNzWxdgr7rba/NyGpYT7Q9gV4XN2QQ14PgU9a0z8pIeOnEAmg8uVE27QlVOee6nL5U
  p/yXC3zTLNDtvnOrHN+WQp20pD9EYxS8I9VDwTJbNhzoulSX6db3eTx49mRL4Hce5LsTpgP
  jPp/4AjTo9L2q1qUDhhFyLsh3xdHRh56Esh30tyPb9DVWmOkU9LRHMO6LccEUphRkU+d/Us
  Jcxh2/hb8zmnwi5nZu3ScLmaEpKsepeoNWd/tF7yz7m3tFjH/61gn/q4/VDCN1Tud+S//q3
  79q8rjuNgicsfQ== rsa-key-20171119
```

Finally, click `Purchase` to generate the Docker Swarm Mode cluster. This does not incur any costs other than your Azure resource consumption and should be fit easily within your Azure trial.

## Wrapup
You have prepared your laptop and cloud environment to be ready for the next labs. Any issues you may have, can probably be resolved during the labs. Ask your fellow attendees or the proctor to help you, if you cannot solve the issues.

Continue with [Lab 1 - Docker101](Lab1-Docker101.md).