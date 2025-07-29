# Implementační plán - Školní knihovna

## 1. Přehled projektu

Aplikace pro správu školní knihovny s víceúrovňovou autorizací pro různé role uživatelů (knihovník, učitelé, žáci, rodiče). Systém bude postavený na OptimalyBlueprint s využitím Blueprint pattern pro rychlé prototypování a automatické nacenění.

## 2. Identifikované entity

### Základní entity (Core)
1. **Book** (Kniha)
   - Id, ISBN, Title, Subtitle, Author, Publisher
   - YearPublished, Genre, Keywords, PhysicalCondition
   - TotalCopies, AvailableCopies, AgeRecommendation
   - ImageUrl, Description, ReadingLevel

2. **User** (Uživatel)
   - Id, FirstName, LastName, Email, Phone
   - UserType (Student/Teacher/Librarian/Parent)
   - ClassId (pro žáky), IsActive
   - RegistrationDate, LastLoginDate

3. **Loan** (Výpůjčka)
   - Id, BookId, UserId, LoanDate, DueDate
   - ReturnDate, Status, Notes
   - RenewCount, IsOverdue

4. **Reservation** (Rezervace)
   - Id, BookId, UserId, ReservationDate
   - ExpiryDate, Status, NotificationSent

5. **BookRating** (Hodnocení knih)
   - Id, BookId, UserId, Rating (1-5), Review
   - CreatedDate, IsRecommended, HelpfulVotes

6. **ReadingNote** (Poznámky k přečteným knihám)
   - Id, BookId, UserId, NoteText, PageNumber
   - IsPrivate, CreatedDate, Tags

7. **BookRecommendation** (Doporučení knih)
   - Id, BookId, UserId, RecommendationType
   - SourceType (AI/Teacher/System), Reason
   - CreatedDate, IsAccepted

### Lookup entity
8. **Genre** (Žánr)
   - Id, Name, Description, SortOrder
   - IsEducational, AgeRecommendation

9. **Class** (Třída)
   - Id, Name, Grade, SchoolYear
   - TeacherId, IsActive

10. **AgeGroup** (Věková skupina)
    - Id, Name, MinAge, MaxAge
    - ReadingLevelRange, RecommendedGenres

11. **ChipCard** (Čipová karta)
    - Id, CardNumber, UserId, IsActive
    - IssueDate, ExpiryDate, LastUsed

### Complex entity
12. **Student** (Žák - rozšíření User)
    - UserId, ClassId, ParentId
    - ReadingLevel, TotalBooksRead
    - ResponsibilityScore, FavoriteGenres

13. **LibraryStatistics** (Statistiky)
    - EntityType, EntityId, Period
    - LoanCount, AverageRating, PopularityScore
    - DamageReports, LateReturns

14. **DiscussionTopic** (Diskusní téma)
    - Id, Title, Description, CreatedBy
    - BookId, IsModerated, ViewCount
    - CreatedDate, LastActivityDate

15. **RewardSystem** (Motivační systém)
    - Id, UserId, RewardType, Points
    - AchievementDate, Description, BadgeImageUrl

### Audit entity
16. **ActivityLog** (Log aktivit)
    - Id, UserId, Action, EntityType, EntityId
    - Timestamp, IPAddress, UserAgent

17. **BookConditionReport** (Hlášení stavu knihy)
    - Id, BookId, LoanId, ReportedBy
    - ConditionBefore, ConditionAfter
    - DamageDescription, PhotoUrl

18. **DiscussionPost** (Příspěvek v diskusi)
    - Id, TopicId, UserId, Content
    - CreatedDate, ParentPostId, IsModerated

## 3. Architektura aplikace

### Technologie
- **Backend**: ASP.NET Core MVC (.NET 9)
- **Frontend**: Razor Pages + Bootstrap 5 + Alpine.js
- **Database**: PostgreSQL (pro produkci) / SQLite (pro vývoj)
- **Auth**: ASP.NET Core Identity s rolemi
- **Session**: Distribuovaná cache (Redis pro produkci)

