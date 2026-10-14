# Svetsko prvenstvo u košarci – Qatar 2027

Projekat predstavlja informacioni sistem za organizaciju i upravljanje Svetskim prvenstvom u košarci Qatar 2027, razvijen prema arhitekturi **A**.

Sistem obuhvata dve funkcionalne celine:
- **A.1 – Administracija prvenstva i upravljanje podacima**
- **A.2 – Korisnički portal za pregled prvenstva i kupovinu karata**

Obe celine koriste zajedničku MySQL bazu podataka.

## 1. Korišćene tehnologije
| Frontend - React 18, TypeScript |
| Backend - ASP.NET Core 8 Web API |
| Baza podataka - MySQL |
| ORM - Entity Framework Core |
| Keširanje - Memory Cache |
| API dokumentacija - Swagger |
| Razvojno okruženje - Visual Studio Community 2026 |
| Upravljanje paketima - npm, NuGet |

## 2. Funkcionalnosti aplikacije

### Korisnički portal

- Prikaz osnovnih informacija o prvenstvu
- Pregled grupa, reprezentacija i utakmica
- Keširanje podataka radi bržeg učitavanja
- Kupovina karata za jednu ili više utakmica
- Popust od 10% za kupovinu do definisanog datuma
- Dodatni popust od 5% uz validan promo-kod
- Automatsko generisanje jedinstvene šifre pristupa
- Generisanje promo-koda nakon kupovine
- Prikaz cena u različitim valutama
- Konverzija iz EUR pomoću eksternog API-ja
- Korišćenje rezervnih kurseva ako API nije dostupan
- Pretraga kupljenih karata
- Izmena podataka na kartama
- Otkazivanje karata uz čuvanje istorije

### Administracija

- Upravljanje podacima o prvenstvu
- Dodavanje, izmena i brisanje grupa
- Upravljanje reprezentacijama
- Upravljanje stadionima
- Kreiranje i izmena utakmica
- Podešavanje cena karata
- Upravljanje valutama
- Pregled prodaje karata
- Izveštaji o prodaji po utakmicama i datumima
- Grafički prikaz statističkih podataka


## 3. Preduslovi za pokretanje

Pre pokretanja projekta potrebno je instalirati:

1. Visual Studio Community 2022 ili noviji, sa podrškom za ASP.NET Core
2. .NET 8 SDK
3. Node.js LTS sa npm alatom
4. MySQL Server
5. SQLyog ili MySQL Workbench

Provera instalacije:

```powershell
node -v
npm -v
dotnet --version
```

## 4. Pokretanje baze podataka

1. Pokrenuti MySQL Server.
2. Otvoriti SQLyog ili MySQL Workbench.
3. Otvoriti fajl `sql/database.sql`.
4. Izvršiti SQL skriptu.
5. Proveriti da li je baza uspešno kreirana.

Podrazumevani naziv baze:

`SvetskoPrvenstvoUKosarci`

Podrazumevani korisnik:

`root`

Podrazumevana lozinka je prazna.

Ako MySQL instalacija koristi drugačije pristupne podatke, potrebno je promeniti konekcioni string u:

`backend/API/appsettings.json`

**Napomena:** Pre ponovnog izvršavanja SQL skripte proveriti da li sadrži naredbe `DROP TABLE`, jer one mogu obrisati postojeće podatke.

## 5. Pokretanje backenda

Backend je razvijen korišćenjem ASP.NET Core 8 Web API tehnologije.

### Pokretanje preko Visual Studio Community-ja

1. Otvoriti Visual Studio.
2. Izabrati `File → Open → Project/Solution`.
3. Otvoriti `backend/SvetskoPrvenstvoUKosarci.sln`.
4. Postaviti projekat `API` kao Startup Project.
5. Pokrenuti aplikaciju pomoću `F5` ili `Ctrl + F5`.

### Pokretanje pomoću terminala

Iz glavnog direktorijuma projekta:

```powershell
cd backend/API
dotnet restore
dotnet run
```

Swagger dokumentacija:

http://localhost:5260/swagger

Adresa važi ako je backend konfigurisan da koristi port 5260.

Backend mora ostati pokrenut kako bi frontend mogao da komunicira sa API-jem.

## 6. Pokretanje frontenda

Frontend je razvijen korišćenjem React-a i TypeScript-a.

### Instalacija biblioteka

Otvoriti terminal u folderu `frontend` i izvršiti:

```powershell
cd frontend
npm install
```

Ova komanda instalira sve neophodne biblioteke definisane u `package.json`.

### Provera TypeScript koda

Pre pokretanja preporučuje se:

```powershell
npx tsc --noEmit
```

Ako nema prijavljenih grešaka, TypeScript provera je uspešno završena.

### Pokretanje aplikacije

```powershell
npm start
```

Nakon uspešnog pokretanja frontend je dostupan na:

http://localhost:3000

**Napomena:** Koristi se `npm start`, a ne `npm run dev`, zato što je frontend podešen pomoću `react-scripts`.

## 7. Redosled pokretanja kompletnog sistema

Za ispravan rad aplikacije potrebno je:

1. Pokrenuti MySQL Server.
2. Proveriti da baza `SvetskoPrvenstvoUKosarci` postoji.
3. Pokrenuti ASP.NET Core backend.
4. Proveriti dostupnost Swagger dokumentacije.
5. Otvoriti drugi terminal.
6. U folderu `frontend` pokrenuti `npm start`.
7. Otvoriti http://localhost:3000.

Backend i frontend treba da ostanu aktivni tokom korišćenja aplikacije.

## 8. Testiranje aplikacije

Nakon pokretanja potrebno je proveriti:

- Učitavanje početne stranice
- Prikaz grupa i reprezentacija
- Prikaz utakmica
- Kupovinu karata
- Obračun popusta i promo-kodova
- Preračunavanje valuta
- Pretragu i izmenu kupljenih karata
- Otkazivanje karata
- Administrativne funkcionalnosti
- Generisanje izveštaja i grafikona

## 9. Dodatne napomene

U razvojnom okruženju backend koristi Entity Framework Core za rad sa bazom podataka.

Ako je uključena metoda `Database.EnsureCreated()`, ona kreira šemu baze kada ona ne postoji, ali ne izvršava migracije nad već postojećom šemom.

Preporučuje se korišćenje SQL skripte `database.sql` za inicijalno podešavanje baze.

Za ispravan rad sistema neophodno je da frontend ima pristup odgovarajućoj adresi backend API-ja.