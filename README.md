# 🚀 FOSDEM 2025: Serverless Workflow Demo

[![Serverless Workflow](https://img.shields.io/badge/Serverless__Workflow-blue)](https://serverlessworkflow.io)
[![FOSDEM 2025](https://img.shields.io/badge/Event-FOSDEM%202025-red)](https://fosdem.org/2025/)
[![License](https://img.shields.io/badge/License-APACHE__2.0-green)](LICENSE)

## 🌟 Overview
This repository contains a **real-time event-driven workflow demo** built with [Serverless Workflow](https://serverlessworkflow.io), designed for [FOSDEM 2025](https://fosdem.org/2025/).

🔹 **Built with OpenAPI & AsyncAPI** for synchronous & asynchronous service interactions.  
🔹 **Showcases event-driven workflows** using event streaming & complex event correlation.  
🔹 **Scalable microservices** running in a **containerized** environment.  

## 📺 Live Demo
This demo showcases a **dynamic event-driven workflow** for the **Galactic Bounty Network (GBN)**, where bounty hunters **connect to the network, track targets, and engage in contracts**.  

The system orchestrates **multiple asynchronous services**, **real-time event streaming**, and **complex event correlation**, creating a fully **serverless bounty-hunting experience**.

📌 **Demo Date:** [FOSDEM 2025](https://fosdem.org/2025/) (February 2, 2025)  
📌 **Location:** Brussels, Belgium  
📌 **Presented by:** [Charles d'Avernas](https://github.com/cdavernas), [Jean-Baptiste Bianchi](https://github.com/jbbianchi), [Ricardo Zanini](https://github.com/ricardozanini)

## 🔧 Tech Stack
- 🛠 **Serverless Workflow Specification**
- 🔄 **OpenAPI** & **AsyncAPI** for API interaction
- 📡 **Event Streaming**
- 🏗 **Containerized Microservices**
- 🌐 **Real-Time Event Handling**

## 📜 Workflow Description
The **Galactic Bounty Network Workflow** consists of the following steps:

1. **Hunter Connects to the GBN** → The system detects a new bounty hunter connection (Cloud Event).  
2. **Target Selection** → The hunter browses available contracts (OpenAPI call).  
3. **Bounty Tracking Activated** → The hunter subscribes to real-time target movement (AsyncAPI event streaming).  
4. **Contract Engagement** → The hunter locks in a contract, triggering event-driven processing.  
5. **Event Correlation** → The system listens for **target status updates** & **payment verification** before finalizing.  
6. **Mission Completion** → The hunter claims the reward, completing the workflow.  


## 🚀 Getting Started
### 🖥️ Prerequisites
- [Docker](https://www.docker.com/)

### ⚡ Run the Demo
```bash
git clone https://github.com/your-org/fosdem-2025-sw-demo.git
cd fosdem-2025-sw-demo
docker-compose up
