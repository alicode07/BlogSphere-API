# BlogSphere

A blogging REST API built with **ASP.NET Core (.NET 9)**, **Entity Framework Core**, and **MySQL**, using a clean layered architecture and JWT authentication.

Users can register, log in, publish blog posts, and comment on posts. Only the owner of a blog or comment can edit or delete it.

---

## Features

- User registration and login with **JWT** authentication
- Passwords hashed with **BCrypt**
- Full CRUD for blog posts, with author-only edit and delete
- Bulk blog creation (send an array of posts in one request)
- Comments on blog posts, with owner-only delete
- Input validation with DTOs and data annotations
- Interactive API docs with **Swagger UI** (with a bearer-token Authorize button)
- Layered architecture: API, BLL, DAL, Models

---

## Tech Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 9) |
| ORM | Entity Framework Core 9 |
| Database | MySQL (Pomelo provider) |
| Auth | JWT Bearer |
| Password hashing | BCrypt.Net-Next |
| API docs | Swashbuckle (Swagger UI) |

---

## Project Structure

```
BlogSphere/
│
├── BlogSphere.API/                    # Presentation layer
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── BlogController.cs
│   │   └── CommentController.cs
│   ├── DTOs/
│   │   ├── UserDto.cs
│   │   ├── LoginDto.cs
│   │   ├── BlogDto.cs
│   │   └── CommentDto.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── BlogSphere.BLL/                    # Business logic layer
│   ├── Interfaces/
│   └── Services/
│
├── BlogSphere.DAL/                    # Data access layer
│   ├── Context/
│   ├── Interfaces/
│   └── Repositories/
│
└── BlogSphere.Models/                 # Entity / domain layer
    ├── User.cs
    ├── Blog.cs
    └── Comment.cs
```

Dependencies flow one way: **API → BLL → DAL → Models**.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8](https://dev.mysql.com/downloads/) (running locally or in Docker)
- `dotnet-ef` tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/<your-username>/BlogSphere.git
cd BlogSphere
```

### 2. Configure the app

Edit `BlogSphere.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=BlogSphereDb;User=root;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "REPLACE_WITH_A_LONG_RANDOM_SECRET_AT_LEAST_32_CHARS",
    "Issuer": "BlogSphere",
    "Audience": "BlogSphereUsers",
    "ExpiryMinutes": 60
  }
}
```

> **Do not commit real secrets.** Use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables for your database password and `Jwt:Key`.

### 3. Restore, build, and create the database

```bash
dotnet restore
dotnet build

dotnet ef migrations add InitialCreate -p BlogSphere.DAL -s BlogSphere.API -o Migrations
dotnet ef database update -p BlogSphere.DAL -s BlogSphere.API
```

### 4. Run the API

```bash
dotnet run --project BlogSphere.API --urls "http://localhost:5253"
```

Open **http://localhost:5253/swagger** in your browser.

> On macOS, port 5000 is often used by AirPlay Receiver, so use another port such as 5253.

---

## Using the API

### Authenticate in Swagger

1. `POST /api/auth/register` to create an account.
2. `POST /api/auth/login` and copy the `token` value from the response.
3. Click **Authorize** (padlock, top right) and paste **only the token**. Do not add the word `Bearer`, and do not include quotes.
4. Protected endpoints now work. Tokens expire after 60 minutes, so log in again when needed.

### Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | No | Create an account |
| POST | `/api/auth/login` | No | Log in and receive a JWT |
| GET | `/api/blogs` | No | List all blogs (newest first) |
| GET | `/api/blogs/{id}` | No | Get one blog |
| POST | `/api/blogs` | Yes | Create a blog |
| POST | `/api/blogs/bulk` | Yes | Create multiple blogs at once |
| PUT | `/api/blogs/{id}` | Yes (author) | Update a blog |
| DELETE | `/api/blogs/{id}` | Yes (author) | Delete a blog |
| GET | `/api/blogs/{blogId}/comments` | No | List comments on a blog |
| POST | `/api/blogs/{blogId}/comments` | Yes | Add a comment |
| DELETE | `/api/comments/{id}` | Yes (owner) | Delete a comment |

### Example requests

**Register**
```json
POST /api/auth/register
{
  "username": "faisal",
  "email": "faisal@example.com",
  "password": "Secret123"
}
```

**Login**
```json
POST /api/auth/login
{
  "email": "faisal@example.com",
  "password": "Secret123"
}
```

**Create a blog**
```json
POST /api/blogs
{
  "title": "My First Post",
  "content": "Hello from BlogSphere!"
}
```

**Create blogs in bulk**
```json
POST /api/blogs/bulk
[
  { "title": "First post", "content": "Content of the first post" },
  { "title": "Second post", "content": "Content of the second post" }
]
```

**Add a comment**
```json
POST /api/blogs/1/comments
{
  "content": "Great article!"
}
```

---

## Data Model

```
User 1 ──── * Blog 1 ──── * Comment * ──── 1 User
```

- Deleting a blog deletes its comments.
- Deleting a user who has comments is restricted, to avoid multiple cascade paths.
- `Email` and `Username` are unique.

---

## Package Reference

| Project | Packages |
|---|---|
| **DAL** | `Pomelo.EntityFrameworkCore.MySql` 9.0.0 |
| **BLL** | `BCrypt.Net-Next` 4.0.3, `System.IdentityModel.Tokens.Jwt` 8.0.1, `Microsoft.Extensions.Configuration.Abstractions` 9.0.0 |
| **API** | `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.0, `Microsoft.EntityFrameworkCore.Design` 9.0.0, `Swashbuckle.AspNetCore` 10.2.3 |

---

## Troubleshooting

| Problem | Fix |
|---|---|
| `You must install or update .NET` | The project targets .NET 9. Install the .NET 9 runtime, or change `TargetFramework` in every `.csproj`. |
| Swagger page won't load | Check the terminal for a startup error (MySQL not running, wrong password, missing `Jwt:Key`). |
| `401 Unauthorized` on protected endpoints | Click **Authorize** and paste the full token (it must contain two dots). Log in again if it expired. |
| `IDX14120: JWT is not well formed` | The token was copied incompletely. Select the whole token, not just one segment. |
| `403 Forbidden` on PUT or DELETE | You are logged in as a different user than the blog or comment owner. |
| Swagger build errors after a package update | `Swashbuckle.AspNetCore` 10.x uses `using Microsoft.OpenApi;`. Version 6.x uses `using Microsoft.OpenApi.Models;`. |

---

## Roadmap

- Refresh tokens
- Pagination and search for blogs
- Categories and tags
- Unit and integration tests
- Docker Compose setup (API + MySQL)

---

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -m "Add my feature"`
4. Push the branch and open a Pull Request

---



## Author

**Mohd Faisal Ali**: [GitHub](https://github.com/alicode07)
