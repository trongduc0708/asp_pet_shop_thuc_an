/*
 Navicat Premium Dump SQL

 Source Server         : sql_server
 Source Server Type    : SQL Server
 Source Server Version : 16001000 (16.00.1000)
 Source Host           : localhost:1433
 Source Catalog        : PetShopDB_10_2025
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 16001000 (16.00.1000)
 File Encoding         : 65001

 Date: 18/11/2025 00:44:18
*/


-- ----------------------------
-- Table structure for Addresses
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND type IN ('U'))
	DROP TABLE [dbo].[Addresses]
GO

CREATE TABLE [dbo].[Addresses] (
  [AddressID] int  IDENTITY(1,1) NOT NULL,
  [UserID] int  NOT NULL,
  [FullName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Phone] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Address] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Ward] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [District] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [City] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IsDefault] bit DEFAULT 0 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Addresses] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Addresses
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Addresses] ON
GO

INSERT INTO [dbo].[Addresses] ([AddressID], [UserID], [FullName], [Phone], [Address], [Ward], [District], [City], [IsDefault], [CreatedDate]) VALUES (N'1', N'1', N'Nguy?n Van A', N'0901234567', N'123 Ðu?ng ABC', N'Phu?ng 1', N'Qu?n 1', N'TP. H? Chí Minh', N'0', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Addresses] ([AddressID], [UserID], [FullName], [Phone], [Address], [Ward], [District], [City], [IsDefault], [CreatedDate]) VALUES (N'3', N'2', N'Tr?n Th? B', N'0987654321', N'789 Ðu?ng DEF', N'Phu?ng 3', N'Qu?n 3', N'TP. H? Chí Minh', N'0', N'2025-10-15 21:30:37.410')
GO

SET IDENTITY_INSERT [dbo].[Addresses] OFF
GO


-- ----------------------------
-- Table structure for Banners
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Banners]') AND type IN ('U'))
	DROP TABLE [dbo].[Banners]
GO

CREATE TABLE [dbo].[Banners] (
  [BannerID] int  IDENTITY(1,1) NOT NULL,
  [BannerName] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ImageURL] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [LinkURL] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AltText] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Position] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SortOrder] int DEFAULT 0 NULL,
  [StartDate] datetime  NULL,
  [EndDate] datetime  NULL,
  [IsActive] bit DEFAULT 1 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Banners] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Banners
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Banners] ON
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'1', N'Banner chính trang chủ', N'/uploads/banners/f4c1e9f4-b707-4792-a548-999f2dfe6bb6.jpeg', N'/products', N'Khuyến mãi thức ăn thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2025-10-31 00:00:00.000', N'0', N'2025-10-15 21:30:37.000')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'2', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:37.430')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'3', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:37.430')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'4', N'Banner thức an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Thức ăn cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:37.000')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'5', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:37.430')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'6', N'Banner chính trang chủ chính', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuyến mãi trang chủ', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:45.000')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'7', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:45.050')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'8', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:45.050')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'9', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:45.050')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'10', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:45.050')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'11', N'Banner chính trang chủ', N'/uploads/banners/24b9d335-1071-431c-889a-59168e0bd569.jpeg', N'/products', N'Khuyến mãi thức an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:57.000')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'12', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:57.333')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'13', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:57.333')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'14', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:57.333')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'15', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:57.333')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'16', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:31:38.490')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'17', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:31:38.490')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'18', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:31:38.490')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'19', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:31:38.490')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'20', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:31:38.490')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'21', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:03.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'22', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:03.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'23', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:03.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'24', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:03.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'25', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:03.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'26', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:17.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'27', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:17.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'28', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:17.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'29', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:17.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'30', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:33:17.030')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'31', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:37:29.933')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'32', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:37:29.933')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'33', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:37:29.933')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'34', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:37:29.933')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'35', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:37:29.933')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'36', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:38:44.063')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'37', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:38:44.063')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'38', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:38:44.063')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'39', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:38:44.063')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'40', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:38:44.063')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'41', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:17.823')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'42', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:17.823')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'43', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:17.823')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'44', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:17.823')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'45', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:17.823')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'46', N'Banner chính trang ch?', N'/images/banners/homepage-banner-1.jpg', N'/products', N'Khuy?n mãi th?c an thú cung', N'Homepage', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:57.663')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'47', N'Banner s?n ph?m m?i', N'/images/banners/new-products-banner.jpg', N'/products?filter=new', N'S?n ph?m m?i cho thú cung', N'Homepage', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:57.663')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'48', N'Banner khuy?n mãi', N'/images/banners/promotion-banner.jpg', N'/promotions', N'Gi?m giá lên d?n 50%', N'Homepage', N'3', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:57.663')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'49', N'Banner th?c an chó', N'/images/banners/dog-food-banner.jpg', N'/category/1', N'Th?c an cho chó', N'Category', N'1', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:57.663')
GO

INSERT INTO [dbo].[Banners] ([BannerID], [BannerName], [ImageURL], [LinkURL], [AltText], [Position], [SortOrder], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'50', N'Banner th?c an mèo', N'/images/banners/cat-food-banner.jpg', N'/category/2', N'Th?c an cho mèo', N'Category', N'2', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:39:57.663')
GO

SET IDENTITY_INSERT [dbo].[Banners] OFF
GO


-- ----------------------------
-- Table structure for Brands
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Brands]') AND type IN ('U'))
	DROP TABLE [dbo].[Brands]
GO

CREATE TABLE [dbo].[Brands] (
  [BrandID] int  IDENTITY(1,1) NOT NULL,
  [BrandName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [LogoURL] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Description] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IsActive] bit DEFAULT 1 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Brands] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Brands
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Brands] ON
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'46', N'Royal Canin', NULL, N'Thuong hi?u th?c an cao c?p cho thú cung', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'47', N'Whiskas', NULL, N'Th?c an cho mèo', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'48', N'Pedigree', NULL, N'Th?c an cho chó', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'49', N'Felix', NULL, N'Th?c an u?t cho mèo', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'50', N'Hill''s', NULL, N'Th?c an dinh du?ng cho thú cung', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'51', N'Purina', NULL, N'Th?c an da d?ng cho thú cung', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'52', N'Orijen', NULL, N'Th?c an cao c?p cho thú cung', N'0', N'2025-10-15 21:39:57.657')
GO

INSERT INTO [dbo].[Brands] ([BrandID], [BrandName], [LogoURL], [Description], [IsActive], [CreatedDate]) VALUES (N'53', N'Acana', NULL, N'Th?c an t? nhiên cho thú cung', N'0', N'2025-10-15 21:39:57.657')
GO

SET IDENTITY_INSERT [dbo].[Brands] OFF
GO


-- ----------------------------
-- Table structure for Cart
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Cart]') AND type IN ('U'))
	DROP TABLE [dbo].[Cart]
GO

CREATE TABLE [dbo].[Cart] (
  [CartID] int  IDENTITY(1,1) NOT NULL,
  [UserID] int  NOT NULL,
  [ProductID] int  NOT NULL,
  [Quantity] int DEFAULT 1 NOT NULL,
  [AddedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Cart] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Cart
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Cart] ON
GO

INSERT INTO [dbo].[Cart] ([CartID], [UserID], [ProductID], [Quantity], [AddedDate]) VALUES (N'21', N'2', N'60', N'2', N'2024-01-21 14:30:00.000')
GO

INSERT INTO [dbo].[Cart] ([CartID], [UserID], [ProductID], [Quantity], [AddedDate]) VALUES (N'22', N'2', N'64', N'5', N'2024-01-21 14:35:00.000')
GO

INSERT INTO [dbo].[Cart] ([CartID], [UserID], [ProductID], [Quantity], [AddedDate]) VALUES (N'37', N'1', N'64', N'1', N'2025-11-04 00:44:16.770')
GO

INSERT INTO [dbo].[Cart] ([CartID], [UserID], [ProductID], [Quantity], [AddedDate]) VALUES (N'38', N'1', N'60', N'1', N'2025-11-04 00:44:19.493')
GO

INSERT INTO [dbo].[Cart] ([CartID], [UserID], [ProductID], [Quantity], [AddedDate]) VALUES (N'39', N'1', N'68', N'1', N'2025-11-04 00:44:25.307')
GO

INSERT INTO [dbo].[Cart] ([CartID], [UserID], [ProductID], [Quantity], [AddedDate]) VALUES (N'40', N'1', N'62', N'1', N'2025-11-04 00:44:59.610')
GO

