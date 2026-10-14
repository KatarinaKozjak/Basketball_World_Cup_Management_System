CREATE DATABASE IF NOT EXISTS SvetskoPrvenstvoUKosarci CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE SvetskoPrvenstvoUKosarci;

SET FOREIGN_KEY_CHECKS=0;
DROP TABLE IF EXISTS ticket_items;
DROP TABLE IF EXISTS promo_codes;
DROP TABLE IF EXISTS tickets;
DROP TABLE IF EXISTS customers;
DROP TABLE IF EXISTS matches;
DROP TABLE IF EXISTS stadiums;
DROP TABLE IF EXISTS teams;
DROP TABLE IF EXISTS `groups`;
DROP TABLE IF EXISTS currencies;
DROP TABLE IF EXISTS championships;
SET FOREIGN_KEY_CHECKS=1;

CREATE TABLE championships (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  Name VARCHAR(150) NOT NULL,
  Location VARCHAR(200) NOT NULL,
  StartsAt DATETIME NOT NULL,
  EndsAt DATETIME NOT NULL,
  Discount10Until DATETIME NOT NULL,
  Description VARCHAR(1000) NULL
);
CREATE TABLE currencies (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  Code VARCHAR(3) NOT NULL UNIQUE,
  IsAllowed BOOLEAN NOT NULL DEFAULT TRUE
);
CREATE TABLE `groups` (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  Code VARCHAR(30) NOT NULL,
  ChampionshipId BIGINT NOT NULL,
  CONSTRAINT FK_groups_championships FOREIGN KEY (ChampionshipId) REFERENCES championships(Id) ON DELETE CASCADE
);
CREATE TABLE teams (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  Name VARCHAR(100) NOT NULL,
  PlayerCount INT NOT NULL DEFAULT 0,
  Coach VARCHAR(200) NULL,
  Wins INT NOT NULL DEFAULT 0,
  Losses INT NOT NULL DEFAULT 0,
  Points INT NOT NULL DEFAULT 0,
  GroupId BIGINT NOT NULL,
  CONSTRAINT FK_teams_groups FOREIGN KEY (GroupId) REFERENCES `groups`(Id)
);
CREATE TABLE stadiums (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  Name VARCHAR(120) NOT NULL,
  City VARCHAR(100) NOT NULL,
  Address VARCHAR(200) NOT NULL,
  Capacity INT NOT NULL
);
CREATE TABLE matches (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  StartsAt DATETIME NOT NULL,
  BasePriceEur DECIMAL(18,2) NOT NULL,
  StadiumId BIGINT NOT NULL,
  Team1Id BIGINT NOT NULL,
  Team2Id BIGINT NOT NULL,
  CONSTRAINT FK_matches_stadiums FOREIGN KEY (StadiumId) REFERENCES stadiums(Id),
  CONSTRAINT FK_matches_team1 FOREIGN KEY (Team1Id) REFERENCES teams(Id),
  CONSTRAINT FK_matches_team2 FOREIGN KEY (Team2Id) REFERENCES teams(Id)
);
CREATE TABLE customers (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  FirstName VARCHAR(80) NOT NULL,
  LastName VARCHAR(80) NOT NULL,
  Address VARCHAR(200) NOT NULL,
  PostalCode VARCHAR(20) NOT NULL,
  City VARCHAR(100) NOT NULL,
  Country VARCHAR(100) NOT NULL,
  Email VARCHAR(180) NOT NULL,
  INDEX IX_customers_Email (Email)
);
CREATE TABLE tickets (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  AccessCode VARCHAR(30) NOT NULL UNIQUE,
  CreatedAt DATETIME(6) NOT NULL,
  UpdatedAt DATETIME(6) NOT NULL,
  Status VARCHAR(20) NOT NULL,
  CustomerId BIGINT NOT NULL,
  CurrencyId BIGINT NOT NULL,
  ExchangeRate DECIMAL(18,6) NOT NULL,
  SubtotalEur DECIMAL(18,2) NOT NULL,
  DiscountPercent DECIMAL(5,2) NOT NULL,
  TotalAmount DECIMAL(18,2) NOT NULL,
  UsedPromoCodeId BIGINT NULL,
  CONSTRAINT FK_tickets_customers FOREIGN KEY (CustomerId) REFERENCES customers(Id),
  CONSTRAINT FK_tickets_currencies FOREIGN KEY (CurrencyId) REFERENCES currencies(Id)
);
CREATE TABLE promo_codes (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  Code VARCHAR(30) NOT NULL UNIQUE,
  Status VARCHAR(20) NOT NULL,
  ExpiresAt DATETIME NOT NULL,
  SourceTicketId BIGINT NULL UNIQUE,
  UsedByTicketId BIGINT NULL UNIQUE,
  CONSTRAINT FK_promo_source FOREIGN KEY (SourceTicketId) REFERENCES tickets(Id),
  CONSTRAINT FK_promo_used_by FOREIGN KEY (UsedByTicketId) REFERENCES tickets(Id)
);
ALTER TABLE tickets ADD CONSTRAINT FK_tickets_used_promo FOREIGN KEY (UsedPromoCodeId) REFERENCES promo_codes(Id);
CREATE TABLE ticket_items (
  Id BIGINT PRIMARY KEY AUTO_INCREMENT,
  TicketId BIGINT NOT NULL,
  MatchId BIGINT NOT NULL,
  SeatCount INT NOT NULL,
  UnitPriceEur DECIMAL(18,2) NOT NULL,
  CONSTRAINT FK_ticket_items_tickets FOREIGN KEY (TicketId) REFERENCES tickets(Id) ON DELETE CASCADE,
  CONSTRAINT FK_ticket_items_matches FOREIGN KEY (MatchId) REFERENCES matches(Id)
);

INSERT INTO championships (Id,Name,Location,StartsAt,EndsAt,Discount10Until,Description) VALUES
(1,'FIBA Svetsko prvenstvo Qatar 2027','Doha, Qatar','2027-08-27','2027-09-12','2027-05-31','Aplikacija za pregled prvenstva i kupovinu karata.');
INSERT INTO currencies (Id,Code,IsAllowed) VALUES (1,'EUR',1),(2,'USD',1),(3,'RSD',1),(4,'QAR',1);
INSERT INTO `groups` (Id,Code,ChampionshipId) VALUES (1,'Grupa A',1),(2,'Grupa B',1);
INSERT INTO teams (Id,Name,PlayerCount,Coach,Wins,Losses,Points,GroupId) VALUES
(1,'Srbija',12,'Selektor Srbije',0,0,0,1),(2,'SAD',12,'Selektor SAD',0,0,0,1),
(3,'Francuska',12,'Selektor Francuske',0,0,0,2),(4,'Kanada',12,'Selektor Kanade',0,0,0,2);
INSERT INTO stadiums (Id,Name,City,Address,Capacity) VALUES
(1,'Lusail Arena','Lusail','Lusail Boulevard',18000),(2,'Doha Arena','Doha','Sports City',12000);
INSERT INTO matches (StartsAt,BasePriceEur,StadiumId,Team1Id,Team2Id) VALUES
('2027-08-27 18:00:00',50,1,1,2),('2027-08-28 20:00:00',45,2,3,4),('2027-08-30 19:00:00',60,1,1,3);
