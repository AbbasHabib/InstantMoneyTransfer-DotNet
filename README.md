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

## Getting Started
1- Start the Databse Server using docker
Notice Postgresql will the latest OutputScript.sql from InstantTransfers and migrate it to the DB
```bash
cd postgresql
sudo docker compose up -d
```
2- Run the Service with Docker
```bash
## inside repo top dir
sudo docker build -t atlasbank_instant-transfer-service:1.0 .
sudo docker run -p 5149:8080 atlasbank_instant-transfer-service:1.0
```
Service would run now u can Access : http://localhost:5149/swagger/index.html
To test different cases 


## 🧠 Core Challenges & Solutions

### 1. **Concurrent Transfers**
**Problem:** Multiple transfers might target the same account at the same time, risking race conditions.  
**Solution:**  
1 - **row-level locking** is User via `SELECT ... FOR UPDATE` on the involved accounts to ensure only one **transaction** modifies the balances at a time.  
2 - Updates to account balances are done using **atomic SQL statements** (`UPDATE ... SET Balance = Balance ± X WHERE Balance >= X`)
instead of value retrieval from code
ex: 
```C#
    var acc1 = await _context.Accounts.FindAsync(id1);
    var acc2 = await _context.Accounts.FindAsync(id2);
    // some other workers could go and change the Balance values in one or both accounts 
    acc1.Balance = acc.Balance - 50;
    acc2.Balance = acc.Balance + 50;
    // then persist in the DB
```
in this case if row-level locking is not used Balance could go in undetrmined state. So both 1 and 2 are used to prevent concurrency issues.

3 - Accounts are locked in a **consistent order** (sorted by ID) to avoid deadlocks.


---

### 2. **Duplicate Requests (Idempotency)**
**Problem:** A merchant might retry a transfer due to a network hiccup or timeout, causing double-debits.  
**Solution:**  
- Simplest solution has been used, we rely on the database integrity on that one.
- A unique Index is created on the attributes received in the client request.
```SQL
CREATE UNIQUE INDEX "IX_Transactions_FromAccountId_ToAccountId_Timestamp" ON public."Transactions" USING btree ("FromAccountId", "ToAccountId", "Timestamp");
``` 
the previous composite unique identifier is create in the .net app at OnModelCreating
```C#
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite unique index on FromAccountId + ToAccountId + Timestamp for idempotency
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => new { t.FromAccountId, t.ToAccountId, t.Timestamp })
            .IsUnique();
    }
```
- The Databse during the transaction, by definition it refuses the reuqests which have the UNIQUE composite Index.
- Duplicate requests are **ignored**, returning BADREQUEST, ensuring **idempotency**.


---

### 3. **Network Hiccups & Partial Failures**
**Problem:** The network might fail mid-transfer, leaving the system in an unknown state.  
Both Point 1 and 2 fix these issues.

### 4. **System Overload**
Suggestion is using database connection pools and configure the number of pools on the system.

---

## 🧩 API Endpoints

### Account Endpoints
| Method | Endpoint            | Description            |
| ------ | ------------------- | ---------------------- |
| GET    | `/api/account`      | Retrieve all accounts  |
| POST   | `/api/account`      | Create a new account   |
| GET    | `/api/account/{id}` | Retrieve account by ID |
| PUT    | `/api/account/{id}` | Update account by ID   |
| DELETE | `/api/account/{id}` | Delete account by ID   |
### Transaction Endpoints
| Method | Endpoint                | Description                |
| ------ | ----------------------- | -------------------------- |
| GET    | `/api/transaction`      | Retrieve all transactions  |
| POST   | `/api/transaction`      | Create a new transaction   |
| GET    | `/api/transaction/{id}` | Retrieve transaction by ID |
| DELETE | `/api/transaction/{id}` | Delete transaction by ID   |
### Auth Endpoints [Work In Progress ⏰]
| Method | Endpoint             | Description             |
| ------ | -------------------- | ----------------------- |
| POST   | `/api/auth/register` | Register a new user     |
| POST   | `/api/auth/login`    | Log in an existing user |


---

## 🐳 Run with Docker

## Packages used
```bash
    
```



## add the DB migration
```bash
    dotnet ef database update
```

## add databse connection string to you user-secrets
```bash
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=5432;Database=atlasbank;Username=postgres;Password=somepassword"
```


## Run Tests
```bash
# inside repo top dir
dotnet build
dotnet test InstantTransfers.Tests
```