SET IDENTITY_INSERT [dbo].[Cart] OFF
GO


-- ----------------------------
-- Table structure for Categories
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type IN ('U'))
	DROP TABLE [dbo].[Categories]
GO

CREATE TABLE [dbo].[Categories] (
  [CategoryID] int  IDENTITY(1,1) NOT NULL,
  [CategoryName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ParentCategoryID] int  NULL,
  [Description] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IsActive] bit DEFAULT 1 NULL,
  [SortOrder] int DEFAULT 0 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Categories] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Categories
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Categories] ON
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'1', N'Thức ăn chó', NULL, N'Các loại thức ăn  dành cho chó', N'0', N'0', N'2025-10-06 09:24:34.493')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'2', N'Thức ăn mèo', NULL, N'Các loại thức ăn dành cho mèo', N'0', N'0', N'2025-10-06 09:24:34.493')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'3', N'Phụ kiện chó', N'4', N'Dây dắt, chuồng, đồ chơi cho chó', N'0', N'0', N'2025-10-06 09:24:34.000')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'4', N'Phụ kiện mèo', NULL, N'Khay cát, vòng cổ, đồ chơi cho mèo', N'0', N'0', N'2025-10-06 09:24:34.493')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'5', N'Th?c an chó', NULL, N'Các lo?i th?c an dành cho chó', N'0', N'1', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'6', N'Th?c an mèo', NULL, N'Các lo?i th?c an dành cho mèo', N'0', N'2', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'7', N'Ph? ki?n chó', NULL, N'Dây d?t, chu?ng, d? choi cho chó', N'0', N'3', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'8', N'Ph? ki?n mèo', NULL, N'Khay cát, vòng c?, d? choi cho mèo', N'0', N'4', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'9', N'Th?c an khô', NULL, N'Th?c an khô cho thú cung', N'0', N'5', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'10', N'Th?c an u?t', NULL, N'Th?c an u?t cho thú cung', N'0', N'6', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'11', N'Ð? choi', NULL, N'Ð? choi cho thú cung', N'0', N'7', N'2025-10-15 21:30:37.410')
GO

INSERT INTO [dbo].[Categories] ([CategoryID], [CategoryName], [ParentCategoryID], [Description], [IsActive], [SortOrder], [CreatedDate]) VALUES (N'12', N'V? sinh', NULL, N'S?n ph?m v? sinh cho thú cung', N'0', N'8', N'2025-10-15 21:30:37.410')
GO

SET IDENTITY_INSERT [dbo].[Categories] OFF
GO


-- ----------------------------
-- Table structure for ContactMessages
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactMessages]') AND type IN ('U'))
	DROP TABLE [dbo].[ContactMessages]
GO

CREATE TABLE [dbo].[ContactMessages] (
  [MessageID] int  IDENTITY(1,1) NOT NULL,
  [FullName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Email] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Phone] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Subject] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Message] nvarchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Status] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT 'New' NULL,
  [ReplyMessage] nvarchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [RepliedBy] int  NULL,
  [RepliedDate] datetime  NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[ContactMessages] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of ContactMessages
-- ----------------------------
SET IDENTITY_INSERT [dbo].[ContactMessages] ON
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'1', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'2', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'3', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'4', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'5', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'6', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'7', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'8', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'9', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'10', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'11', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'12', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'13', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'14', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'15', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'16', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'17', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'18', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'19', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'20', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'21', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'22', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'23', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'24', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'25', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'26', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'27', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'28', N'Nguy?n Van C', N'nguyenvanc@email.com', N'0912345678', N'H?i v? s?n ph?m', N'Tôi mu?n h?i v? th?c an cho chó con, có s?n ph?m nào phù h?p không?', N'New', NULL, NULL, NULL, N'2024-01-20 09:00:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'29', N'Tr?n Th? D', N'tranthid@email.com', N'0987654321', N'Khi?u n?i', N'Tôi dã d?t hàng nhung chua nh?n du?c, có th? ki?m tra giúp tôi không?', N'Read', NULL, NULL, NULL, N'2024-01-19 15:30:00.000')
GO

INSERT INTO [dbo].[ContactMessages] ([MessageID], [FullName], [Email], [Phone], [Subject], [Message], [Status], [ReplyMessage], [RepliedBy], [RepliedDate], [CreatedDate]) VALUES (N'30', N'Lê Van E', N'levane@email.com', N'0909876543', N'Tu v?n', N'Tôi có con mèo 6 tháng tu?i, nên cho an th?c an gì?', N'Replied', NULL, NULL, NULL, N'2024-01-18 11:20:00.000')
GO

SET IDENTITY_INSERT [dbo].[ContactMessages] OFF
GO


-- ----------------------------
-- Table structure for CustomerProfiles
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerProfiles]') AND type IN ('U'))
	DROP TABLE [dbo].[CustomerProfiles]
GO

CREATE TABLE [dbo].[CustomerProfiles] (
  [ProfileID] int  IDENTITY(1,1) NOT NULL,
  [UserID] int  NOT NULL,
  [DateOfBirth] date  NULL,
  [Gender] nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [TotalOrders] int DEFAULT 0 NULL,
  [TotalSpent] decimal(15,2) DEFAULT 0 NULL,
  [MembershipLevel] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT 'Bronze' NULL,
  [Points] int DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[CustomerProfiles] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of CustomerProfiles
-- ----------------------------
SET IDENTITY_INSERT [dbo].[CustomerProfiles] ON
GO

INSERT INTO [dbo].[CustomerProfiles] ([ProfileID], [UserID], [DateOfBirth], [Gender], [TotalOrders], [TotalSpent], [MembershipLevel], [Points]) VALUES (N'1', N'1', N'2025-10-15', N'Nam', N'20', N'14700000.00', N'Bronze', N'1470')
GO

INSERT INTO [dbo].[CustomerProfiles] ([ProfileID], [UserID], [DateOfBirth], [Gender], [TotalOrders], [TotalSpent], [MembershipLevel], [Points]) VALUES (N'2', N'2', N'2025-10-09', N'Nam', N'20', N'16250000.00', N'Bronze', N'1620')
GO

SET IDENTITY_INSERT [dbo].[CustomerProfiles] OFF
GO


-- ----------------------------
-- Table structure for Inventory
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Inventory]') AND type IN ('U'))
	DROP TABLE [dbo].[Inventory]
GO

CREATE TABLE [dbo].[Inventory] (
  [InventoryID] int  IDENTITY(1,1) NOT NULL,
  [ProductID] int  NOT NULL,
  [QuantityInStock] int DEFAULT 0 NOT NULL,
  [MinStockLevel] int DEFAULT 10 NULL,
  [MaxStockLevel] int DEFAULT 1000 NULL,
  [LastUpdated] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Inventory] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Inventory
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Inventory] ON
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'43', N'60', N'46', N'10', N'100', N'2025-10-27 10:01:00.313')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'44', N'61', N'78', N'15', N'150', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'45', N'62', N'25', N'5', N'80', N'2025-10-29 23:02:39.740')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'46', N'63', N'98', N'20', N'200', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'47', N'64', N'185', N'50', N'500', N'2025-11-01 22:10:54.987')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'48', N'65', N'39', N'8', N'100', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'49', N'66', N'23', N'5', N'50', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'50', N'67', N'9', N'2', N'20', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'51', N'68', N'157', N'10', N'100', N'2025-11-01 22:14:47.557')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'52', N'69', N'14', N'3', N'30', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'53', N'70', N'40', N'8', N'80', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[Inventory] ([InventoryID], [ProductID], [QuantityInStock], [MinStockLevel], [MaxStockLevel], [LastUpdated]) VALUES (N'54', N'71', N'20', N'4', N'40', N'2025-10-15 21:39:57.660')
GO

SET IDENTITY_INSERT [dbo].[Inventory] OFF
GO


-- ----------------------------
-- Table structure for InventoryTransactions
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryTransactions]') AND type IN ('U'))
	DROP TABLE [dbo].[InventoryTransactions]
GO

CREATE TABLE [dbo].[InventoryTransactions] (
  [TransactionID] int  IDENTITY(1,1) NOT NULL,
  [ProductID] int  NOT NULL,
  [TransactionType] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Quantity] int  NOT NULL,
  [UnitPrice] decimal(15,2)  NULL,
  [TotalValue] decimal(15,2)  NULL,
  [ReferenceNumber] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Notes] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CreatedBy] int  NOT NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[InventoryTransactions] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of InventoryTransactions
