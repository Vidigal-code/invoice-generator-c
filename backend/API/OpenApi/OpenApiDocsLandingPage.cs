namespace InvoiceGenerator.Api.API.OpenApi;

/// <summary>HTML hub linking Swagger UI and ReDoc (English and Brazilian Portuguese).</summary>
public static class OpenApiDocsLandingPage
{
    public const string Html = """
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>invoice-generator-c — API documentation</title>
  <style>
    :root { color-scheme: light dark; font-family: system-ui, sans-serif; }
    body { max-width: 52rem; margin: 2rem auto; padding: 0 1rem; line-height: 1.5; }
    h1 { font-size: 1.35rem; }
    ul { padding-left: 1.2rem; }
    a { color: #1565c0; }
    @media (prefers-color-scheme: dark) { a { color: #90caf9; } }
    code { font-size: .9em; }
    .muted { opacity: .85; font-size: .9rem; }
  </style>
</head>
<body>
  <h1>API documentation — invoice-generator-c</h1>
  <p class="muted">Use the <strong>api</strong> service port from Docker Compose (e.g. <code>http://localhost:5283</code>).</p>
  <h2>English</h2>
  <ul>
    <li><a href="/docs/en/swagger">Swagger UI</a> — try requests</li>
    <li><a href="/docs/en/redoc">ReDoc</a> — read-only reference</li>
    <li>OpenAPI JSON: <a href="/swagger/v1-en/swagger.json"><code>/swagger/v1-en/swagger.json</code></a></li>
  </ul>
  <h2>Português (Brasil)</h2>
  <ul>
    <li><a href="/docs/br/swagger">Swagger UI</a></li>
    <li><a href="/docs/br/redoc">ReDoc</a></li>
    <li>OpenAPI JSON: <a href="/swagger/v1-br/swagger.json"><code>/swagger/v1-br/swagger.json</code></a></li>
  </ul>
  <p class="muted">Authentication: after <code>POST /api/Auth/login</code> in Swagger, the browser keeps the <code>AuthToken</code> HttpOnly cookie for subsequent calls on the same host.</p>
</body>
</html>
""";
}
