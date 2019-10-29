# Datbase setup

## Create Migrations

`dotnet ef migrations add InitialCreate --project "../Database/src" -c DrugDbContext`
`dotnet ef migrations add InitialCreate --project "../Database/src" -c AuditDbContext`

## Run migrations

`dotnet ef database update --project "../Database/src" -c DrugDbContext`
`dotnet ef database update --project "../Database/src" -c AuditDbContext`