-- ----------------------------
SET IDENTITY_INSERT [dbo].[InventoryTransactions] ON
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'43', N'60', N'Import', N'50', N'300000.00', N'15000000.00', N'IMP-001', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'44', N'61', N'Import', N'80', N'120000.00', N'9600000.00', N'IMP-002', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'45', N'62', N'Import', N'30', N'250000.00', N'7500000.00', N'IMP-003', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'46', N'63', N'Import', N'100', N'80000.00', N'8000000.00', N'IMP-004', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'47', N'64', N'Import', N'200', N'15000.00', N'3000000.00', N'IMP-005', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'48', N'65', N'Import', N'40', N'200000.00', N'8000000.00', N'IMP-006', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'49', N'66', N'Import', N'25', N'80000.00', N'2000000.00', N'IMP-007', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'50', N'67', N'Import', N'10', N'500000.00', N'5000000.00', N'IMP-008', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'51', N'68', N'Import', N'60', N'40000.00', N'2400000.00', N'IMP-009', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'52', N'69', N'Import', N'15', N'120000.00', N'1800000.00', N'IMP-010', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'53', N'70', N'Import', N'40', N'25000.00', N'1000000.00', N'IMP-011', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'54', N'71', N'Import', N'20', N'100000.00', N'2000000.00', N'IMP-012', N'Nh?p hàng l?n d?u', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'55', N'60', N'Export', N'1', N'400000.00', N'400000.00', N'ORDER_20251015223222', N'Bán hàng', N'1', N'2025-10-15 22:32:22.557')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'56', N'60', N'Export', N'1', N'400000.00', N'400000.00', N'ORDER_20251015223605', N'Bán hàng', N'1', N'2025-10-15 22:36:05.050')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'57', N'62', N'Export', N'1', N'350000.00', N'350000.00', N'ORDER_20251027100100', N'Bán hàng', N'1', N'2025-10-27 10:01:00.217')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'58', N'60', N'Export', N'1', N'400000.00', N'400000.00', N'ORDER_20251027100100', N'Bán hàng', N'1', N'2025-10-27 10:01:00.313')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'59', N'68', N'Export', N'1', N'60000.00', N'60000.00', N'ORDER_20251029215348', N'Bán hàng', N'1', N'2025-10-29 21:53:48.967')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'60', N'62', N'Export', N'1', N'350000.00', N'350000.00', N'ORDER_20251029215643', N'Bán hàng', N'1', N'2025-10-29 21:56:43.393')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'61', N'62', N'Export', N'1', N'350000.00', N'350000.00', N'ORDER_20251029220439', N'Bán hàng', N'1', N'2025-10-29 22:04:39.563')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'62', N'68', N'Export', N'1', N'60000.00', N'60000.00', N'ORDER_20251029220541', N'Bán hàng', N'1', N'2025-10-29 22:05:41.680')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'63', N'62', N'Export', N'1', N'350000.00', N'350000.00', N'ORDER_20251029230239', N'Bán hàng', N'1', N'2025-10-29 23:02:39.747')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'64', N'64', N'Export', N'5', N'20000.00', N'100000.00', N'ORDER_20251101221054', N'Bán hàng', N'1', N'2025-11-01 22:10:55.000')
GO

INSERT INTO [dbo].[InventoryTransactions] ([TransactionID], [ProductID], [TransactionType], [Quantity], [UnitPrice], [TotalValue], [ReferenceNumber], [Notes], [CreatedBy], [CreatedDate]) VALUES (N'65', N'68', N'Import', N'100', N'2000.00', N'200000.00', N'agdgakjd', NULL, N'3', N'2025-11-01 22:14:47.557')
GO

SET IDENTITY_INSERT [dbo].[InventoryTransactions] OFF
GO


-- ----------------------------
-- Table structure for OrderItems
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type IN ('U'))
	DROP TABLE [dbo].[OrderItems]
GO

CREATE TABLE [dbo].[OrderItems] (
  [OrderItemID] int  IDENTITY(1,1) NOT NULL,
  [OrderID] int  NOT NULL,
  [ProductID] int  NOT NULL,
  [Quantity] int  NOT NULL,
  [UnitPrice] decimal(15,2)  NOT NULL,
  [TotalPrice] decimal(15,2)  NOT NULL
)
GO

ALTER TABLE [dbo].[OrderItems] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of OrderItems
-- ----------------------------
SET IDENTITY_INSERT [dbo].[OrderItems] ON
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'37', N'1', N'60', N'1', N'400000.00', N'400000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'38', N'1', N'63', N'2', N'100000.00', N'200000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'39', N'2', N'61', N'2', N'150000.00', N'300000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'40', N'2', N'64', N'10', N'20000.00', N'200000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'41', N'2', N'67', N'1', N'700000.00', N'700000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'42', N'3', N'62', N'1', N'350000.00', N'350000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'43', N'3', N'66', N'2', N'120000.00', N'240000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'44', N'3', N'68', N'1', N'60000.00', N'60000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'45', N'4', N'65', N'1', N'280000.00', N'280000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'46', N'4', N'69', N'1', N'180000.00', N'180000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'47', N'13', N'60', N'1', N'400000.00', N'400000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'48', N'14', N'60', N'1', N'400000.00', N'400000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'49', N'15', N'62', N'1', N'350000.00', N'350000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'50', N'15', N'60', N'1', N'400000.00', N'400000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'51', N'16', N'68', N'1', N'60000.00', N'60000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'52', N'17', N'62', N'1', N'350000.00', N'350000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'53', N'18', N'62', N'1', N'350000.00', N'350000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'54', N'19', N'68', N'1', N'60000.00', N'60000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'55', N'20', N'62', N'1', N'350000.00', N'350000.00')
GO

INSERT INTO [dbo].[OrderItems] ([OrderItemID], [OrderID], [ProductID], [Quantity], [UnitPrice], [TotalPrice]) VALUES (N'56', N'21', N'64', N'5', N'20000.00', N'100000.00')
GO

SET IDENTITY_INSERT [dbo].[OrderItems] OFF
GO


-- ----------------------------
-- Table structure for Orders
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type IN ('U'))
	DROP TABLE [dbo].[Orders]
GO

CREATE TABLE [dbo].[Orders] (
  [OrderID] int  IDENTITY(1,1) NOT NULL,
  [OrderNumber] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [UserID] int  NOT NULL,
  [OrderDate] datetime DEFAULT getdate() NULL,
  [StatusID] int  NOT NULL,
  [PaymentMethodID] int  NOT NULL,
  [SubTotal] decimal(15,2)  NOT NULL,
  [ShippingFee] decimal(15,2) DEFAULT 0 NULL,
  [DiscountAmount] decimal(15,2) DEFAULT 0 NULL,
  [TotalAmount] decimal(15,2)  NOT NULL,
  [PromotionID] int  NULL,
  [ShippingAddress] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Notes] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [AdminNotes] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL,
  [UpdatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Orders] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Orders
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Orders] ON
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'1', N'ORD-2024-001', N'1', N'2024-01-15 10:30:00.000', N'4', N'1', N'600000.00', N'30000.00', N'0.00', N'630000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', N'Giao hàng vào bu?i chi?u', NULL, N'2024-01-15 10:30:00.000', N'2025-10-15 21:30:37.427')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'2', N'ORD-2024-002', N'2', N'2024-01-16 14:20:00.000', N'3', N'2', N'1200000.00', N'50000.00', N'100000.00', N'1150000.00', NULL, N'789 Ðu?ng DEF, Phu?ng 3, Qu?n 3, TP. H? Chí Minh', N'Thanh toán qua VNPay', NULL, N'2024-01-16 14:20:00.000', N'2025-10-15 21:30:37.427')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'3', N'ORD-2024-003', N'1', N'2024-01-18 09:15:00.000', N'2', N'1', N'800000.00', N'40000.00', N'0.00', N'840000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', N'C?n giao nhanh', NULL, N'2024-01-18 09:15:00.000', N'2025-10-15 21:30:37.427')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'4', N'ORD-2024-004', N'2', N'2024-01-20 16:45:00.000', N'1', N'2', N'450000.00', N'25000.00', N'0.00', N'475000.00', NULL, N'789 Ðu?ng DEF, Phu?ng 3, Qu?n 3, TP. H? Chí Minh', N'Ðon hàng m?i', NULL, N'2024-01-20 16:45:00.000', N'2025-10-15 21:30:37.427')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'13', N'PS20251015223222', N'1', N'2025-10-15 22:32:22.287', N'1', N'1', N'400000.00', N'30000.00', N'0.00', N'430000.00', NULL, N'fdf, fdf, fdf, fdfd', NULL, NULL, N'2025-10-15 22:32:22.287', N'2025-10-15 22:32:22.287')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'14', N'PS20251015223605', N'1', N'2025-10-15 22:36:05.017', N'1', N'2', N'400000.00', N'30000.00', N'0.00', N'430000.00', NULL, N'dsds, dsds, das, dsdsds', NULL, NULL, N'2025-10-15 22:36:05.017', N'2025-10-15 22:36:05.017')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'15', N'PS20251027100058', N'1', N'2025-10-27 10:00:58.543', N'1', N'2', N'750000.00', N'0.00', N'0.00', N'750000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-10-27 10:00:58.697', N'2025-10-27 10:00:58.700')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'16', N'PS20251029215348', N'1', N'2025-10-29 21:53:48.433', N'1', N'2', N'60000.00', N'30000.00', N'0.00', N'90000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-10-29 21:53:48.487', N'2025-10-29 21:53:48.487')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'17', N'PS20251029215643', N'1', N'2025-10-29 21:56:43.017', N'1', N'2', N'350000.00', N'30000.00', N'0.00', N'380000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-10-29 21:56:43.070', N'2025-10-29 21:56:43.070')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'18', N'PS20251029220439', N'1', N'2025-10-29 22:04:39.333', N'1', N'2', N'350000.00', N'30000.00', N'0.00', N'380000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-10-29 22:04:39.360', N'2025-10-29 22:04:39.360')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'19', N'PS20251029220541', N'1', N'2025-10-29 22:05:41.640', N'1', N'2', N'60000.00', N'30000.00', N'0.00', N'90000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-10-29 22:05:41.650', N'2025-10-29 22:05:41.650')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'20', N'PS20251029230238', N'1', N'2025-10-29 23:02:38.187', N'1', N'2', N'350000.00', N'30000.00', N'0.00', N'380000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-10-29 23:02:38.287', N'2025-10-29 23:02:38.287')
GO