### Vrstvy aplikace
```
OptimalySchoolLibrary/
├── Controllers/
│   ├── BooksController.cs       # CRUD operace s knihami
│   ├── LoansController.cs       # Správa výpůjček
│   ├── UsersController.cs       # Správa uživatelů
│   ├── ReportsController.cs     # Statistiky a reporty
│   └── DashboardController.cs   # Role-based dashboardy
├── Services/
│   ├── BookService.cs           # Business logika pro knihy
│   ├── LoanService.cs           # Logika výpůjček, upomínky
│   ├── NotificationService.cs   # Email/SMS notifikace
│   └── StatisticsService.cs     # Generování statistik
├── Views/
│   ├── Shared/
│   │   ├── _LibrarianLayout.cshtml
│   │   ├── _TeacherLayout.cshtml
│   │   ├── _StudentLayout.cshtml
│   │   └── _ParentLayout.cshtml
│   └── [Controller folders...]
└── Areas/
    ├── Librarian/              # Knihovník funkce
    ├── Teacher/                # Učitelské funkce
    ├── Student/                # Žákovské funkce
    └── Parent/                 # Rodičovské funkce
```

## 4. Funkční požadavky

### Pro knihovníka
- **Evidence knih**: Kompletní CRUD s batch importem, ISBN skenování
- **Správa výpůjček**: Půjčování, vracení, prodlužování, čipové karty
- **Správa uživatelů**: Registrace, deaktivace, role, zodpovědnostní skóre
- **Statistiky**: Dashboardy s Excel exportem, grafy popularity žánrů
- **Inventura**: Kontrola stavu knihovny, vyřazování starých knih
- **Moderování**: Diskusní fórum, hodnocení knih

### Pro učitele
- **Přehled třídy**: Čtenářská aktivita žáků, zodpovědnost
- **Doporučení**: Knihy dle učiva a ročníku, AI doporučení
- **Rezervace**: Hromadné rezervace pro třídu
- **Diskuse**: Moderování diskusí o knihách
- **Propojení s výukou**: Knihy související s probíraným učivem

### Pro žáky
- **Katalog**: Vyhledávání, filtrování podle věku a obtížnosti
- **Moje výpůjčky**: Přehled, prodloužení, QR/čipové karty
- **Rezervace**: Online rezervace s notifikacemi
- **Čtenářský deník**: Poznámky, hodnocení (hvězdičky/smajlíci)
- **Diskusní fórum**: Diskuse o knihách, doporučení kamarádům
- **Gamifikace**: Body za čtení, badges, odměny
- **Personalizace**: AI doporučení podle preferencí

### Pro rodiče
- **Přehled dítěte**: Vypůjčené knihy, termíny, poškození
- **Notifikace**: Upomínky emailem, SMS
- **Historie**: Kompletní čtenářská historie s žánrovým přehledem
- **Zodpovědnost**: Skóre zodpovědnosti při vracení knih

## 5. UI/UX Design

### Dashboard layouts

#### Knihovník
```
+------------------+
| Dnešní výpůjčky  |
| Vrácení: 15      |
| Půjčeno: 23      |
+------------------+
| Top knihy měsíce |
| 1. Harry Potter  |
| 2. Hobbit        |
+------------------+
```

#### Žák
```
+------------------+
| Moje knihy    3  |
| Vrátit do: 5 dní |
+------------------+
| Doporučené knihy |
| [Obálka] [Obálka]|
+------------------+
```

### Barevné schéma
- **Primary**: #2E7D32 (Zelená - klid, čtení)
- **Secondary**: #1976D2 (Modrá - důvěra)
- **Warning**: #F57C00 (Oranžová - upomínky)
- **Danger**: #D32F2F (Červená - překročení termínu)

## 6. Bezpečnost

### Autentizace a autorizace
- Multi-factor authentication pro knihovníky
- Role-based access control (RBAC)
- Session timeout po 30 minutách
- IP whitelisting pro admin funkce

### Ochrana dat
- HTTPS everywhere
- Šifrování citlivých dat
- GDPR compliance
- Pravidelné zálohy

## 7. Integrace

### Školní systémy
- API pro Bakaláře/Edupage
- Import žáků ze školní matriky
- Export statistik pro vedení školy

### Notifikace
- Email gateway (SendGrid)
- SMS brána (pro upomínky)
- Push notifikace (mobile app v2)

