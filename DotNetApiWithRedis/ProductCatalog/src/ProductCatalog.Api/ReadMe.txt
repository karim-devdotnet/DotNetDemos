dotnet ef migrations add InitialCreate --context ProductCatalogContext --project ProductCatalog.Api -s ProductCatalog.Api -v
dotnet ef database update --context ProductCatalogContext --project ProductCatalog.Api -s ProductCatalog.Api -v



create product:
curl -X POST http://localhost:8080/products -H "Content-Type: application/json" -d "{\"name\":\"Mechanical Keyboard\",\"price\":149.99}"