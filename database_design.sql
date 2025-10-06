-- =====================================================
-- CƠ SỞ DỮ LIỆU HỆ THỐNG QUẢN LÝ CỬA HÀNG THÚ CƯNG
-- Website bán phụ kiện và thức ăn cho chó mèo
-- =====================================================

-- Tạo database
CREATE DATABASE PetShopDB_10_2025;
USE PetShopDB_10_2025;

-- =====================================================
-- 1. BẢNG QUẢN LÝ NGƯỜI DÙNG
-- =====================================================

-- Bảng phân quyền người dùng
CREATE TABLE UserRoles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE, -- Admin, Employee, Customer
    Description NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng người dùng (Admin, Nhân viên, Khách hàng)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(200),
    RoleID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLoginDate DATETIME,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES UserRoles(RoleID)
);

-- Bảng hồ sơ khách hàng chi tiết
CREATE TABLE CustomerProfiles (
    ProfileID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    TotalOrders INT DEFAULT 0,
    TotalSpent DECIMAL(15,2) DEFAULT 0,
    MembershipLevel NVARCHAR(20) DEFAULT 'Bronze', -- Bronze, Silver, Gold, Platinum
    Points INT DEFAULT 0,
    CONSTRAINT FK_CustomerProfiles_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- =====================================================
-- 2. BẢNG QUẢN LÝ SẢN PHẨM
-- =====================================================

-- Bảng danh mục sản phẩm
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    ParentCategoryID INT NULL, -- Để tạo danh mục con
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    SortOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentCategoryID) REFERENCES Categories(CategoryID)
);

-- Bảng thương hiệu
CREATE TABLE Brands (
    BrandID INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL UNIQUE,
    LogoURL NVARCHAR(255),
    Description NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng sản phẩm chính
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(200) NOT NULL,
    ProductCode NVARCHAR(50) UNIQUE, -- Mã sản phẩm
    CategoryID INT NOT NULL,
    BrandID INT NOT NULL,
    ProductType NVARCHAR(20) NOT NULL, -- 'Food', 'Accessory'
    PetType NVARCHAR(20) NOT NULL, -- 'Dog', 'Cat', 'Both'
    Weight DECIMAL(8,2), -- Trọng lượng (kg)
    Dimensions NVARCHAR(100), -- Kích thước
    ExpiryDate DATE, -- Hạn sử dụng
    Description NVARCHAR(2000),
    ShortDescription NVARCHAR(500),
    Price DECIMAL(15,2) NOT NULL,
    SalePrice DECIMAL(15,2), -- Giá khuyến mãi
    Cost DECIMAL(15,2), -- Giá nhập
    IsNew BIT DEFAULT 0, -- Sản phẩm mới
    IsActive BIT DEFAULT 1,
    IsFeatured BIT DEFAULT 0, -- Sản phẩm nổi bật
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Products_Brands FOREIGN KEY (BrandID) REFERENCES Brands(BrandID)
);

-- Bảng hình ảnh sản phẩm
CREATE TABLE ProductImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    ImageURL NVARCHAR(255) NOT NULL,
    AltText NVARCHAR(100),
    IsPrimary BIT DEFAULT 0, -- Hình ảnh chính
    SortOrder INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Bảng quản lý tồn kho
