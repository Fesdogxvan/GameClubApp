IF DB_ID('GameClub') IS NOT NULL
BEGIN
    ALTER DATABASE GameClub SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GameClub;
END
GO

CREATE DATABASE GameClub;
GO

USE GameClub;
GO

-- 1. Сотрудники клуба
CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Login NVARCHAR(30) NOT NULL UNIQUE,
    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleUser VARCHAR(20) NOT NULL
        CONSTRAINT CK_Users_Role CHECK (RoleUser IN ('Admin', 'Employee')),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- 2 Посетители клуба
CREATE TABLE Customers
(
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    RegistrationDate DATE NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- 3 Рабочие места
CREATE TABLE Workstations
(
    WorkstationID INT IDENTITY(1,1) PRIMARY KEY,
    InventoryNumber VARCHAR(20) NOT NULL UNIQUE,
    Name NVARCHAR(50) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'Свободен'
        CONSTRAINT CK_Workstations_Status
        CHECK (Status IN (N'Свободен', N'Занят', N'Обслуживание')),
    Location NVARCHAR(50) NULL
);
GO

-- 4 Справочник типов комплектующих
CREATE TABLE ComponentTypes
(
    TypeID   INT IDENTITY(1,1) PRIMARY KEY,
    TypeName NVARCHAR(30) NOT NULL UNIQUE
);
GO

-- 5 Комплектующие
CREATE TABLE Components
(
    ComponentID INT IDENTITY(1,1) PRIMARY KEY,
    TypeID INT NOT NULL,
    Name NVARCHAR(100)  NOT NULL,
    InventoryNumber VARCHAR(30) NOT NULL UNIQUE,
    PerformanceLevel INT NOT NULL DEFAULT 1,
    Price DECIMAL(10,2) NOT NULL DEFAULT 0,
    IsAvailable BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Components_ComponentTypes
        FOREIGN KEY (TypeID) REFERENCES ComponentTypes(TypeID)
);
GO

CREATE INDEX IX_Components_TypeID ON Components(TypeID);
GO

-- 6 Конфигурация ПК
CREATE TABLE WorkstationComponents
(
    WorkstationComponentID INT IDENTITY(1,1) PRIMARY KEY,
    WorkstationID INT  NOT NULL,
    ComponentID INT  NOT NULL,
    InstalledDate DATE NOT NULL DEFAULT GETDATE(),
    RemovedDate DATE NULL,

    CONSTRAINT FK_WC_Workstations
        FOREIGN KEY (WorkstationID) REFERENCES Workstations(WorkstationID),
    CONSTRAINT FK_WC_Components
        FOREIGN KEY (ComponentID)   REFERENCES Components(ComponentID)
);
GO

CREATE INDEX IX_WC_WorkstationID ON WorkstationComponents(WorkstationID);
CREATE INDEX IX_WC_ComponentID   ON WorkstationComponents(ComponentID);
GO

-- 7 Тарифы
CREATE TABLE Tariffs
(
    TariffID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    PricePerHour DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- 8. Каталог игр
CREATE TABLE Games
(
    GameID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Genre NVARCHAR(50)  NULL,
    MinPerformanceLevel INT NOT NULL DEFAULT 1,
    IsAvailable BIT NOT NULL DEFAULT 1
);
GO

-- 9 Игровые сессии
CREATE TABLE Sessions
(
    SessionID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    UserID INT NULL,
    WorkstationID INT NOT NULL,
    TariffID INT NOT NULL,
    GameID INT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NULL,
    Cost DECIMAL(10,2)  NULL,
    Status NVARCHAR(20)   NOT NULL DEFAULT N'Активна'
        CONSTRAINT CK_Sessions_Status
        CHECK (Status IN (N'Активна', N'Завершена', N'Отменена')),

    CONSTRAINT FK_Sessions_Customers
        FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    CONSTRAINT FK_Sessions_Users
        FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Sessions_Workstations
        FOREIGN KEY (WorkstationID) REFERENCES Workstations(WorkstationID),
    CONSTRAINT FK_Sessions_Tariffs
        FOREIGN KEY (TariffID) REFERENCES Tariffs(TariffID),
    CONSTRAINT FK_Sessions_Games
        FOREIGN KEY (GameID) REFERENCES Games(GameID)
);
GO

CREATE INDEX IX_Sessions_CustomerID    ON Sessions(CustomerID);
CREATE INDEX IX_Sessions_WorkstationID ON Sessions(WorkstationID);
CREATE INDEX IX_Sessions_Status        ON Sessions(Status);
CREATE INDEX IX_Sessions_StartTime     ON Sessions(StartTime);
GO