INSERT INTO [dbo].[Orders] ([OrderID], [OrderNumber], [UserID], [OrderDate], [StatusID], [PaymentMethodID], [SubTotal], [ShippingFee], [DiscountAmount], [TotalAmount], [PromotionID], [ShippingAddress], [Notes], [AdminNotes], [CreatedDate], [UpdatedDate]) VALUES (N'21', N'PS20251101221053', N'1', N'2025-11-01 22:10:53.557', N'3', N'2', N'100000.00', N'30000.00', N'0.00', N'130000.00', NULL, N'123 Ðu?ng ABC, Phu?ng 1, Qu?n 1, TP. H? Chí Minh', NULL, NULL, N'2025-11-01 22:10:53.707', N'2025-11-01 22:14:08.903')
GO

SET IDENTITY_INSERT [dbo].[Orders] OFF
GO


-- ----------------------------
-- Table structure for OrderStatuses
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderStatuses]') AND type IN ('U'))
	DROP TABLE [dbo].[OrderStatuses]
GO

CREATE TABLE [dbo].[OrderStatuses] (
  [StatusID] int  IDENTITY(1,1) NOT NULL,
  [StatusName] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Description] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [SortOrder] int DEFAULT 0 NULL
)
GO

ALTER TABLE [dbo].[OrderStatuses] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of OrderStatuses
-- ----------------------------
SET IDENTITY_INSERT [dbo].[OrderStatuses] ON
GO

INSERT INTO [dbo].[OrderStatuses] ([StatusID], [StatusName], [Description], [SortOrder]) VALUES (N'1', N'New', N'Ðon hàng m?i', N'1')
GO

INSERT INTO [dbo].[OrderStatuses] ([StatusID], [StatusName], [Description], [SortOrder]) VALUES (N'2', N'Processing', N'Ðang x? lý', N'2')
GO

INSERT INTO [dbo].[OrderStatuses] ([StatusID], [StatusName], [Description], [SortOrder]) VALUES (N'3', N'Shipping', N'Ðang giao hàng', N'3')
GO

INSERT INTO [dbo].[OrderStatuses] ([StatusID], [StatusName], [Description], [SortOrder]) VALUES (N'4', N'Delivered', N'Ðã giao hàng', N'4')
GO

INSERT INTO [dbo].[OrderStatuses] ([StatusID], [StatusName], [Description], [SortOrder]) VALUES (N'5', N'Cancelled', N'Ðã h?y', N'5')
GO

SET IDENTITY_INSERT [dbo].[OrderStatuses] OFF
GO


-- ----------------------------
-- Table structure for OrderStatusHistory
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderStatusHistory]') AND type IN ('U'))
	DROP TABLE [dbo].[OrderStatusHistory]
GO

CREATE TABLE [dbo].[OrderStatusHistory] (
  [HistoryID] int  IDENTITY(1,1) NOT NULL,
  [OrderID] int  NOT NULL,
  [OldStatusID] int  NULL,
  [NewStatusID] int  NOT NULL,
  [ChangedBy] int  NOT NULL,
  [ChangedDate] datetime DEFAULT getdate() NULL,
  [Notes] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[OrderStatusHistory] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of OrderStatusHistory
-- ----------------------------
SET IDENTITY_INSERT [dbo].[OrderStatusHistory] ON
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'1', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'2', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'3', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'4', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'5', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'6', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'7', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'8', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'9', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'10', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'11', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'12', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'13', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'14', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'15', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'16', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'17', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'18', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'19', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'20', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'21', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'22', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'23', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'24', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'25', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'26', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'27', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'28', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'29', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'30', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'31', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'32', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'33', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'34', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'35', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'36', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'37', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'38', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'39', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'40', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'41', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'42', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'43', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'44', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'45', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'46', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'47', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'48', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'49', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'50', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'51', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'52', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'53', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'54', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'55', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'56', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'57', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'58', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'59', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'60', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'61', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'62', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'63', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'64', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'65', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'66', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'67', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'68', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'69', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'70', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'71', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'72', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'73', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'74', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'75', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'76', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'77', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'78', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'79', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'80', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'81', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'82', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'83', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'84', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'85', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'86', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'87', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'88', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'89', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'90', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'91', N'1', NULL, N'1', N'1', N'2024-01-15 10:30:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'92', N'1', N'1', N'2', N'1', N'2024-01-15 11:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'93', N'1', N'2', N'3', N'1', N'2024-01-15 14:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'94', N'1', N'3', N'4', N'1', N'2024-01-15 16:30:00.000', N'Ðã giao hàng thành công')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'95', N'2', NULL, N'1', N'2', N'2024-01-16 14:20:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'96', N'2', N'1', N'2', N'1', N'2024-01-16 15:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'97', N'2', N'2', N'3', N'1', N'2024-01-17 09:00:00.000', N'Ðang giao hàng')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'98', N'3', NULL, N'1', N'1', N'2024-01-18 09:15:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'99', N'3', N'1', N'2', N'1', N'2024-01-18 10:00:00.000', N'B?t d?u x? lý')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'100', N'4', NULL, N'1', N'2', N'2024-01-20 16:45:00.000', N'Ðon hàng du?c t?o')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'101', N'13', NULL, N'1', N'1', N'2025-10-15 22:32:22.523', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'102', N'14', NULL, N'1', N'1', N'2025-10-15 22:36:05.040', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'103', N'15', NULL, N'1', N'1', N'2025-10-27 10:01:00.067', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'104', N'16', NULL, N'1', N'1', N'2025-10-29 21:53:48.907', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'105', N'17', NULL, N'1', N'1', N'2025-10-29 21:56:43.310', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'106', N'18', NULL, N'1', N'1', N'2025-10-29 22:04:39.507', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'107', N'19', NULL, N'1', N'1', N'2025-10-29 22:05:41.670', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'108', N'20', NULL, N'1', N'1', N'2025-10-29 23:02:39.647', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'109', N'21', NULL, N'1', N'1', N'2025-11-01 22:10:54.597', N'Đơn hàng được tạo')
GO

INSERT INTO [dbo].[OrderStatusHistory] ([HistoryID], [OrderID], [OldStatusID], [NewStatusID], [ChangedBy], [ChangedDate], [Notes]) VALUES (N'110', N'21', N'1', N'3', N'3', N'2025-11-01 22:14:08.903', NULL)
GO