CREATE TABLE Inventory (
    InventoryID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    QuantityInStock INT NOT NULL DEFAULT 0,
    MinStockLevel INT DEFAULT 10, -- Mức tồn kho tối thiểu
    MaxStockLevel INT DEFAULT 1000, -- Mức tồn kho tối đa
    LastUpdated DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Bảng lịch sử nhập/xuất kho
CREATE TABLE InventoryTransactions (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    TransactionType NVARCHAR(20) NOT NULL, -- 'Import', 'Export', 'Adjustment'
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(15,2),
    TotalValue DECIMAL(15,2),
    ReferenceNumber NVARCHAR(50), -- Số phiếu nhập/xuất
    Notes NVARCHAR(500),
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_InventoryTransactions_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT FK_InventoryTransactions_Users FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
);

-- =====================================================
-- 3. BẢNG QUẢN LÝ ĐỊA CHỈ GIAO HÀNG
-- =====================================================

CREATE TABLE Addresses (
    AddressID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    Ward NVARCHAR(100), -- Phường/Xã
    District NVARCHAR(100), -- Quận/Huyện
    City NVARCHAR(100), -- Tỉnh/Thành phố
    IsDefault BIT DEFAULT 0, -- Địa chỉ mặc định
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Addresses_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- =====================================================
-- 4. BẢNG QUẢN LÝ GIỎ HÀNG
-- =====================================================

CREATE TABLE Cart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    AddedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Cart_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Cart_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT UK_Cart_User_Product UNIQUE (UserID, ProductID)
);

-- =====================================================
-- 5. BẢNG QUẢN LÝ KHUYẾN MÃI
-- =====================================================

-- Bảng mã giảm giá
CREATE TABLE Promotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    PromotionCode NVARCHAR(50) NOT NULL UNIQUE,
    PromotionName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    DiscountType NVARCHAR(20) NOT NULL, -- 'Percentage', 'FixedAmount'
    DiscountValue DECIMAL(15,2) NOT NULL,
    MinOrderAmount DECIMAL(15,2) DEFAULT 0, -- Đơn hàng tối thiểu
    MaxDiscountAmount DECIMAL(15,2), -- Giảm giá tối đa
    UsageLimit INT, -- Số lần sử dụng tối đa
    UsedCount INT DEFAULT 0, -- Số lần đã sử dụng
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng áp dụng khuyến mãi cho sản phẩm
CREATE TABLE ProductPromotions (
    ProductPromotionID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    PromotionID INT NOT NULL,
    CONSTRAINT FK_ProductPromotions_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT FK_ProductPromotions_Promotions FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID)
);

-- =====================================================
-- 6. BẢNG QUẢN LÝ ĐƠN HÀNG
-- =====================================================

-- Bảng trạng thái đơn hàng
CREATE TABLE OrderStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200),
    SortOrder INT DEFAULT 0
);

-- Bảng phương thức thanh toán
CREATE TABLE PaymentMethods (
    PaymentMethodID INT PRIMARY KEY IDENTITY(1,1),
    MethodName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200),
    IsActive BIT DEFAULT 1
);

-- Bảng đơn hàng chính
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE, -- Mã đơn hàng
    UserID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    StatusID INT NOT NULL,
    PaymentMethodID INT NOT NULL,
    SubTotal DECIMAL(15,2) NOT NULL, -- Tổng tiền hàng
    ShippingFee DECIMAL(15,2) DEFAULT 0, -- Phí vận chuyển
    DiscountAmount DECIMAL(15,2) DEFAULT 0, -- Tiền giảm giá
    TotalAmount DECIMAL(15,2) NOT NULL, -- Tổng tiền thanh toán
    PromotionID INT NULL, -- Mã khuyến mãi đã sử dụng
    ShippingAddress NVARCHAR(500) NOT NULL,
    Notes NVARCHAR(500), -- Ghi chú của khách hàng
    AdminNotes NVARCHAR(500), -- Ghi chú của admin
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Orders_Status FOREIGN KEY (StatusID) REFERENCES OrderStatuses(StatusID),
    CONSTRAINT FK_Orders_PaymentMethod FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods(PaymentMethodID),
    CONSTRAINT FK_Orders_Promotions FOREIGN KEY (PromotionID) REFERENCES Promotions(PromotionID)
);

-- Bảng chi tiết đơn hàng
CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(15,2) NOT NULL, -- Giá tại thời điểm mua
    TotalPrice DECIMAL(15,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Bảng lịch sử thay đổi trạng thái đơn hàng
CREATE TABLE OrderStatusHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    OldStatusID INT,
    NewStatusID INT NOT NULL,
    ChangedBy INT NOT NULL,
    ChangedDate DATETIME DEFAULT GETDATE(),
    Notes NVARCHAR(500),
    CONSTRAINT FK_OrderStatusHistory_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderStatusHistory_OldStatus FOREIGN KEY (OldStatusID) REFERENCES OrderStatuses(StatusID),
    CONSTRAINT FK_OrderStatusHistory_NewStatus FOREIGN KEY (NewStatusID) REFERENCES OrderStatuses(StatusID),
    CONSTRAINT FK_OrderStatusHistory_Users FOREIGN KEY (ChangedBy) REFERENCES Users(UserID)
);

-- =====================================================
-- 7. BẢNG QUẢN LÝ BANNER QUẢNG CÁO
-- =====================================================

