# FeintFramework User Guide

**A Django-inspired Web Framework for .NET**

This guide is for developers who want to build web applications using FeintFramework. If you're familiar with Django, you'll feel right at home.

---

## Table of Contents

- [Introduction](#introduction)
- [Quick Start](#quick-start)
- [Installation](#installation)
- [Project Structure](#project-structure)
- [Settings](#settings)
- [Routing](#routing)
- [Views](#views)
- [Templates](#templates)
- [Models](#models)
- [Migrations](#migrations)
- [Forms](#forms)
- [Admin Panel](#admin-panel)
- [Authentication](#authentication)
- [Sessions](#sessions)
- [Example Project Walkthrough](#example-project-walkthrough)
- [FAQ](#faq)

---

## Introduction

FeintFramework is a web framework for .NET that implements the **Model-View-Template (MVT)** pattern, inspired by Django. It provides:

- **Routing** with typed URL parameters
- **Template Engine** with Django-like syntax
- **ORM** with migrations
- **Forms** with automatic validation
- **Admin Panel** auto-generated from models
- **Authentication & Sessions** built-in

### Requirements

- .NET 9.0 or later
- C# 13

---

## Quick Start

Create a working web application in 5 minutes:

### 1. Create a new project

```bash
dotnet new console -n MyApp
cd MyApp
```

### 2. Add FeintFramework packages

```bash
dotnet add package FeintFramework
dotnet add package FeintFramework.Db.Sqlite
dotnet add package FeintFramework.Contrib
```

### 3. Create Settings

Create `Core/Settings.cs`:

```csharp
using FeintFramework.Config.Settings;
using FeintFramework.Routing;
using FeintFramework.Db.Sqlite.Migrator;

namespace MyApp.Core;

public class Settings : BaseSettings
{
    public override bool Debug => true;

    public override Type[] InstalledApps => [
        typeof(Blog.BlogApp)
    ];

    public override RootUrlPatterns RootUrlPatterns => new MainUrlPatterns();

    public override DatabaseHandler DatabaseHandler =>
        new SqliteDatabaseHandler(DatabaseConnectionString);
}
```

### 4. Create an Application

Create `Blog/App.cs`:

```csharp
using FeintFramework.Apps;

namespace MyApp.Blog;

public class BlogApp : BaseApplication
{
}
```

### 5. Create a Model

Create `Blog/Models/Post.cs`:

```csharp
using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace MyApp.Blog.Models;

[Table(Name = "blog_post")]
public partial class Post : IntModel
{
    [Column, CharField(Length = 255)]
    public string Title { get; set; }

    [Column, TextField]
    public string Content { get; set; }
}
```

### 6. Create URL Patterns

Create `Urls.cs`:

```csharp
using FeintFramework.Routing;
using FeintFramework.Http;

namespace MyApp;

class MainUrlPatterns : RootUrlPatterns
{
    public override List<UrlPattern> Urls => [
        new UrlPattern("/", HomeView, "home"),
        new Path("/post/<int:id>", PostDetailView, "post_detail")
    ];

    FeintHttpResponse HomeView(FeintHttpRequest request)
    {
        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = "<h1>Welcome to MyApp!</h1>",
            ContentType = "text/html"
        };
    }

    FeintHttpResponse PostDetailView(FeintHttpRequest request)
    {
        var postId = int.Parse(request.PathParams["id"]);
        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = $"<h1>Post {postId}</h1>",
            ContentType = "text/html"
        };
    }
}
```

### 7. Create Entry Point

Update `Program.cs`:

```csharp
using FeintFramework.Config;

Configurator.Settings = new MyApp.Core.Settings();
Configurator.Migrate();
Configurator.Configure(args);
```

### 8. Generate Migrations and Run

```bash
# Generate migrations
dotnet run --project path/to/FeintFramework.Db.MigrationGenerator -- \
    --settings "MyApp.Core.Settings, MyApp" \
    --project "$(pwd)"

# Run the application
dotnet run
```

Your app is now running at `http://localhost:9000`!

---

## Installation

### NuGet Packages

| Package | Description |
|---------|-------------|
| `FeintFramework` | Core framework |
| `FeintFramework.Db.Sqlite` | SQLite database provider |
| `FeintFramework.Contrib` | Admin, Auth, Sessions |

### Add to your project

```bash
dotnet add package FeintFramework
dotnet add package FeintFramework.Db.Sqlite
dotnet add package FeintFramework.Contrib
```

---

## Project Structure

Recommended project layout:

```
MyApp/
├── Core/
│   └── Settings.cs           # Application settings
├── Blog/                     # Your application module
│   ├── App.cs               # Application registration
│   ├── Models/
│   │   └── Post.cs          # Database models
│   ├── Migrations/          # Auto-generated migrations
│   ├── templates/           # HTML templates
│   └── Views.cs             # View handlers
├── Urls.cs                  # Root URL configuration
├── Program.cs               # Entry point
└── MyApp.csproj
```

---

## Settings

Settings configure your entire application. Create a class that extends `BaseSettings`:

```csharp
using FeintFramework.Config.Settings;
using FeintFramework.Routing;
using FeintFramework.Db.Sqlite.Migrator;
using FeintFramework.Contrib.Sessions;
using FeintFramework.Contrib.Auth;

namespace MyApp.Core;

public class Settings : BaseSettings
{
    // Enable debug mode for detailed error pages
    public override bool Debug => true;

    // Register your applications
    public override Type[] InstalledApps => [
        typeof(Blog.BlogApp),
        typeof(SessionsApp),    // Enable sessions
        typeof(AuthApp)         // Enable authentication
    ];

    // Root URL configuration
    public override RootUrlPatterns RootUrlPatterns => new MainUrlPatterns();

    // Middleware pipeline
    public override List<Type> Middlewares => [
        typeof(SessionMiddleware),
        typeof(AuthMiddleware)
    ];

    // Database configuration
    public override DatabaseHandler DatabaseHandler =>
        new SqliteDatabaseHandler(DatabaseConnectionString);

    // Optional: Register custom template tags
    public override void ConfigureAdditionalSettings()
    {
        TagRegistry.Register("mytag", MyTag.ParseMyTag);
    }
}
```

### Available Settings Properties

| Property | Type | Description |
|----------|------|-------------|
| `Debug` | `bool` | Enable debug mode |
| `InstalledApps` | `Type[]` | Registered applications |
| `RootUrlPatterns` | `RootUrlPatterns` | URL routing configuration |
| `Middlewares` | `List<Type>` | Middleware pipeline |
| `DatabaseHandler` | `DatabaseHandler` | Database provider |
| `DatabaseConnectionString` | `string` | Default: `Data Source=db.sqlite` |

---

## Routing

### Basic URL Patterns

```csharp
using FeintFramework.Routing;

class MainUrlPatterns : RootUrlPatterns
{
    public override List<UrlPattern> Urls => [
        // Simple route
        new UrlPattern("/about", AboutView, "about"),

        // Route with typed parameter
        new Path("/user/<int:id>", UserView, "user_detail"),

        // Route with string parameter
        new Path("/article/<str:slug>", ArticleView, "article"),

        // Include nested URL patterns
        new UrlPattern("/blog", new BlogUrls().Urls)
    ];
}
```

### Parameter Types

| Type | Pattern | Example Match |
|------|---------|---------------|
| `int` | `<int:name>` | `/post/123` |
| `str` | `<str:name>` | `/article/hello-world` |
| `slug` | `<slug:name>` | `/tag/my-tag-name` |

### Accessing Parameters

```csharp
FeintHttpResponse UserView(FeintHttpRequest request)
{
    // Get typed parameter from URL
    var userId = int.Parse(request.PathParams["id"]);

    return new FeintHttpResponse { /* ... */ };
}
```

### Reverse URL Lookup

Generate URLs from names:

```csharp
using FeintFramework.Routing;

// Generate URL: "/user/42"
var url = Router.Reverse("user_detail", new Dictionary<string, object> {
    { "id", 42 }
});
```

### Nested URL Patterns

Organize URLs by application:

```csharp
// Blog/BlogUrls.cs
class BlogUrls : UrlPatterns
{
    public override List<UrlPattern> Urls => [
        new UrlPattern("/", BlogListView, "blog:list"),
        new Path("/<int:id>", BlogDetailView, "blog:detail")
    ];
}

// Urls.cs - include with prefix
new UrlPattern("/blog", new BlogUrls().Urls)
// Results in: /blog/, /blog/123
```

---

## Views

Views are handlers that process requests and return responses.

### Basic View

```csharp
using FeintFramework.Http;

FeintHttpResponse MyView(FeintHttpRequest request)
{
    return new FeintHttpResponse
    {
        StatusCode = 200,
        Content = "Hello, World!",
        ContentType = "text/plain"
    };
}
```

### Class-Based View

```csharp
class ArticleView
{
    public FeintHttpResponse AsView(FeintHttpRequest request)
    {
        // Access request data
        var method = request.Method;      // GET, POST, etc.
        var path = request.Path;          // /article/hello
        var query = request.Query;        // Query parameters

        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = "<h1>Article</h1>",
            ContentType = "text/html"
        };
    }
}

// Register in URLs
new UrlPattern("/article", new ArticleView().AsView, "article")
```

### Request Object

`FeintHttpRequest` provides access to:

| Property | Type | Description |
|----------|------|-------------|
| `Method` | `string` | HTTP method (GET, POST, etc.) |
| `Path` | `string` | Request path |
| `PathParams` | `Dictionary<string, string>` | URL parameters |
| `Query` | `IQueryCollection` | Query string parameters |
| `Body` | `Stream` | Request body |
| `Headers` | `IHeaderDictionary` | HTTP headers |
| `Cookies` | `IRequestCookieCollection` | Request cookies |
| `Form` | `IFormCollection` | Form data (POST) |
| `AdditionalData` | `Dictionary<string, object>` | Custom data (session, user) |

### Response Types

**HTML Response:**
```csharp
return new FeintHttpResponse
{
    StatusCode = 200,
    Content = "<h1>Hello</h1>",
    ContentType = "text/html"
};
```

**Template Response:**
```csharp
return new FeintTemplateResponse("blog/detail.html", new Dictionary<string, object>
{
    { "post", post },
    { "comments", comments }
});
```

**Redirect:**
```csharp
return new FeintResponseRedirect("/login");
// or using shortcut
return Shortcuts.Redirect("login");  // Uses reverse URL lookup
```

---

## Templates

FeintFramework uses a Django-like template engine.

### Template Syntax

**Variables:**
```django
{{ variable }}
{{ user.name }}
{{ post.title }}
```

**Tags:**
```django
{% if condition %}
    Content if true
{% else %}
    Content if false
{% endif %}

{% for item in items %}
    <li>{{ item.name }}</li>
{% endfor %}
```

**Comments:**
```django
{# This is a comment #}
```

### Template Location

Templates are loaded from `templates/` directories within your applications:

```
Blog/
├── templates/
│   └── blog/
│       ├── base.html
│       ├── list.html
│       └── detail.html
```

### Template Inheritance

**base.html:**
```django
<!DOCTYPE html>
<html>
<head>
    <title>{% block title %}My Site{% endblock %}</title>
</head>
<body>
    <header>
        {% block header %}
        <h1>My Website</h1>
        {% endblock %}
    </header>

    <main>
        {% block content %}{% endblock %}
    </main>

    <footer>
        {% block footer %}
        <p>2025 My Site</p>
        {% endblock %}
    </footer>
</body>
</html>
```

**list.html:**
```django
{% extends "blog/base.html" %}

{% block title %}Blog Posts{% endblock %}

{% block content %}
<h2>All Posts</h2>
<ul>
    {% for post in posts %}
    <li><a href="/post/{{ post.Id }}">{{ post.Title }}</a></li>
    {% endfor %}
</ul>
{% endblock %}
```

### Built-in Template Tags

| Tag | Description |
|-----|-------------|
| `{% if %}...{% endif %}` | Conditional |
| `{% for %}...{% endfor %}` | Loop |
| `{% extends %}` | Template inheritance |
| `{% block %}...{% endblock %}` | Define/override blocks |
| `{% include %}` | Include another template |
| `{% url "name" %}` | Reverse URL lookup |
| `{% csrf_token %}` | CSRF protection token |

### URL Tag

```django
<a href="{% url 'post_detail' id=post.Id %}">View Post</a>
```

### Rendering Templates

```csharp
return new FeintTemplateResponse("blog/list.html", new Dictionary<string, object>
{
    { "posts", Post.Objects.ToList() },
    { "title", "All Blog Posts" }
});
```

---

## Models

Models define your database schema using LinqToDB attributes and FeintFramework field types.

### Defining a Model

```csharp
using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace MyApp.Blog.Models;

[Table(Name = "blog_post")]
public partial class Post : IntModel  // IntModel provides auto-increment Id
{
    [Column, CharField(Length = 255)]
    public string Title { get; set; }

    [Column, TextField]
    public string Content { get; set; }

    [Column, DateTimeField(DefaultValue = "2024-01-01")]
    public DateTime CreatedAt { get; set; }

    [Column, BooleanField(DefaultValue = false)]
    public bool IsPublished { get; set; }
}
```

### Available Field Types

| Field | Description |
|-------|-------------|
| `CharField(Length)` | String with max length |
| `TextField` | Unlimited text |
| `IntegerField` | Integer number |
| `BooleanField` | True/False |
| `DateTimeField` | Date and time |
| `ForeignKey` | Relationship to another model |

### Model Base Classes

| Class | Description |
|-------|-------------|
| `IntModel` | Model with `int Id` primary key |
| `Model` | Base model without predefined primary key |

### CRUD Operations

**Create:**
```csharp
var post = new Post
{
    Title = "Hello World",
    Content = "My first post",
    CreatedAt = DateTime.Now
};
post.Save();
```

**Read:**
```csharp
// Get all
var posts = Post.Objects.ToList();

// Get by ID
var post = Post.Objects.FirstOrDefault(p => p.Id == 1);

// Filter
var published = Post.Objects
    .Where(p => p.IsPublished)
    .OrderByDescending(p => p.CreatedAt)
    .ToList();
```

**Update:**
```csharp
var post = Post.Objects.First(p => p.Id == 1);
post.Title = "Updated Title";
post.Save();  // Performs UPDATE
```

**Delete:**
```csharp
var post = Post.Objects.First(p => p.Id == 1);
post.Delete();
```

### Relationships

**Foreign Key:**
```csharp
[Table(Name = "blog_post")]
public partial class Post : IntModel
{
    [Column, CharField(Length = 255)]
    public string Title { get; set; }

    [Association(ThisKey = nameof(Author), OtherKey = nameof(Author.Id))]
    [ForeignKey("BlogApp.Author", ForeignKeyAction.Cascade)]
    public Author Author { get; set; }
}

[Table(Name = "blog_author")]
public partial class Author : IntModel
{
    [Column, CharField(Length = 200)]
    public string Name { get; set; }
}
```

**Query with relationship:**
```csharp
var post = Post.Objects
    .LoadWith(p => p.Author)  // Eager load
    .First(p => p.Id == 1);

Console.WriteLine(post.Author.Name);
```

---

## Migrations

Migrations track database schema changes.

### Generating Migrations

After modifying models, generate migration files:

```bash
dotnet run --project path/to/FeintFramework.Db.MigrationGenerator -- \
    --settings "MyApp.Core.Settings, MyApp" \
    --project "/path/to/MyApp"
```

This creates migration files in `YourApp/Migrations/`:

```
Blog/
└── Migrations/
    ├── _0001_migration.g.cs  # Initial migration
    └── _0002_migration.g.cs  # Added field
```

### Running Migrations

Migrations run automatically on startup:

```csharp
Configurator.Settings = new MyApp.Core.Settings();
Configurator.Migrate();  // Applies pending migrations
Configurator.Configure(args);
```

### Migration Operations

The migration generator detects:

- **CreateModel** - New model added
- **AddField** - New field on existing model
- **RemoveField** - Field removed
- **AlterField** - Field type/constraints changed

### Migration Example

```csharp
// Auto-generated migration
public class _0001_migration : Migration
{
    public override (string, string)[] Dependencies => [];

    public override MigrationOperation[] Operations => [
        new CreateModel("Post")
        {
            Fields = [
                ("Id", new IntegerField { PrimaryKey = true }),
                ("Title", new CharField { Length = 255 }),
                ("Content", new TextField())
            ]
        }
    ];
}
```

---

## Forms

Forms handle user input with validation.

### Basic Form

```csharp
using FeintFramework.Forms;
using FeintFramework.Forms.Fields;

class ContactForm : Form
{
    public CharFormField Name = new CharFormField
    {
        Label = "Your Name",
        Required = true
    };

    public CharFormField Email = new CharFormField
    {
        Label = "Email Address"
    };

    public CharFormField Message = new CharFormField
    {
        Label = "Message",
        Widget = new TextAreaWidget()
    };
}
```

### Using Forms in Views

```csharp
FeintHttpResponse ContactView(FeintHttpRequest request)
{
    var form = new ContactForm();

    if (request.Method == "POST")
    {
        form.Bind(request.Form);

        if (form.IsValid())
        {
            // Process form data
            var name = form.Name.Value;
            var email = form.Email.Value;

            return Shortcuts.Redirect("thank_you");
        }
    }

    return new FeintTemplateResponse("contact.html", new Dictionary<string, object>
    {
        { "form", form }
    });
}
```

### Rendering Forms in Templates

```django
<form method="post">
    {% csrf_token %}

    {# Render entire form #}
    {{ form.AsP }}

    {# Or render individual fields #}
    <div>
        {{ form.Name.Label }}
        {{ form.Name.Widget }}
        {{ form.Name.Errors }}
    </div>

    <button type="submit">Send</button>
</form>
```

### ModelForm

Auto-generate forms from models:

```csharp
class PostForm : ModelForm<Post>
{
    public static class Meta
    {
        // Include all fields
        public static string[] Fields = ["__all__"];

        // Or specify fields explicitly
        // public static string[] Fields = ["Title", "Content"];
    }
}
```

**Using ModelForm:**
```csharp
// Create new
var form = new PostForm();

// Edit existing
var post = Post.Objects.First(p => p.Id == 1);
var form = new PostForm(post);

if (form.IsValid())
{
    form.Save();  // Creates or updates the model
}
```

### Form Fields

| Field | Description |
|-------|-------------|
| `CharFormField` | Text input |
| `PasswordFormField` | Password input |
| `IntegerFormField` | Number input |
| `BooleanFormField` | Checkbox |
| `ChoiceFormField` | Select dropdown |

### Widgets

| Widget | Description |
|--------|-------------|
| `TextInput` | `<input type="text">` |
| `PasswordInput` | `<input type="password">` |
| `TextAreaWidget` | `<textarea>` |
| `CheckboxInput` | `<input type="checkbox">` |
| `Select` | `<select>` |

---

## Admin Panel

Auto-generated CRUD interface for your models.

### Enabling Admin

1. Add Admin URLs:

```csharp
using FeintFramework.Contrib.Admin;

class MainUrlPatterns : RootUrlPatterns
{
    public override List<UrlPattern> Urls => [
        // Your URLs...

        // Admin panel at /admin/
        new UrlPattern("", new AdminUrls().Urls)
    ];
}
```

2. Register models with admin:

```csharp
// Blog/Admins.cs
using FeintFramework.Contrib.Admin;
using MyApp.Blog.Models;

namespace MyApp.Blog;

public class PostAdmin : ModelAdmin<Post>
{
    // Columns to display in list view
    public override string[]? ListDisplay => ["Title", "CreatedAt", "IsPublished"];
}

public class AuthorAdmin : ModelAdmin<Author>
{
    public override string[]? ListDisplay => ["Name", "Email"];
}
```

### Accessing Admin

Navigate to `http://localhost:9000/admin/`

Features:
- List all records
- Create new records
- Edit existing records
- Delete records
- Search and filter (if configured)

### Admin Configuration Options

```csharp
public class PostAdmin : ModelAdmin<Post>
{
    // Columns in list view
    public override string[]? ListDisplay => ["Title", "CreatedAt"];

    // Fields in edit form
    public override string[]? Fields => ["Title", "Content", "Author"];

    // Read-only fields
    public override string[]? ReadonlyFields => ["CreatedAt"];
}
```

---

## Authentication

Built-in user authentication system.

### Setup

1. Add Auth to settings:

```csharp
public override Type[] InstalledApps => [
    typeof(MyApp.Blog.BlogApp),
    typeof(SessionsApp),
    typeof(AuthApp)
];

public override List<Type> Middlewares => [
    typeof(SessionMiddleware),
    typeof(AuthMiddleware)
];
```

### User Model

The built-in `User` model provides:

| Field | Type | Description |
|-------|------|-------------|
| `Username` | `string` | Unique username |
| `Email` | `string` | Email address |
| `Password` | `string` | Hashed password |
| `IsStaff` | `bool` | Can access admin |
| `IsSuperuser` | `bool` | Full permissions |

### Creating Users

```csharp
using FeintFramework.Contrib.Auth;

var user = new User
{
    Username = "john",
    Email = "john@example.com",
    IsStaff = true
};
user.SetPassword("secure_password");  // Hashes password
user.Save();
```

### Login/Logout

```csharp
using FeintFramework.Contrib.Auth;

// Login
FeintHttpResponse LoginView(FeintHttpRequest request)
{
    if (request.Method == "POST")
    {
        var username = request.Form["username"];
        var password = request.Form["password"];

        var user = User.Objects.FirstOrDefault(u => u.Username == username);

        if (user != null && user.CheckPassword(password))
        {
            // Get session store
            var session = request.GetSessionStore();
            session.Set("user_id", user.Id);

            return Shortcuts.Redirect("home");
        }
    }

    return new FeintTemplateResponse("login.html");
}

// Logout
FeintHttpResponse LogoutView(FeintHttpRequest request)
{
    var session = request.GetSessionStore();
    session.Remove("user_id");

    return Shortcuts.Redirect("home");
}
```

### Accessing Current User

With `AuthMiddleware`, the current user is available on the request:

```csharp
FeintHttpResponse ProfileView(FeintHttpRequest request)
{
    var user = request.GetUser();  // Returns User or null

    if (user == null)
    {
        return Shortcuts.Redirect("login");
    }

    return new FeintTemplateResponse("profile.html", new Dictionary<string, object>
    {
        { "user", user }
    });
}
```

---

## Sessions

Server-side session storage.

### Setup

Add sessions to settings:

```csharp
public override Type[] InstalledApps => [
    typeof(SessionsApp)
];

public override List<Type> Middlewares => [
    typeof(SessionMiddleware)
];
```

### Using Sessions

```csharp
FeintHttpResponse MyView(FeintHttpRequest request)
{
    // Get session store
    var session = request.GetSessionStore();

    // Set value
    session.Set("cart_items", 5);

    // Get value
    var items = session.Get<int>("cart_items");

    // Remove value
    session.Remove("cart_items");

    // Check if exists
    if (session.ContainsKey("user_id"))
    {
        // ...
    }
}
```

### Session Configuration

Sessions are stored in the database and identified by a cookie. Configure in settings:

```csharp
public override string SessionCookieName() => "feint_session";

public override CookieOptions SessionCookieOptions() => new CookieOptions
{
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Strict
};
```

---

## Example Project Walkthrough

The `Example/` directory contains a complete reference application.

### Project Structure

```
Example/
├── Core/
│   └── Settings.cs           # Application configuration
├── Blog/
│   ├── App.cs               # BlogApp registration
│   ├── Models/
│   │   ├── Author.cs        # Author model
│   │   └── BlogPost.cs      # BlogPost model
│   ├── Migrations/          # Auto-generated migrations
│   ├── Admins.cs            # Admin panel configuration
│   └── Views.cs             # View handlers
├── Urls.cs                  # URL routing
├── Manage.cs                # Entry point (Program.cs)
└── Example.csproj
```

### Entry Point (Manage.cs)

```csharp
using FeintFramework.Config;

// Set application settings
Configurator.Settings = new Example.Core.Settings();

// Run database migrations
Configurator.Migrate();

// Start HTTP server
Configurator.Configure(args);  // Listens on http://localhost:9000
```

### Settings (Core/Settings.cs)

```csharp
public class Settings : BaseSettings
{
    public override bool Debug => true;

    public override Type[] InstalledApps => [
        typeof(Blog.BlogApp),
        typeof(SessionsApp),
        typeof(AuthApp)
    ];

    public override RootUrlPatterns RootUrlPatterns => new MainUrlPatterns();

    public override List<Type> Middlewares => [
        typeof(SessionMiddleware),
        typeof(AuthMiddleware)
    ];

    public override DatabaseHandler DatabaseHandler =>
        new SqliteDatabaseHandler(DatabaseConnectionString);

    public override void ConfigureAdditionalSettings()
    {
        TagRegistry.Register("admin_url", AdminUrlTag.ParseAdminUrlTag);
    }
}
```

### Models

**Author (Blog/Models/Author.cs):**
```csharp
[Table(Name = "blog_app_author")]
public partial class Author : IntModel
{
    [Column(Name = "full_name"), CharField(Length = 200, NotNull = false)]
    public string FullName { get; set; }

    [Column("email"), CharField(Length = 255, NotNull = true)]
    public string Email { get; set; }

    [Column("created_at"), DateTimeField(NotNull = true, DefaultValue = "2021-01-01")]
    public DateTime CreatedAt { get; set; }
}
```

**BlogPost (Blog/Models/BlogPost.cs):**
```csharp
[Table(Name = "blog_app_blog_post")]
public partial class BlogPost : IntModel
{
    [Column, NotNull, CharField(Length = 255)]
    public string Title { get; set; }

    [Column, TextField()]
    public string Content { get; set; }

    [Association(ThisKey = nameof(Author), OtherKey = nameof(Author.Id))]
    [ForeignKey("BlogApp.Author", ForeignKeyAction.Cascade)]
    public Author Author { get; set; }
}
```

### URL Configuration (Urls.cs)

```csharp
class MainUrlPatterns : RootUrlPatterns
{
    public override List<UrlPattern> Urls => [
        new UrlPattern("/example", new ExampleView().AsView, "example"),
        new Path("/example2/<int:test>", new ExampleView().AsView, "example2"),
        new UrlPattern("", new AdminUrls().Urls)  // Admin at /admin/
    ];
}
```

### Admin Configuration (Blog/Admins.cs)

```csharp
public class AuthorAdmin : ModelAdmin<Author>
{
    public override string[]? ListDisplay => ["FullName", "Email"];
}

public class BlogPostAdmin : ModelAdmin<BlogPost>
{
}
```

### Views (Blog/Views.cs)

```csharp
class BlogPostForm : ModelForm<BlogPost>
{
    public static class Meta
    {
        public static string[] Fields = ["__all__"];
    }
}

class ExampleView
{
    public FeintHttpResponse AsView(FeintHttpRequest request)
    {
        // Create test data
        var author = new Author
        {
            FullName = "Test Author",
            Email = "test@example.com"
        };
        author.Save();

        // Render form
        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = new BlogPostForm().AsP,
            ContentType = "text/html"
        };
    }
}
```

### Running the Example

```bash
cd Example
dotnet run
```

Navigate to:
- `http://localhost:9000/example` - Form demo
- `http://localhost:9000/admin/` - Admin panel

---

## FAQ

### How do I change the port?

Modify `Configurator.cs` or set it in your settings (feature may require customization).

### Where is the database stored?

By default, SQLite creates `db.sqlite` in your project root.

### How do I add custom middleware?

1. Create a middleware class:

```csharp
using FeintFramework.Middleware;

public class LoggingMiddleware : BaseMiddleware
{
    public LoggingMiddleware(RequestHandler handler) : base(handler) { }

    public override FeintHttpResponse HandleRequest(FeintHttpRequest request)
    {
        Console.WriteLine($"{request.Method} {request.Path}");
        return handler(request);  // Call next middleware
    }
}
```

2. Register in settings:

```csharp
public override List<Type> Middlewares => [
    typeof(LoggingMiddleware),
    typeof(SessionMiddleware),
    typeof(AuthMiddleware)
];
```

### How do I serve static files?

Currently, static files should be served by a reverse proxy (nginx, etc.) in production. For development, you can create a view that reads and returns file contents.

### How do I run in production?

1. Set `Debug => false` in settings
2. Use a reverse proxy (nginx) in front of Kestrel
3. Configure proper database connection string
4. Set secure session cookie options

---

## Getting Help

- Check the [README](../README.md) for framework architecture
- Review the Example project for working code
- Open an issue on GitHub for bugs or questions

---

*FeintFramework - Django-inspired web development for .NET*