SET IDENTITY_INSERT [dbo].[OrderStatusHistory] OFF
GO


-- ----------------------------
-- Table structure for PaymentMethods
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentMethods]') AND type IN ('U'))
	DROP TABLE [dbo].[PaymentMethods]
GO

CREATE TABLE [dbo].[PaymentMethods] (
  [PaymentMethodID] int  IDENTITY(1,1) NOT NULL,
  [MethodName] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Description] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IsActive] bit DEFAULT 1 NULL
)
GO

ALTER TABLE [dbo].[PaymentMethods] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of PaymentMethods
-- ----------------------------
SET IDENTITY_INSERT [dbo].[PaymentMethods] ON
GO

INSERT INTO [dbo].[PaymentMethods] ([PaymentMethodID], [MethodName], [Description], [IsActive]) VALUES (N'1', N'COD', N'Thanh toán khi nhận hàng', N'0')
GO

INSERT INTO [dbo].[PaymentMethods] ([PaymentMethodID], [MethodName], [Description], [IsActive]) VALUES (N'2', N'VNPay', N'Thanh toán qua VNPay', N'0')
GO

SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF
GO


-- ----------------------------
-- Table structure for ProductImages
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductImages]') AND type IN ('U'))
	DROP TABLE [dbo].[ProductImages]
GO

CREATE TABLE [dbo].[ProductImages] (
  [ImageID] int  IDENTITY(1,1) NOT NULL,
  [ProductID] int  NOT NULL,
  [ImageURL] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [AltText] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [IsPrimary] bit DEFAULT 0 NULL,
  [SortOrder] int DEFAULT 0 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[ProductImages] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of ProductImages
-- ----------------------------
SET IDENTITY_INSERT [dbo].[ProductImages] ON
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'77', N'63', N'/images/products/whiskas-adult-1.jpg', N'Whiskas Adult - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'78', N'63', N'/images/products/whiskas-adult-2.jpg', N'Whiskas Adult - Hình ph?', N'0', N'2', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'80', N'65', N'/images/products/royal-canin-kitten-1.jpg', N'Royal Canin Kitten - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'81', N'65', N'/images/products/royal-canin-kitten-2.jpg', N'Royal Canin Kitten - Hình ph?', N'0', N'2', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'82', N'66', N'/images/products/dog-leash-1.jpg', N'Dây d?t chó - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'83', N'67', N'/images/products/dog-cage-1.jpg', N'Chu?ng chó - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'84', N'67', N'/images/products/dog-cage-2.jpg', N'Chu?ng chó - Hình ph?', N'0', N'2', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'85', N'68', N'/images/products/dog-bowl-1.jpg', N'Bát an chó - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'86', N'69', N'/images/products/cat-litter-1.jpg', N'Khay cát mèo - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'87', N'69', N'/images/products/cat-litter-2.jpg', N'Khay cát mèo - Hình ph?', N'0', N'2', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'88', N'70', N'/images/products/cat-collar-1.jpg', N'Vòng c? mèo - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'89', N'71', N'/images/products/cat-scratch-1.jpg', N'C?t mài vu?t mèo - Hình chính', N'0', N'1', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'90', N'71', N'/images/products/cat-scratch-2.jpg', N'C?t mài vu?t mèo - Hình ph?', N'0', N'2', N'2025-10-15 21:39:57.660')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'91', N'60', N'/uploads/products/380c22a6-1f88-4c38-82e1-ba14bcd93229.jpeg', NULL, N'0', N'0', N'2025-10-22 16:50:38.910')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'92', N'61', N'/uploads/products/e75f730b-c1f1-4861-92c8-9604009d6ee4.jpg', NULL, N'0', N'0', N'2025-11-01 22:16:39.607')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'93', N'61', N'/uploads/products/655f0d7e-a4b6-44ea-aee0-7c5b0b10c6c4.jpg', NULL, N'0', N'0', N'2025-11-01 22:16:39.610')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'94', N'61', N'/uploads/products/b6892663-85cb-42fd-af59-cba9a0be8988.jpg', NULL, N'0', N'0', N'2025-11-01 22:16:39.607')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'95', N'62', N'/uploads/products/3b2b5840-31de-4cf8-af4c-ee2a52fa30ef.jpg', NULL, N'0', N'0', N'2025-11-01 22:17:11.717')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'96', N'62', N'/uploads/products/52ada6a2-472d-4018-a154-6a184ad6ac88.jpg', NULL, N'0', N'0', N'2025-11-01 22:17:11.717')
GO

INSERT INTO [dbo].[ProductImages] ([ImageID], [ProductID], [ImageURL], [AltText], [IsPrimary], [SortOrder], [CreatedDate]) VALUES (N'97', N'64', N'/uploads/products/92850fb5-50fd-46ef-b135-be0152fdaff3.jpg', NULL, N'0', N'0', N'2025-11-01 22:18:15.037')
GO

SET IDENTITY_INSERT [dbo].[ProductImages] OFF
GO


-- ----------------------------
-- Table structure for ProductPromotions
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductPromotions]') AND type IN ('U'))
	DROP TABLE [dbo].[ProductPromotions]
GO

CREATE TABLE [dbo].[ProductPromotions] (
  [ProductPromotionID] int  IDENTITY(1,1) NOT NULL,
  [ProductID] int  NOT NULL,
  [PromotionID] int  NOT NULL
)
GO

ALTER TABLE [dbo].[ProductPromotions] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of ProductPromotions
-- ----------------------------
SET IDENTITY_INSERT [dbo].[ProductPromotions] ON
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'34', N'60', N'1')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'35', N'61', N'1')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'36', N'62', N'2')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'37', N'63', N'1')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'38', N'64', N'2')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'39', N'65', N'3')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'40', N'67', N'3')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'41', N'69', N'2')
GO

INSERT INTO [dbo].[ProductPromotions] ([ProductPromotionID], [ProductID], [PromotionID]) VALUES (N'42', N'71', N'3')
GO

SET IDENTITY_INSERT [dbo].[ProductPromotions] OFF
GO


-- ----------------------------
-- Table structure for Products
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type IN ('U'))
	DROP TABLE [dbo].[Products]
GO

CREATE TABLE [dbo].[Products] (
  [ProductID] int  IDENTITY(1,1) NOT NULL,
  [ProductName] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [ProductCode] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CategoryID] int  NOT NULL,
  [BrandID] int  NOT NULL,
  [ProductType] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [PetType] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Weight] decimal(8,2)  NULL,
  [Dimensions] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ExpiryDate] date  NULL,
  [Description] nvarchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [ShortDescription] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Price] decimal(15,2)  NOT NULL,
  [SalePrice] decimal(15,2)  NULL,
  [Cost] decimal(15,2)  NULL,
  [IsNew] bit DEFAULT 0 NULL,
  [IsActive] bit DEFAULT 1 NULL,
  [IsFeatured] bit DEFAULT 0 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL,
  [UpdatedDate] datetime DEFAULT getdate() NULL,
  [AgeInMonths] int  NULL
)
GO