CREATE TABLE Banners (
    BannerID INT PRIMARY KEY IDENTITY(1,1),
    BannerName NVARCHAR(200) NOT NULL,
    ImageURL NVARCHAR(255) NOT NULL,
    LinkURL NVARCHAR(255),
    AltText NVARCHAR(100),
    Position NVARCHAR(50), -- 'Homepage', 'Category', 'Product'
    SortOrder INT DEFAULT 0,
    StartDate DATETIME,
    EndDate DATETIME,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- =====================================================
-- 8. BẢNG QUẢN LÝ LIÊN HỆ VÀ HỖ TRỢ
-- =====================================================

CREATE TABLE ContactMessages (
    MessageID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20),
    Subject NVARCHAR(200) NOT NULL,
    Message NVARCHAR(2000) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'New', -- 'New', 'Read', 'Replied', 'Closed'
    ReplyMessage NVARCHAR(2000),
    RepliedBy INT,
    RepliedDate DATETIME,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ContactMessages_Users FOREIGN KEY (RepliedBy) REFERENCES Users(UserID)
);

-- Bảng FAQ đã được loại bỏ - sẽ fix cứng trong code

-- =====================================================
-- 9. BẢNG QUẢN LÝ BÁO CÁO VÀ THỐNG KÊ
-- =====================================================

-- Các bảng báo cáo đã được loại bỏ - sẽ tính toán trong code
-- RevenueReports và ProductSalesStats sẽ được tạo động từ dữ liệu Orders và OrderItems

-- =====================================================
-- 10. CHÈN DỮ LIỆU MẪU
-- =====================================================

-- Chèn dữ liệu mẫu cho UserRoles
INSERT INTO UserRoles (RoleName, Description) VALUES
('Admin', 'Quản trị viên hệ thống'),
('Employee', 'Nhân viên cửa hàng'),
('Customer', 'Khách hàng');

-- Chèn dữ liệu mẫu cho OrderStatuses
INSERT INTO OrderStatuses (StatusName, Description, SortOrder) VALUES
('New', 'Đơn hàng mới', 1),
('Processing', 'Đang xử lý', 2),
('Shipping', 'Đang giao hàng', 3),
('Delivered', 'Đã giao hàng', 4),
('Cancelled', 'Đã hủy', 5);

-- Chèn dữ liệu mẫu cho PaymentMethods
INSERT INTO PaymentMethods (MethodName, Description) VALUES
('COD', 'Thanh toán khi nhận hàng'),
('VNPay', 'Thanh toán qua VNPay');

-- Chèn dữ liệu mẫu cho Categories
INSERT INTO Categories (CategoryName, Description) VALUES
('Thức ăn chó', 'Các loại thức ăn dành cho chó'),
('Thức ăn mèo', 'Các loại thức ăn dành cho mèo'),
('Phụ kiện chó', 'Dây dắt, chuồng, đồ chơi cho chó'),
('Phụ kiện mèo', 'Khay cát, vòng cổ, đồ chơi cho mèo');

-- Chèn dữ liệu mẫu cho Brands
INSERT INTO Brands (BrandName, Description) VALUES
('Royal Canin', 'Thương hiệu thức ăn cao cấp cho thú cưng'),
('Whiskas', 'Thức ăn cho mèo'),
('Pedigree', 'Thức ăn cho chó'),
('Felix', 'Thức ăn ướt cho mèo');

-- Dữ liệu FAQ đã được loại bỏ - sẽ fix cứng trong code

-- =====================================================
-- 11. TẠO INDEX ĐỂ TỐI ƯU HIỆU SUẤT
-- =====================================================

-- Index cho bảng Users
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_RoleID ON Users(RoleID);

-- Index cho bảng Products
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IX_Products_BrandID ON Products(BrandID);
CREATE INDEX IX_Products_ProductType ON Products(ProductType);
CREATE INDEX IX_Products_PetType ON Products(PetType);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);

-- Index cho bảng Orders
CREATE INDEX IX_Orders_UserID ON Orders(UserID);
CREATE INDEX IX_Orders_StatusID ON Orders(StatusID);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);

-- Index cho bảng OrderItems
CREATE INDEX IX_OrderItems_OrderID ON OrderItems(OrderID);
CREATE INDEX IX_OrderItems_ProductID ON OrderItems(ProductID);

-- Index cho bảng Inventory
CREATE INDEX IX_Inventory_ProductID ON Inventory(ProductID);