## 8. Fázování projektu

### Fáze 1: MVP (3 měsíce)
- Základní CRUD operace
- Výpůjčky a vracení
- Role knihovník + žák
- Jednoduché statistiky

### Fáze 2: Rozšíření (2 měsíce)
- Role učitel + rodič
- Rezervace
- Notifikace
- Pokročilé statistiky

### Fáze 3: Integrace (2 měsíce)
- Napojení na školní systémy
- Mobilní aplikace
- Čipové karty
- Diskusní fórum

## 9. Cenový odhad (dle OptimalyBlueprint)

### Entity a jejich ceny
**Core entity (7x):**
- Book, User, Loan, Reservation, BookRating, ReadingNote, BookRecommendation
- 7 × 12,500 Kč = **87,500 Kč**

**Lookup entity (4x):**
- Genre, Class, AgeGroup, ChipCard  
- 4 × 5,000 Kč = **20,000 Kč**

**Complex entity (4x):**
- Student, LibraryStatistics, DiscussionTopic, RewardSystem
- 4 × 20,000 Kč = **80,000 Kč**

**Audit entity (3x):**
- ActivityLog, BookConditionReport, DiscussionPost
- 3 × 10,000 Kč = **30,000 Kč**

**Celkem za entity: 217,500 Kč**

### Relace (odhad 15 relací)
- OneToOne: 3 × 5,000 Kč = 15,000 Kč
- OneToMany: 8 × 7,500 Kč = 60,000 Kč  
- ManyToMany: 4 × 12,500 Kč = 50,000 Kč

**Celkem za relace: 125,000 Kč**

### UI komponenty (18 entit × 23,750 Kč CRUD)
**UI náklady: 427,500 Kč**

### Specializované funkce
- **Excel export**: 25,000 Kč
- **AI doporučení**: 50,000 Kč
- **Diskusní fórum**: 37,500 Kč
- **Gamifikace/Badges**: 30,000 Kč
- **Čipové karty integrace**: 40,000 Kč
- **QR kódy**: 15,000 Kč
- **SMS/Email notifikace**: 20,000 Kč

**Celkem specializované: 217,500 Kč**

### Další náklady
- **Bezpečnost (multi-role)**: 37,500 Kč
- **Edupage/Bakaláři integrace**: 75,000 Kč
- **Testování (20% z vývoje)**: 175,000 Kč
- **Deployment + DevOps**: 50,000 Kč

**Další náklady celkem: 337,500 Kč**

---

## **CELKOVÁ CENA: 1,325,000 Kč (bez DPH)**
## **Odhadovaný čas: 2,650 hodin (66 týdnů)**
## **S DPH: 1,603,250 Kč**

### Porovnání s původním odhadem
- **Původní odhad**: €22,640 (565,000 Kč)
- **Aktualizovaný odhad**: 1,325,000 Kč  
- **Nárůst**: +134% (zapomenuté funkce!)

## 10. Maintenance a provoz

### Měsíční náklady
- Hosting (Azure/AWS): 1,250-2,500 Kč
- Database PostgreSQL: 750-1,250 Kč
- Email/SMS service: 500 Kč
- Backups a monitoring: 500 Kč
- SSL certifikáty: 250 Kč
- **Celkem: 3,250-5,000 Kč/měsíc**

### Podpora
- SLA 99.9% uptime
- Response time: 4 hodiny
- Měsíční updaty
- Školení uživatelů

## 11. Rizika a jejich mitigace

1. **Adopce uživateli**
   - Řešení: Intuitivní UI, školení, video tutoriály

2. **GDPR compliance**
   - Řešení: Privacy by design, pravidelné audity

3. **Výkon při velkém počtu knih**
   - Řešení: Indexování, caching, CDN

4. **Integrace se školními systémy**
   - Řešení: Modulární API, fallback řešení

## 12. Success metrics

- 80% žáků aktivně používá systém
- Snížení času na administrativu o 60%
- Zvýšení počtu výpůjček o 40%
- 90% spokojnost uživatelů

---

**Tento plán je living document a bude průběžně aktualizován dle potřeb školy a feedback uživatelů.**