# Puroguramu

Application web ASP.NET Core Razor Pages pour la gestion de lecons et d'exercices de programmation.
Projet realise en duo.

## Stack

- .NET 6
- ASP.NET Core Razor Pages
- Entity Framework Core
- SQLite en local (`Development`)
- SQL Server en production (`Production`)

## Prerequis

- Windows + PowerShell
- .NET SDK 6.x
- Outil EF Core (`dotnet ef`)
- `sqlite3` en ligne de commande

## Lancer le projet en local (PowerShell)

1. Se placer a la racine du projet:

```powershell
cd "c:\Users\badr\Documents\projet helmo\porugumaru"
```

2. Restaurer les dependances:

```powershell
dotnet restore Puroguramu.sln
```

3. Initialiser la base SQLite locale (a faire une fois, ou si la DB est vide):

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet ef dbcontext script --project Puroguramu.Infrastructures --startup-project Puroguramu.App --context ApplicationDbContext --output Puroguramu.App\db-init.sql
sqlite3 Puroguramu.App\localdatabase.db ".read Puroguramu.App\db-init.sql"
Remove-Item Puroguramu.App\db-init.sql -Force
```

4. Demarrer l'application:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project Puroguramu.App --no-launch-profile --urls "http://127.0.0.1:5083"
```

5. Ouvrir l'application dans le navigateur:

- `http://127.0.0.1:5083`
- si besoin avec PathBase: `http://127.0.0.1:5083/E200382`

## Comptes de demo (seed)

Le seeding cree des utilisateurs au demarrage avec le meme mot de passe:

- (matricule `P123456`)
- (matricule `Q123456`)
- Mot de passe: `kjjpLLa6r3wmfCO@`

## Arreter l'application

- Dans le terminal: `Ctrl + C`
- Ou via PID:

```powershell
Stop-Process -Id <PID> -Force
```

## Depannage

- Si l'application essaie de se connecter au SQL Server distant, lance toujours avec:
  - `ASPNETCORE_ENVIRONMENT=Development`
  - `--no-launch-profile`
- Si tu vois `SQLite Error 1: no such table: AspNetRoles`, reinitialise la DB avec l'etape 3.
- Si le port `5083` est deja pris, change la valeur dans `--urls`.