ALTER TABLE [dbo].[Products] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Products
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Products] ON
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'60', N'Royal Canin Adult', N'RC-ADULT-001', N'1', N'46', N'Food', N'Dog', N'3.00', N'30x20x10 cm', N'2025-12-31', N'Th?c an khô cao c?p cho chó tru?ng thành, giàu protein và vitamin', N'Th?c an khô cho chó tru?ng thành', N'450000.00', N'400000.00', N'300000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'61', N'Pedigree Adult', N'PED-ADULT-001', N'1', N'48', N'Food', N'Dog', N'2.50', N'25x18x8 cm', N'2025-11-30', N'Th?c an khô cho chó tru?ng thành, giá c? h?p lý', N'Th?c an khô cho chó tru?ng thành', N'180000.00', N'150000.00', N'120000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'62', N'Hill''s Science Diet', N'HILLS-ADULT-001', N'1', N'50', N'Food', N'Dog', N'4.00', N'35x25x12 cm', N'2025-10-31', N'Th?c an dinh du?ng khoa h?c cho chó', N'Th?c an dinh du?ng cho chó', N'380000.00', N'350000.00', N'250000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'63', N'Whiskas Adult', N'WHISKAS-ADULT-001', N'2', N'47', N'Food', N'Cat', N'1.50', N'20x15x6 cm', N'2025-12-31', N'Th?c an khô cho mèo tru?ng thành', N'Th?c an khô cho mèo', N'120000.00', N'100000.00', N'80000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'64', N'Felix Wet Food', N'FELIX-WET-001', N'2', N'49', N'Food', N'Cat', N'0.40', N'10x8x3 cm', N'2025-09-30', N'Th?c an u?t cho mèo, huong cá', N'Th?c an u?t cho mèo', N'25000.00', N'20000.00', N'15000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'65', N'Royal Canin Kitten', N'RC-KITTEN-001', N'2', N'46', N'Food', N'Cat', N'2.00', N'25x18x8 cm', N'2025-11-30', N'Th?c an cho mèo con', N'Th?c an cho mèo con', N'320000.00', N'280000.00', N'200000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'66', N'Dây d?t chó', N'DOG-LEASH-001', N'3', N'53', N'Accessory', N'Dog', N'0.30', N'120x2 cm', NULL, N'Dây d?t chó ch?t lu?ng cao, ch?ng r?', N'Dây d?t chó cao c?p', N'150000.00', N'120000.00', N'80000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'67', N'Chu?ng chó', N'DOG-CAGE-001', N'3', N'53', N'Accessory', N'Dog', N'5.00', N'80x60x70 cm', NULL, N'Chu?ng chó b?ng thép, có th? g?p l?i', N'Chu?ng chó di d?ng', N'800000.00', N'700000.00', N'500000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'68', N'Bát an chó', N'DOG-BOWL-001', N'3', N'53', N'Accessory', N'Dog', N'0.50', N'20x20x8 cm', NULL, N'Bát an chó b?ng inox, ch?ng tru?t', N'Bát an chó inox', N'80000.00', N'60000.00', N'40000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'69', N'Khay cát mèo', N'CAT-LITTER-001', N'4', N'53', N'Accessory', N'Cat', N'2.00', N'50x40x15 cm', NULL, N'Khay cát mèo có n?p, ch?ng tràn', N'Khay cát mèo cao c?p', N'200000.00', N'180000.00', N'120000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'70', N'Vòng c? mèo', N'CAT-COLLAR-001', N'4', N'53', N'Accessory', N'Cat', N'0.10', N'25x2 cm', NULL, N'Vòng c? mèo có chuông', N'Vòng c? mèo', N'50000.00', N'40000.00', N'25000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

INSERT INTO [dbo].[Products] ([ProductID], [ProductName], [ProductCode], [CategoryID], [BrandID], [ProductType], [PetType], [Weight], [Dimensions], [ExpiryDate], [Description], [ShortDescription], [Price], [SalePrice], [Cost], [IsNew], [IsActive], [IsFeatured], [CreatedDate], [UpdatedDate], [AgeInMonths]) VALUES (N'71', N'C?t mài vu?t', N'CAT-SCRATCH-001', N'4', N'53', N'Accessory', N'Cat', N'1.00', N'60x15x15 cm', NULL, N'C?t mài vu?t cho mèo, có d? choi', N'C?t mài vu?t mèo', N'180000.00', N'150000.00', N'100000.00', N'0', N'0', N'0', N'2025-10-15 21:39:57.657', N'2025-10-15 21:39:57.657', NULL)
GO

SET IDENTITY_INSERT [dbo].[Products] OFF
GO


-- ----------------------------
-- Table structure for Promotions
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Promotions]') AND type IN ('U'))
	DROP TABLE [dbo].[Promotions]
GO

CREATE TABLE [dbo].[Promotions] (
  [PromotionID] int  IDENTITY(1,1) NOT NULL,
  [PromotionCode] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [PromotionName] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Description] nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [DiscountType] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [DiscountValue] decimal(15,2)  NOT NULL,
  [MinOrderAmount] decimal(15,2) DEFAULT 0 NULL,
  [MaxDiscountAmount] decimal(15,2)  NULL,
  [UsageLimit] int  NULL,
  [UsedCount] int DEFAULT 0 NULL,
  [StartDate] datetime  NOT NULL,
  [EndDate] datetime  NOT NULL,
  [IsActive] bit DEFAULT 1 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Promotions] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Promotions
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Promotions] ON
GO

INSERT INTO [dbo].[Promotions] ([PromotionID], [PromotionCode], [PromotionName], [Description], [DiscountType], [DiscountValue], [MinOrderAmount], [MaxDiscountAmount], [UsageLimit], [UsedCount], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'1', N'WELCOME10', N'Chào mừng khách hàng mới', N'Gi?m 10% cho don hàng d?u tiên', N'Percentage', N'10.00', N'500000.00', N'100000.00', N'100', N'0', N'2024-01-01 00:00:00.000', N'2025-10-26 00:00:00.000', N'0', N'2025-10-26 14:57:55.147')
GO

INSERT INTO [dbo].[Promotions] ([PromotionID], [PromotionCode], [PromotionName], [Description], [DiscountType], [DiscountValue], [MinOrderAmount], [MaxDiscountAmount], [UsageLimit], [UsedCount], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'2', N'SAVE50K', N'Tiết kiệm 50k', N'Gi?m 50k cho don hàng từ 500k', N'FixedAmount', N'50000.00', N'500000.00', N'50000.00', N'200', N'0', N'2024-01-01 00:00:00.000', N'2025-10-26 00:00:00.000', N'0', N'2025-10-26 14:59:04.413')
GO

INSERT INTO [dbo].[Promotions] ([PromotionID], [PromotionCode], [PromotionName], [Description], [DiscountType], [DiscountValue], [MinOrderAmount], [MaxDiscountAmount], [UsageLimit], [UsedCount], [StartDate], [EndDate], [IsActive], [CreatedDate]) VALUES (N'3', N'VIP20', N'Khách hàng VIP', N'Gi?m 20% cho khách hàng VIP', N'Percentage', N'20.00', N'1000000.00', N'200000.00', N'50', N'0', N'2024-01-01 00:00:00.000', N'2024-12-31 00:00:00.000', N'0', N'2025-10-15 21:30:37.427')
GO

SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO


-- ----------------------------
-- Table structure for UserRoles
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type IN ('U'))
	DROP TABLE [dbo].[UserRoles]
GO

CREATE TABLE [dbo].[UserRoles] (
  [RoleID] int  IDENTITY(1,1) NOT NULL,
  [RoleName] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Description] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[UserRoles] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of UserRoles
-- ----------------------------
SET IDENTITY_INSERT [dbo].[UserRoles] ON
GO

INSERT INTO [dbo].[UserRoles] ([RoleID], [RoleName], [Description], [CreatedDate]) VALUES (N'1', N'Admin', N'Quản trị viên hệ thống', N'2025-10-06 09:24:34.480')
GO

INSERT INTO [dbo].[UserRoles] ([RoleID], [RoleName], [Description], [CreatedDate]) VALUES (N'2', N'Employee', N'Nhân viên cửa hàng', N'2025-10-06 09:24:34.480')
GO

INSERT INTO [dbo].[UserRoles] ([RoleID], [RoleName], [Description], [CreatedDate]) VALUES (N'3', N'Customer', N'Khách hàng', N'2025-10-06 09:24:34.480')
GO

SET IDENTITY_INSERT [dbo].[UserRoles] OFF
GO


-- ----------------------------
-- Table structure for Users
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type IN ('U'))
	DROP TABLE [dbo].[Users]
GO

CREATE TABLE [dbo].[Users] (
  [UserID] int  IDENTITY(1,1) NOT NULL,
  [Username] nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Email] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [PasswordHash] nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [FullName] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS  NOT NULL,
  [Phone] nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [Address] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS  NULL,
  [RoleID] int  NOT NULL,
  [IsActive] bit DEFAULT 1 NULL,
  [CreatedDate] datetime DEFAULT getdate() NULL,
  [LastLoginDate] datetime  NULL
)
GO

ALTER TABLE [dbo].[Users] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Users
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Users] ON
GO

