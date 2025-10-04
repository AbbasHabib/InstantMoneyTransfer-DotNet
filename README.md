# 💸 Instant Transfer API (.NET 8 + PostgreSQL)

A reliable and safe **Instant Transfer Service** for fintech systems.  
Built with **.NET 8**, **PostgreSQL**, designed to handle high concurrency, duplicate requests, and system reliability challenges.

---

## 🚀 Features

- Create accounts with initial balances.  
- Perform atomic money transfers between accounts.  
- Prevent negative balances.  
- Ensure **idempotency** — no duplicated or lost transfers.  
- Handle concurrent transfers safely.  
- Fully containerized with **Docker** and ready for **Kubernetes**.

---

## 🧠 Core Challenges & Solutions

### 1. **Concurrent Transfers**
**Problem:** Multiple transfers might target the same account at the same time, risking race conditions.  


---

### 2. **Duplicate Requests (Idempotency)**
**Problem:** A merchant might retry a transfer due to a network hiccup or timeout, causing double-debits.  

---

### 3. **Network Hiccups & Partial Failures**
**Problem:** The network might fail mid-transfer, leaving the system in an unknown state.  


---

### 4. **System Overload**
**Problem:** Merchants might push too many transfer requests in a short time.  


---

## 🧩 API Endpoints


---

## 🐳 Run with Docker
