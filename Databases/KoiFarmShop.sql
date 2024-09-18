CREATE DATABASE [FA24_SE1717_PRN231_G1_KOIFARMSHOP]
GO
USE [FA24_SE1717_PRN231_G1_KOIFARMSHOP]
GO

ALTER DATABASE KoiFarmShop COLLATE Vietnamese_CI_AS;

CREATE TABLE [Token](
  [UserId] [varchar] (50)  PRIMARY KEY NOT NULL,
  [AccessToken] [varchar] (max) NULL,
  [RefreshToken] [varchar] (max) NULL,
  [ExpiredTime] [datetime] NULL,
  [Status] [int] NOT NULL,
)

CREATE TABLE [User] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [UserId] [varchar] (50) UNIQUE NOT NULL, --USER0001
  [FullName] [varchar](max) NOT NULL,
  [UserName] [nvarchar](50) NOT NULL,
  [Password] [varchar] (max) NOT NULL,
  [Email] [varchar] (max) NOT NULL,
  [Phone] [NVARCHAR](10) NOT NULL,
  [Role] [int] NOT NULL, -- 1:Customer 2:Staff 3:Manager
  [Status] [int] NOT NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [varchar] (max) NULL,
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL,
  [DeletedDate] [datetime] NULL,
  [DeletedBy] [varchar] (max) NULL,
);

CREATE TABLE [KoiFish] (
  [Id] INT IDENTITY(1,1) PRIMARY KEY, 
  [KoiId] [varchar] (50) UNIQUE NOT NULL, -- KOI0001
  [KoiName] [NVARCHAR] (255) NOT NULL, 
  [Origin] [NVARCHAR](255) NOT NULL,
  [Gender] [NVARCHAR](50) NOT NULL CHECK ([Gender] IN ('Male', 'Female')),
  [Age] [INT] NOT NULL,
  [Size] [FLOAT] NOT NULL,
  [Breed] [NVARCHAR] (255) NOT NULL, --Giong loai
  [Type] [NVARCHAR] (50) NOT NULL CHECK ([Type] IN ('Imported Purebred', 'F1', 'Vietnamese Purebred')), --  'Thuần chủng nhập khẩu', 'F1', 'Thuần Việt'
  [Price] [float] NOT NULL,
  [Quantity] [INT] NOT NULL,
  [OwnerType] [int] NOT NULL, --1: Store-owned, 2: Consigned 
  [Description] [NVARCHAR] (max) NOT NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [varchar] (max) NULL,
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL,
  [DeletedDate] [datetime] NULL,
  [DeletedBy] [varchar] (max) NULL,
);

CREATE TABLE [Consignment] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [ConsignmentId] [varchar] (50) UNIQUE NOT NULL, --CONSIGN0001
  [UserId] [varchar] (50) NOT NULL,
  [KoiId] [varchar] (50) NOT NULL,
  [Type] [INT] NOT NULL, --1: Care 2:Sale
  [DealPrice] [float] NULL,
  [Method] [varchar] (50) NOT NULL CHECK ([Method] IN ('Online', 'Offline')),
  [PaymentId] [varchar] (50) NOT NULL,
  [Status] [int] NOT NULL, --1:Pending, 2:Agreed, 3: In store, 4:Sold, 5:Return 6:Cancel
  [ConsignmentDate] [date] NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [varchar] (max) NULL, /*name người tạo*/
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL,    /*name người chỉnh sửa*/
);


CREATE TABLE [Order] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [OrderId] [varchar] (50) UNIQUE NOT NULL, --ORDER0001
  [UserId] [varchar] (50) NOT NULL,
  [PaymentId] [varchar] (50) NOT NULL,
  [TotalAmount] [float] NOT NULL,
  [Quantity] [int] NOT NULL,
  [Status] [int] NOT NULL,
  [VoucherId] [varchar] (50) NOT NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [varchar] (max) NULL,
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL,
);

CREATE TABLE [OrderDetail] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [OrderId] [varchar]  (50) NOT NULL,
  [KoiId] [varchar] (50) NOT NULL,
  [Price] [float] NOT NULL,
  [Quantity] [int] NOT NULL,
);

