IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users];
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type in (N'U'))
DROP TABLE [dbo].[OrderItems];
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
DROP TABLE [dbo].[Orders];
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND type in (N'U'))
DROP TABLE [dbo].[Items];
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
DROP TABLE [dbo].[Customers];

CREATE TABLE [dbo].[Users] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NOT NULL,
    [Password] NVARCHAR (100) NOT NULL,
    [Age]      INT            NOT NULL
);


CREATE TABLE [dbo].[Orders] (
    [OrderId]    INT      IDENTITY (1, 1) NOT NULL,
    [OrderDate]  DATETIME NOT NULL,
    [CustomerId] INT      NOT NULL
);

ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([OrderId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

CREATE TABLE [dbo].[OrderItems] (
    [OrderItemId] INT IDENTITY (1, 1) NOT NULL,
    [OrderId]     INT NOT NULL,
    [ItemId]      INT NOT NULL,
    [Quantity]    INT NOT NULL
);

ALTER TABLE [dbo].[OrderItems]
    ADD CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED ([OrderItemId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

CREATE TABLE [dbo].[Items] (
    [ItemId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]   NVARCHAR (100) NOT NULL,
    [Price]  MONEY          NOT NULL
);

ALTER TABLE [dbo].[Items]
    ADD CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([ItemId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

CREATE TABLE [dbo].[Customers] (
    [CustomerId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (100) NOT NULL,
    [Address]    NVARCHAR (200) NULL
);

ALTER TABLE [dbo].[Customers]
    ADD CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([CustomerId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
    
BEGIN TRANSACTION
SET IDENTITY_INSERT [dbo].[Customers] ON
INSERT INTO [dbo].[Customers] ([CustomerId], [Name], [Address]) VALUES (1, N'ACME', N'100 Road')
INSERT INTO [dbo].[Customers] ([CustomerId], [Name], [Address]) VALUES (2, N'Zenith', N'42 Street')
SET IDENTITY_INSERT [dbo].[Customers] OFF
SET IDENTITY_INSERT [dbo].[Orders] ON
INSERT INTO [dbo].[Orders] ([OrderId], [OrderDate], [CustomerId]) VALUES (1, '20101010 00:00:00.000', 1)
SET IDENTITY_INSERT [dbo].[Orders] OFF
SET IDENTITY_INSERT [dbo].[Items] ON
INSERT INTO [dbo].[Items] ([ItemId], [Name], [Price]) VALUES (1, N'Widget', 4.5000)
SET IDENTITY_INSERT [dbo].[Items] OFF
SET IDENTITY_INSERT [dbo].[OrderItems] ON
INSERT INTO [dbo].[OrderItems] ([OrderItemId], [OrderId], [ItemId], [Quantity]) VALUES (1, 1, 1, 10)
SET IDENTITY_INSERT [dbo].[OrderItems] OFF
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT INTO [dbo].[Users] ([Id], [Name], [Password], [Age]) VALUES (1,'Bob','Secret',42)
INSERT INTO [dbo].[Users] ([Id], [Name], [Password], [Age]) VALUES (2,'Charlie','Squirrel',49)
INSERT INTO [dbo].[Users] ([Id], [Name], [Password], [Age]) VALUES (3,'Dave','Secret',42)
SET IDENTITY_INSERT [dbo].[Users] OFF

COMMIT TRANSACTION


ALTER TABLE [dbo].[Orders] WITH NOCHECK
    ADD CONSTRAINT [FK_Orders_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([CustomerId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE [dbo].[OrderItems] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderItems_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([ItemId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE [dbo].[OrderItems] WITH NOCHECK
    ADD CONSTRAINT [FK_OrderItems_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([OrderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomers]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerAndOrders]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerAndOrders]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerCount]
GO
CREATE PROCEDURE [dbo].[GetCustomers]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Customers
    ORDER BY CustomerId
END
GO
CREATE PROCEDURE [dbo].[GetCustomerAndOrders] (@CustomerId int)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Customers WHERE CustomerId = @CustomerId;
    SELECT * FROM Orders WHERE CustomerId = @CustomerId;
END
GO
CREATE PROCEDURE [dbo].[GetCustomerCount]
AS
BEGIN
    SET NOCOUNT ON;
    RETURN 1
END