INSERT INTO [dbo].[Users] ([UserID], [Username], [Email], [PasswordHash], [FullName], [Phone], [Address], [RoleID], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (N'1', N'trongduc03', N'trongduc03@gmail.com', N'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', N'Nguyễn Trọng', N'0386693354', N'hà Tĩnh', N'3', N'0', N'2025-10-09 23:07:49.137', N'2025-11-04 00:34:27.500')
GO

INSERT INTO [dbo].[Users] ([UserID], [Username], [Email], [PasswordHash], [FullName], [Phone], [Address], [RoleID], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (N'2', N'hotro', N'hotro123@gmail.com', N'bHuOhVvOPBhvPN+6WJ2Gr/q5/FLVcrAMnMtK3bnpXMk=', N'hotro', N'0386693355', N'dxsdsds', N'3', N'0', N'2025-10-09 23:14:37.397', N'2025-10-09 23:28:19.523')
GO

INSERT INTO [dbo].[Users] ([UserID], [Username], [Email], [PasswordHash], [FullName], [Phone], [Address], [RoleID], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (N'3', N'admin', N'admin@hyhy.com', N'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', N'Administrator', N'0123456789', N'Hà N?i', N'1', N'0', N'2025-10-21 10:44:41.993', N'2025-11-12 20:50:52.650')
GO

SET IDENTITY_INSERT [dbo].[Users] OFF
GO


-- ----------------------------
-- Table structure for Wishlist
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Wishlist]') AND type IN ('U'))
	DROP TABLE [dbo].[Wishlist]
GO

CREATE TABLE [dbo].[Wishlist] (
  [WishlistID] int  IDENTITY(1,1) NOT NULL,
  [UserID] int  NOT NULL,
  [ProductID] int  NOT NULL,
  [AddedDate] datetime DEFAULT getdate() NULL
)
GO

ALTER TABLE [dbo].[Wishlist] SET (LOCK_ESCALATION = TABLE)
GO


-- ----------------------------
-- Records of Wishlist
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Wishlist] ON
GO

INSERT INTO [dbo].[Wishlist] ([WishlistID], [UserID], [ProductID], [AddedDate]) VALUES (N'3', N'1', N'71', N'2025-11-01 22:13:09.943')
GO

SET IDENTITY_INSERT [dbo].[Wishlist] OFF
GO


-- ----------------------------
-- procedure structure for sp_UpdateInventory
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateInventory]') AND type IN ('P', 'PC', 'RF', 'X'))
	DROP PROCEDURE[dbo].[sp_UpdateInventory]
GO

CREATE PROCEDURE [dbo].[sp_UpdateInventory]
    @ProductID INT,
    @Quantity INT,
    @TransactionType NVARCHAR(20),
    @CreatedBy INT
AS
BEGIN
    BEGIN TRANSACTION;
    
    -- Cập nhật tồn kho
    UPDATE Inventory 
    SET QuantityInStock = QuantityInStock + 
        CASE 
            WHEN @TransactionType = 'Import' THEN @Quantity
            WHEN @TransactionType = 'Export' THEN -@Quantity
            ELSE @Quantity
        END,
        LastUpdated = GETDATE()
    WHERE ProductID = @ProductID;
    
    -- Ghi lịch sử giao dịch
    INSERT INTO InventoryTransactions (ProductID, TransactionType, Quantity, CreatedBy)
    VALUES (@ProductID, @TransactionType, @Quantity, @CreatedBy);
    
    COMMIT TRANSACTION;
END
GO


-- ----------------------------
-- Auto increment value for Addresses
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Addresses]', RESEED, 40)
GO