CREATE TABLE [Payment] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [PaymentId] [varchar] (50) UNIQUE NOT NULL, --TRANS0001
  [UserId] [varchar] (50) NOT NULL,
  [Amount] [float] NOT NULL,
  [Type] [int] NOT NULL, 
  [Status] [int] NOT NULL, --1:Success -- 2:Pending -- 3:Faild
  [CreatedDate] [datetime] NULL,
);

CREATE TABLE [Image] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [ImageId] [varchar] (50) UNIQUE NOT NULL, --IMG0001
  [KoiId] [varchar] (50) NOT NULL,
  [URL] [varchar] (max) NULL,
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [varchar] (max) NULL, /*name người tạo*/
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL,    /*name người chỉnh sửa*/
  [DeletedDate] [datetime] NULL,
  [DeletedBy] [varchar] (max) NULL /*name người xóa*/
);

CREATE TABLE [Blog] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [BlogId] [varchar] (50) UNIQUE NOT NULL, --BLOG0001
  [UserId] [varchar] (50) NOT NULL,
  [Title] [NVARCHAR] (255) NOT NULL,
  Content [TEXT] NOT NULL,
  [CreatedDate] [date] NULL,
  [CreatedBy] [varchar] (max) NULL, /*name người tạo*/
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL,    /*name người chỉnh sửa*/
  [DeletedDate] [datetime] NULL,
  [DeletedBy] [varchar] (max) NULL /*name người xóa*/
);

CREATE TABLE [Voucher] (
  [Id] [int] IDENTITY(1,1) PRIMARY KEY,
  [VoucherId] [varchar] (50) UNIQUE NOT NULL, -- VOUCH0001
  [VoucherCode] [varchar] (50) UNIQUE NOT NULL, -- Mã voucher
  [DiscountAmount] [float] NOT NULL, -- Số tiền giảm giá
  [MinOrderAmount] [float] NOT NULL, -- Số tiền tối thiểu của đơn hàng
  [Status] [int] NOT NULL, -- 1 Active 2 Inactive
  [ValidityStartDate] [datetime] NULL, -- Ngày bắt đầu áp dụng voucher
  [ValidityEndDate] [datetime] NULL, -- Ngày kết thúc áp dụng voucher
  [CreatedDate] [datetime] NULL,
  [CreatedBy] [varchar] (max) NULL,
  [ModifiedDate] [datetime] NULL,
  [ModifiedBy] [varchar] (max) NULL
);

GO
-----------------------------------------------------------------------------------------------------------------------------------

--OrderDetail and Order
ALTER TABLE [OrderDetail] ADD CONSTRAINT FK_OrderDetail_Order FOREIGN KEY ([OrderId]) REFERENCES [Order]([OrderId]);

--OrderDetail and KoiFish
ALTER TABLE [OrderDetail] ADD CONSTRAINT FK_OrderDetail_KoiFish FOREIGN KEY ([KoiId]) REFERENCES [KoiFish]([KoiId]);

--User and Transaction
ALTER TABLE [Payment] ADD CONSTRAINT FK_Payment_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]);

--Payment and Order
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_Payment FOREIGN KEY ([PaymentId]) REFERENCES [Payment]([PaymentId]);

--Consignment and bảng Payment
ALTER TABLE [Consignment] ADD CONSTRAINT FK_Consignment_Payment FOREIGN KEY ([PaymentId]) REFERENCES [Payment]([PaymentId]);

--Image and KoiFish
ALTER TABLE [Image] ADD CONSTRAINT FK_Image_KoiFish FOREIGN KEY ([KoiId]) REFERENCES [KoiFish]([KoiId]);

--Blog and User
ALTER TABLE [Blog] ADD CONSTRAINT FK_Blog_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]);

--Consignment and KoiFish
ALTER TABLE [Consignment] ADD CONSTRAINT FK_Consignment_KoiFish FOREIGN KEY ([KoiId]) REFERENCES [KoiFish]([KoiId]);

--Consignment and User
ALTER TABLE [Consignment] ADD CONSTRAINT FK_Consignment_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]);

--Order and User
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId]);

--Voucher and Order
ALTER TABLE [Order] ADD CONSTRAINT FK_Order_Voucher FOREIGN KEY ([VoucherId]) REFERENCES [Voucher]([VoucherId]);
