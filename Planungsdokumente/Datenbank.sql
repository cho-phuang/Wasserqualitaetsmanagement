CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(255) NOT NULL, 
    LastLogin DATETIME
);

CREATE TABLE Pools (
    PoolID INT PRIMARY KEY AUTO_INCREMENT,
    PoolName VARCHAR(100) NOT NULL,
    Location VARCHAR(100),
    UserID INT, -- Fremdschlüssel zum User
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE WaterQualityData (
    DataID INT PRIMARY KEY AUTO_INCREMENT,
    UserID INT, -- Fremdschlüssel zum Benutzer
    PoolID INT, -- Fremdschlüssel zum Schwimmbad
    PHValue DOUBLE,
    Temperature DOUBLE,
    ChlorineLevel DOUBLE,
    Turbidity DOUBLE,
    EntryDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (PoolID) REFERENCES Pools(PoolID)
);
