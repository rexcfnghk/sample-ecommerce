DROP DATABASE IF EXISTS SampleECommerceDb

CREATE DATABASE SampleECommerceDb

CREATE TABLE Users (
    Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    EncryptedPassword VARBINARY(128) NOT NULL,
    Salt VARBINARY(16) NOT NULL,
    Balance SMALLMONEY NOT NULL DEFAULT (100.00)

    CONSTRAINT AK_Users_UserNameId UNIQUE(UserName)
    CONSTRAINT AK_Users_Balance CHECK (Balance >= 0)
)

CREATE TABLE Products(
    Id NVARCHAR(50) NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Price SMALLMONEY NOT NULL,
    Quantity INT NOT NULL,
    Category NVARCHAR(50) NOT NULL,

    CONSTRAINT AK_Products_Quantity CHECK (Quantity >= 0),
    CONSTRAINT AK_Products_Price CHECK (Price > 0)
)

CREATE TABLE Orders
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT(NEWID()),
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    OrderTime DATETIMEOFFSET NOT NULL
)

CREATE TABLE OrderItems (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT(NEWID()),
    OrderId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Orders(Id),
    ProductId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Products(Id),
    Quantity INT NOT NULL,

    CONSTRAINT AK_Orders_Quantity CHECK (Quantity > 0)
)

INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p1','Product 1',10.18,26,'Gadgets')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p2','Product 2',17.21,21,'Gadgets')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p3','Product 3',6.92,6,'Home')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p4','Product 4',21.16,10,'Clothing')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p5','Product 5',11.46,0,'Clothing')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p6','Product 6',6.46,24,'Gadgets')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p7','Product 7',31.56,18,'Electronics')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p8','Product 8',9.68,5,'Gadgets')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p9','Product 9',13.37,2,'Clothing')
INSERT INTO Products (Id, Name, Price, Quantity, Category) VALUES ('p10','Product 10',1.01,1,'Electronics')

GO
