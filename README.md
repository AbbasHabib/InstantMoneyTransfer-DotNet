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

## Packages used
```bash
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.11
```


Benefits of DbContext Pooling:
Reduced Overhead: By reusing DbContext instances, you reduce the cost associated with creating and disposing of DbContext objects.
Improved Performance: Pooling can lead to better performance in high-traffic applications by reducing the time spent on DbContext initialization and disposal.
Efficient Resource Utilization: Pooling helps in efficient utilization of resources, especially in scenarios where DbContext instances are frequently created and disposed of.

## add the DB migration
```bash
    dotnet ef database update
```

## add databse connection string to you user-secrets
```bash
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=5432;Database=atlasbank;Username=postgres;Password=somepassword"
```