-- ----------------------------
-- Primary Key structure for table Addresses
-- ----------------------------
ALTER TABLE [dbo].[Addresses] ADD CONSTRAINT [PK__Addresse__091C2A1B68849373] PRIMARY KEY CLUSTERED ([AddressID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Banners
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Banners]', RESEED, 50)
GO


-- ----------------------------
-- Primary Key structure for table Banners
-- ----------------------------
ALTER TABLE [dbo].[Banners] ADD CONSTRAINT [PK__Banners__32E86A319DB7EE99] PRIMARY KEY CLUSTERED ([BannerID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Brands
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Brands]', RESEED, 53)
GO


-- ----------------------------
-- Uniques structure for table Brands
-- ----------------------------
ALTER TABLE [dbo].[Brands] ADD CONSTRAINT [UQ__Brands__2206CE9B2E0C7F56] UNIQUE NONCLUSTERED ([BrandName] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Brands
-- ----------------------------
ALTER TABLE [dbo].[Brands] ADD CONSTRAINT [PK__Brands__DAD4F3BE607EB5C5] PRIMARY KEY CLUSTERED ([BrandID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Cart
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Cart]', RESEED, 40)
GO


-- ----------------------------
-- Uniques structure for table Cart
-- ----------------------------
ALTER TABLE [dbo].[Cart] ADD CONSTRAINT [UK_Cart_User_Product] UNIQUE NONCLUSTERED ([UserID] ASC, [ProductID] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Cart
-- ----------------------------
ALTER TABLE [dbo].[Cart] ADD CONSTRAINT [PK__Cart__51BCD7976BD66E1B] PRIMARY KEY CLUSTERED ([CartID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Categories
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Categories]', RESEED, 12)
GO


-- ----------------------------
-- Primary Key structure for table Categories
-- ----------------------------
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [PK__Categori__19093A2B0F876D0F] PRIMARY KEY CLUSTERED ([CategoryID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for ContactMessages
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[ContactMessages]', RESEED, 30)
GO


-- ----------------------------
-- Primary Key structure for table ContactMessages
-- ----------------------------
ALTER TABLE [dbo].[ContactMessages] ADD CONSTRAINT [PK__ContactM__C87C037C2BDCDBBF] PRIMARY KEY CLUSTERED ([MessageID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for CustomerProfiles
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[CustomerProfiles]', RESEED, 2)
GO


-- ----------------------------
-- Primary Key structure for table CustomerProfiles
-- ----------------------------
ALTER TABLE [dbo].[CustomerProfiles] ADD CONSTRAINT [PK__Customer__290C88849405B23E] PRIMARY KEY CLUSTERED ([ProfileID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Inventory
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Inventory]', RESEED, 54)
GO


-- ----------------------------
-- Indexes structure for table Inventory
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Inventory_ProductID]
ON [dbo].[Inventory] (
  [ProductID] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table Inventory
-- ----------------------------
ALTER TABLE [dbo].[Inventory] ADD CONSTRAINT [PK__Inventor__F5FDE6D33D1C516F] PRIMARY KEY CLUSTERED ([InventoryID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for InventoryTransactions
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[InventoryTransactions]', RESEED, 65)
GO


-- ----------------------------
-- Primary Key structure for table InventoryTransactions
-- ----------------------------
ALTER TABLE [dbo].[InventoryTransactions] ADD CONSTRAINT [PK__Inventor__55433A4BB267B9CE] PRIMARY KEY CLUSTERED ([TransactionID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for OrderItems
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[OrderItems]', RESEED, 56)
GO


-- ----------------------------
-- Indexes structure for table OrderItems
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderID]
ON [dbo].[OrderItems] (
  [OrderID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductID]
ON [dbo].[OrderItems] (
  [ProductID] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table OrderItems
-- ----------------------------
ALTER TABLE [dbo].[OrderItems] ADD CONSTRAINT [PK__OrderIte__57ED06A1A4B5A397] PRIMARY KEY CLUSTERED ([OrderItemID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Orders
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Orders]', RESEED, 21)
GO


-- ----------------------------
-- Indexes structure for table Orders
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Orders_UserID]
ON [dbo].[Orders] (
  [UserID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Orders_StatusID]
ON [dbo].[Orders] (
  [StatusID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Orders_OrderDate]
ON [dbo].[Orders] (
  [OrderDate] ASC
)
GO


-- ----------------------------
-- Uniques structure for table Orders
-- ----------------------------
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [UQ__Orders__CAC5E74352FD9EEF] UNIQUE NONCLUSTERED ([OrderNumber] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Orders
-- ----------------------------
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [PK__Orders__C3905BAFABDC1056] PRIMARY KEY CLUSTERED ([OrderID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for OrderStatuses
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[OrderStatuses]', RESEED, 5)
GO


-- ----------------------------
-- Uniques structure for table OrderStatuses
-- ----------------------------
ALTER TABLE [dbo].[OrderStatuses] ADD CONSTRAINT [UQ__OrderSta__05E7698AA7C462AD] UNIQUE NONCLUSTERED ([StatusName] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table OrderStatuses
-- ----------------------------
ALTER TABLE [dbo].[OrderStatuses] ADD CONSTRAINT [PK__OrderSta__C8EE20439063C844] PRIMARY KEY CLUSTERED ([StatusID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for OrderStatusHistory
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[OrderStatusHistory]', RESEED, 110)
GO


-- ----------------------------
-- Primary Key structure for table OrderStatusHistory
-- ----------------------------
ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [PK__OrderSta__4D7B4ADDB8E3B580] PRIMARY KEY CLUSTERED ([HistoryID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for PaymentMethods
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[PaymentMethods]', RESEED, 2)
GO


-- ----------------------------
-- Uniques structure for table PaymentMethods
-- ----------------------------
ALTER TABLE [dbo].[PaymentMethods] ADD CONSTRAINT [UQ__PaymentM__218CFB179543B7D9] UNIQUE NONCLUSTERED ([MethodName] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table PaymentMethods
-- ----------------------------
ALTER TABLE [dbo].[PaymentMethods] ADD CONSTRAINT [PK__PaymentM__DC31C1F36E0566F4] PRIMARY KEY CLUSTERED ([PaymentMethodID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for ProductImages
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[ProductImages]', RESEED, 97)
GO


-- ----------------------------
-- Primary Key structure for table ProductImages
-- ----------------------------
ALTER TABLE [dbo].[ProductImages] ADD CONSTRAINT [PK__ProductI__7516F4EC7AE4BEAC] PRIMARY KEY CLUSTERED ([ImageID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for ProductPromotions
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[ProductPromotions]', RESEED, 42)
GO


-- ----------------------------
-- Primary Key structure for table ProductPromotions
-- ----------------------------
ALTER TABLE [dbo].[ProductPromotions] ADD CONSTRAINT [PK__ProductP__9E52FB2380D5CA10] PRIMARY KEY CLUSTERED ([ProductPromotionID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Products
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Products]', RESEED, 71)
GO


-- ----------------------------
-- Indexes structure for table Products
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Products_CategoryID]
ON [dbo].[Products] (
  [CategoryID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Products_BrandID]
ON [dbo].[Products] (
  [BrandID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Products_ProductType]
ON [dbo].[Products] (
  [ProductType] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Products_PetType]
ON [dbo].[Products] (
  [PetType] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Products_IsActive]
ON [dbo].[Products] (
  [IsActive] ASC
)
GO


-- ----------------------------
-- Uniques structure for table Products
-- ----------------------------
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [UQ__Products__2F4E024F395B6826] UNIQUE NONCLUSTERED ([ProductCode] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Products
-- ----------------------------
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [PK__Products__B40CC6EDE4BBB053] PRIMARY KEY CLUSTERED ([ProductID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Promotions
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Promotions]', RESEED, 10)
GO


-- ----------------------------
-- Uniques structure for table Promotions
-- ----------------------------
ALTER TABLE [dbo].[Promotions] ADD CONSTRAINT [UQ__Promotio__A617E4B6B4E30459] UNIQUE NONCLUSTERED ([PromotionCode] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Promotions
-- ----------------------------
ALTER TABLE [dbo].[Promotions] ADD CONSTRAINT [PK__Promotio__52C42F2F633B1E11] PRIMARY KEY CLUSTERED ([PromotionID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for UserRoles
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[UserRoles]', RESEED, 3)
GO


-- ----------------------------
-- Uniques structure for table UserRoles
-- ----------------------------
ALTER TABLE [dbo].[UserRoles] ADD CONSTRAINT [UQ__UserRole__8A2B6160024F4CB5] UNIQUE NONCLUSTERED ([RoleName] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table UserRoles
-- ----------------------------
ALTER TABLE [dbo].[UserRoles] ADD CONSTRAINT [PK__UserRole__8AFACE3A0F2335E6] PRIMARY KEY CLUSTERED ([RoleID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Users
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 3)
GO


-- ----------------------------
-- Indexes structure for table Users
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Users_Email]
ON [dbo].[Users] (
  [Email] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Users_RoleID]
ON [dbo].[Users] (
  [RoleID] ASC
)
GO


-- ----------------------------
-- Uniques structure for table Users
-- ----------------------------
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [UQ__Users__536C85E444670658] UNIQUE NONCLUSTERED ([Username] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users] ADD CONSTRAINT [UQ__Users__A9D10534B8B96E0E] UNIQUE NONCLUSTERED ([Email] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Users
-- ----------------------------
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [PK__Users__1788CCAC17A56828] PRIMARY KEY CLUSTERED ([UserID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Auto increment value for Wishlist
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Wishlist]', RESEED, 3)
GO


-- ----------------------------
-- Indexes structure for table Wishlist
-- ----------------------------
CREATE NONCLUSTERED INDEX [IX_Wishlist_UserID]
ON [dbo].[Wishlist] (
  [UserID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Wishlist_ProductID]
ON [dbo].[Wishlist] (
  [ProductID] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Wishlist_AddedDate]
ON [dbo].[Wishlist] (
  [AddedDate] ASC
)
GO


-- ----------------------------
-- Uniques structure for table Wishlist
-- ----------------------------
ALTER TABLE [dbo].[Wishlist] ADD CONSTRAINT [UQ_Wishlist_User_Product] UNIQUE NONCLUSTERED ([UserID] ASC, [ProductID] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Primary Key structure for table Wishlist
-- ----------------------------
ALTER TABLE [dbo].[Wishlist] ADD CONSTRAINT [PK__Wishlist__233189CB373B4D9C] PRIMARY KEY CLUSTERED ([WishlistID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO


-- ----------------------------
-- Foreign Keys structure for table Addresses
-- ----------------------------
ALTER TABLE [dbo].[Addresses] ADD CONSTRAINT [FK_Addresses_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Cart
-- ----------------------------
ALTER TABLE [dbo].[Cart] ADD CONSTRAINT [FK_Cart_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Cart] ADD CONSTRAINT [FK_Cart_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Categories
-- ----------------------------
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [FK_Categories_Parent] FOREIGN KEY ([ParentCategoryID]) REFERENCES [dbo].[Categories] ([CategoryID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table ContactMessages
-- ----------------------------
ALTER TABLE [dbo].[ContactMessages] ADD CONSTRAINT [FK_ContactMessages_Users] FOREIGN KEY ([RepliedBy]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table CustomerProfiles
-- ----------------------------
ALTER TABLE [dbo].[CustomerProfiles] ADD CONSTRAINT [FK_CustomerProfiles_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Inventory
-- ----------------------------
ALTER TABLE [dbo].[Inventory] ADD CONSTRAINT [FK_Inventory_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table InventoryTransactions
-- ----------------------------
ALTER TABLE [dbo].[InventoryTransactions] ADD CONSTRAINT [FK_InventoryTransactions_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[InventoryTransactions] ADD CONSTRAINT [FK_InventoryTransactions_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table OrderItems
-- ----------------------------
ALTER TABLE [dbo].[OrderItems] ADD CONSTRAINT [FK_OrderItems_Orders] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[OrderItems] ADD CONSTRAINT [FK_OrderItems_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Orders
-- ----------------------------
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[OrderStatuses] ([StatusID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_PaymentMethod] FOREIGN KEY ([PaymentMethodID]) REFERENCES [dbo].[PaymentMethods] ([PaymentMethodID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Promotions] FOREIGN KEY ([PromotionID]) REFERENCES [dbo].[Promotions] ([PromotionID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table OrderStatusHistory
-- ----------------------------
ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OrderStatusHistory_Orders] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OrderStatusHistory_OldStatus] FOREIGN KEY ([OldStatusID]) REFERENCES [dbo].[OrderStatuses] ([StatusID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OrderStatusHistory_NewStatus] FOREIGN KEY ([NewStatusID]) REFERENCES [dbo].[OrderStatuses] ([StatusID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OrderStatusHistory_Users] FOREIGN KEY ([ChangedBy]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table ProductImages
-- ----------------------------
ALTER TABLE [dbo].[ProductImages] ADD CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table ProductPromotions
-- ----------------------------
ALTER TABLE [dbo].[ProductPromotions] ADD CONSTRAINT [FK_ProductPromotions_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[ProductPromotions] ADD CONSTRAINT [FK_ProductPromotions_Promotions] FOREIGN KEY ([PromotionID]) REFERENCES [dbo].[Promotions] ([PromotionID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Products
-- ----------------------------
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Categories] ([CategoryID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Brands] FOREIGN KEY ([BrandID]) REFERENCES [dbo].[Brands] ([BrandID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Users
-- ----------------------------
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[UserRoles] ([RoleID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


-- ----------------------------
-- Foreign Keys structure for table Wishlist
-- ----------------------------
ALTER TABLE [dbo].[Wishlist] ADD CONSTRAINT [FK_Wishlist_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

ALTER TABLE [dbo].[Wishlist] ADD CONSTRAINT [FK_Wishlist_Products] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

