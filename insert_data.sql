-- =====================================================
-- FILE INSERT DATA CHO HỆ THỐNG PET SHOP
-- Dữ liệu mẫu cho cửa hàng thú cưng
-- =====================================================

USE PetShopDB_10_2025;

-- =====================================================
-- 1. INSERT DỮ LIỆU CHO USER ROLES (nếu chưa có)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE RoleName = 'Admin')
BEGIN
    INSERT INTO UserRoles (RoleName, Description) VALUES
    ('Admin', 'Quản trị viên hệ thống'),
    ('Employee', 'Nhân viên cửa hàng'),
    ('Customer', 'Khách hàng');
END

-- =====================================================
-- 2. INSERT DỮ LIỆU CHO ORDER STATUSES
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM OrderStatuses WHERE StatusName = 'New')
BEGIN
    INSERT INTO OrderStatuses (StatusName, Description, SortOrder) VALUES
    ('New', 'Đơn hàng mới', 1),
    ('Processing', 'Đang xử lý', 2),
    ('Shipping', 'Đang giao hàng', 3),
    ('Delivered', 'Đã giao hàng', 4),
    ('Cancelled', 'Đã hủy', 5);
END

-- =====================================================
-- 3. INSERT DỮ LIỆU CHO PAYMENT METHODS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE MethodName = 'COD')
BEGIN
    INSERT INTO PaymentMethods (MethodName, Description) VALUES
    ('COD', 'Thanh toán khi nhận hàng'),
    ('VNPay', 'Thanh toán qua VNPay');
END

-- =====================================================
-- 4. INSERT DỮ LIỆU CHO CATEGORIES
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Thức ăn chó')
BEGIN
    INSERT INTO Categories (CategoryName, Description, SortOrder) VALUES
    ('Thức ăn chó', 'Các loại thức ăn dành cho chó', 1),
    ('Thức ăn mèo', 'Các loại thức ăn dành cho mèo', 2),
    ('Phụ kiện chó', 'Dây dắt, chuồng, đồ chơi cho chó', 3),
    ('Phụ kiện mèo', 'Khay cát, vòng cổ, đồ chơi cho mèo', 4),
    ('Thức ăn khô', 'Thức ăn khô cho thú cưng', 5),
    ('Thức ăn ướt', 'Thức ăn ướt cho thú cưng', 6),
    ('Đồ chơi', 'Đồ chơi cho thú cưng', 7),
    ('Vệ sinh', 'Sản phẩm vệ sinh cho thú cưng', 8);
END

-- =====================================================
-- 5. INSERT DỮ LIỆU CHO BRANDS
-- =====================================================
-- Xóa dữ liệu cũ nếu có để tránh conflict (xóa bảng con trước)
DELETE FROM ProductImages WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001'));
DELETE FROM Inventory WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001'));
DELETE FROM InventoryTransactions WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001'));
DELETE FROM ProductPromotions WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001'));
DELETE FROM OrderItems WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001'));
DELETE FROM Cart WHERE ProductID IN (SELECT ProductID FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001'));
DELETE FROM Products WHERE ProductCode IN ('RC-ADULT-001', 'PED-ADULT-001', 'HILLS-ADULT-001', 'WHISKAS-ADULT-001', 'FELIX-WET-001', 'RC-KITTEN-001', 'DOG-LEASH-001', 'DOG-CAGE-001', 'DOG-BOWL-001', 'CAT-LITTER-001', 'CAT-COLLAR-001', 'CAT-SCRATCH-001');
DELETE FROM Brands WHERE BrandName IN ('Royal Canin', 'Whiskas', 'Pedigree', 'Felix', 'Hill''s', 'Purina', 'Orijen', 'Acana');

INSERT INTO Brands (BrandName, Description) VALUES
('Royal Canin', 'Thương hiệu thức ăn cao cấp cho thú cưng'),
('Whiskas', 'Thức ăn cho mèo'),
('Pedigree', 'Thức ăn cho chó'),
('Felix', 'Thức ăn ướt cho mèo'),
('Hill''s', 'Thức ăn dinh dưỡng cho thú cưng'),
('Purina', 'Thức ăn đa dạng cho thú cưng'),
('Orijen', 'Thức ăn cao cấp cho thú cưng'),
('Acana', 'Thức ăn tự nhiên cho thú cưng');

-- =====================================================
-- 6. INSERT DỮ LIỆU CHO CUSTOMER PROFILES (cho UserID 1 và 2)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM CustomerProfiles WHERE UserID = 1)
BEGIN
    INSERT INTO CustomerProfiles (UserID, DateOfBirth, Gender, TotalOrders, TotalSpent, MembershipLevel, Points) VALUES
    (1, '1990-05-15', 'Male', 5, 2500000, 'Silver', 250),
    (2, '1985-12-20', 'Female', 12, 4500000, 'Gold', 450);
END

-- =====================================================
-- 7. INSERT DỮ LIỆU CHO ADDRESSES
-- =====================================================
INSERT INTO Addresses (UserID, FullName, Phone, Address, Ward, District, City, IsDefault) VALUES
(1, 'Nguyễn Văn A', '0901234567', '123 Đường ABC', 'Phường 1', 'Quận 1', 'TP. Hồ Chí Minh', 1),
(1, 'Nguyễn Văn A', '0901234567', '456 Đường XYZ', 'Phường 2', 'Quận 2', 'TP. Hồ Chí Minh', 0),
(2, 'Trần Thị B', '0987654321', '789 Đường DEF', 'Phường 3', 'Quận 3', 'TP. Hồ Chí Minh', 1),
(2, 'Trần Thị B', '0987654321', '321 Đường GHI', 'Phường 4', 'Quận 4', 'TP. Hồ Chí Minh', 0);

-- =====================================================
-- 8. INSERT DỮ LIỆU CHO PRODUCTS
-- =====================================================
INSERT INTO Products (ProductName, ProductCode, CategoryID, BrandID, ProductType, PetType, Weight, Dimensions, ExpiryDate, Description, ShortDescription, Price, SalePrice, Cost, IsNew, IsActive, IsFeatured) VALUES
-- Thức ăn chó
('Royal Canin Adult', 'RC-ADULT-001', 1, (SELECT BrandID FROM Brands WHERE BrandName = 'Royal Canin'), 'Food', 'Dog', 3.0, '30x20x10 cm', '2025-12-31', 'Thức ăn khô cao cấp cho chó trưởng thành, giàu protein và vitamin', 'Thức ăn khô cho chó trưởng thành', 450000, 400000, 300000, 1, 1, 1),
('Pedigree Adult', 'PED-ADULT-001', 1, (SELECT BrandID FROM Brands WHERE BrandName = 'Pedigree'), 'Food', 'Dog', 2.5, '25x18x8 cm', '2025-11-30', 'Thức ăn khô cho chó trưởng thành, giá cả hợp lý', 'Thức ăn khô cho chó trưởng thành', 180000, 150000, 120000, 0, 1, 0),
('Hill''s Science Diet', 'HILLS-ADULT-001', 1, (SELECT BrandID FROM Brands WHERE BrandName = 'Hill''s'), 'Food', 'Dog', 4.0, '35x25x12 cm', '2025-10-31', 'Thức ăn dinh dưỡng khoa học cho chó', 'Thức ăn dinh dưỡng cho chó', 380000, 350000, 250000, 0, 1, 1),

-- Thức ăn mèo
('Whiskas Adult', 'WHISKAS-ADULT-001', 2, (SELECT BrandID FROM Brands WHERE BrandName = 'Whiskas'), 'Food', 'Cat', 1.5, '20x15x6 cm', '2025-12-31', 'Thức ăn khô cho mèo trưởng thành', 'Thức ăn khô cho mèo', 120000, 100000, 80000, 0, 1, 0),
('Felix Wet Food', 'FELIX-WET-001', 2, (SELECT BrandID FROM Brands WHERE BrandName = 'Felix'), 'Food', 'Cat', 0.4, '10x8x3 cm', '2025-09-30', 'Thức ăn ướt cho mèo, hương cá', 'Thức ăn ướt cho mèo', 25000, 20000, 15000, 1, 1, 1),
('Royal Canin Kitten', 'RC-KITTEN-001', 2, (SELECT BrandID FROM Brands WHERE BrandName = 'Royal Canin'), 'Food', 'Cat', 2.0, '25x18x8 cm', '2025-11-30', 'Thức ăn cho mèo con', 'Thức ăn cho mèo con', 320000, 280000, 200000, 0, 1, 0),

-- Phụ kiện chó
('Dây dắt chó', 'DOG-LEASH-001', 3, (SELECT BrandID FROM Brands WHERE BrandName = 'Acana'), 'Accessory', 'Dog', 0.3, '120x2 cm', NULL, 'Dây dắt chó chất lượng cao, chống rỉ', 'Dây dắt chó cao cấp', 150000, 120000, 80000, 0, 1, 0),
('Chuồng chó', 'DOG-CAGE-001', 3, (SELECT BrandID FROM Brands WHERE BrandName = 'Acana'), 'Accessory', 'Dog', 5.0, '80x60x70 cm', NULL, 'Chuồng chó bằng thép, có thể gập lại', 'Chuồng chó di động', 800000, 700000, 500000, 1, 1, 1),
('Bát ăn chó', 'DOG-BOWL-001', 3, (SELECT BrandID FROM Brands WHERE BrandName = 'Acana'), 'Accessory', 'Dog', 0.5, '20x20x8 cm', NULL, 'Bát ăn chó bằng inox, chống trượt', 'Bát ăn chó inox', 80000, 60000, 40000, 0, 1, 0),

-- Phụ kiện mèo
('Khay cát mèo', 'CAT-LITTER-001', 4, (SELECT BrandID FROM Brands WHERE BrandName = 'Acana'), 'Accessory', 'Cat', 2.0, '50x40x15 cm', NULL, 'Khay cát mèo có nắp, chống tràn', 'Khay cát mèo cao cấp', 200000, 180000, 120000, 0, 1, 0),
('Vòng cổ mèo', 'CAT-COLLAR-001', 4, (SELECT BrandID FROM Brands WHERE BrandName = 'Acana'), 'Accessory', 'Cat', 0.1, '25x2 cm', NULL, 'Vòng cổ mèo có chuông', 'Vòng cổ mèo', 50000, 40000, 25000, 0, 1, 0),
('Cột mài vuốt', 'CAT-SCRATCH-001', 4, (SELECT BrandID FROM Brands WHERE BrandName = 'Acana'), 'Accessory', 'Cat', 1.0, '60x15x15 cm', NULL, 'Cột mài vuốt cho mèo, có đồ chơi', 'Cột mài vuốt mèo', 180000, 150000, 100000, 1, 1, 1);

-- =====================================================
-- 9. INSERT DỮ LIỆU CHO PRODUCT IMAGES
-- =====================================================
INSERT INTO ProductImages (ProductID, ImageURL, AltText, IsPrimary, SortOrder) VALUES
-- Hình ảnh cho Royal Canin Adult
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), '/images/products/royal-canin-adult-1.jpg', 'Royal Canin Adult - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), '/images/products/royal-canin-adult-2.jpg', 'Royal Canin Adult - Hình phụ', 0, 2),
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), '/images/products/royal-canin-adult-3.jpg', 'Royal Canin Adult - Hình phụ', 0, 3),

-- Hình ảnh cho Pedigree Adult
((SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'), '/images/products/pedigree-adult-1.jpg', 'Pedigree Adult - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'), '/images/products/pedigree-adult-2.jpg', 'Pedigree Adult - Hình phụ', 0, 2),

-- Hình ảnh cho Hill's Science Diet
((SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'), '/images/products/hills-science-1.jpg', 'Hill''s Science Diet - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'), '/images/products/hills-science-2.jpg', 'Hill''s Science Diet - Hình phụ', 0, 2),

-- Hình ảnh cho Whiskas Adult
((SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'), '/images/products/whiskas-adult-1.jpg', 'Whiskas Adult - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'), '/images/products/whiskas-adult-2.jpg', 'Whiskas Adult - Hình phụ', 0, 2),

-- Hình ảnh cho Felix Wet Food
((SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'), '/images/products/felix-wet-1.jpg', 'Felix Wet Food - Hình chính', 1, 1),

-- Hình ảnh cho Royal Canin Kitten
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'), '/images/products/royal-canin-kitten-1.jpg', 'Royal Canin Kitten - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'), '/images/products/royal-canin-kitten-2.jpg', 'Royal Canin Kitten - Hình phụ', 0, 2),

-- Hình ảnh cho Dây dắt chó
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-LEASH-001'), '/images/products/dog-leash-1.jpg', 'Dây dắt chó - Hình chính', 1, 1),

-- Hình ảnh cho Chuồng chó
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'), '/images/products/dog-cage-1.jpg', 'Chuồng chó - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'), '/images/products/dog-cage-2.jpg', 'Chuồng chó - Hình phụ', 0, 2),

-- Hình ảnh cho Bát ăn chó
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-BOWL-001'), '/images/products/dog-bowl-1.jpg', 'Bát ăn chó - Hình chính', 1, 1),

-- Hình ảnh cho Khay cát mèo
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'), '/images/products/cat-litter-1.jpg', 'Khay cát mèo - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'), '/images/products/cat-litter-2.jpg', 'Khay cát mèo - Hình phụ', 0, 2),

-- Hình ảnh cho Vòng cổ mèo
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-COLLAR-001'), '/images/products/cat-collar-1.jpg', 'Vòng cổ mèo - Hình chính', 1, 1),

-- Hình ảnh cho Cột mài vuốt
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-SCRATCH-001'), '/images/products/cat-scratch-1.jpg', 'Cột mài vuốt mèo - Hình chính', 1, 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-SCRATCH-001'), '/images/products/cat-scratch-2.jpg', 'Cột mài vuốt mèo - Hình phụ', 0, 2);

-- =====================================================
-- 10. INSERT DỮ LIỆU CHO INVENTORY
-- =====================================================
INSERT INTO Inventory (ProductID, QuantityInStock, MinStockLevel, MaxStockLevel) VALUES
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), 50, 10, 100),
((SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'), 80, 15, 150),
((SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'), 30, 5, 80),
((SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'), 100, 20, 200),
((SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'), 200, 50, 500),
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'), 40, 8, 100),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-LEASH-001'), 25, 5, 50),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'), 10, 2, 20),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-BOWL-001'), 60, 10, 100),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'), 15, 3, 30),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-COLLAR-001'), 40, 8, 80),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-SCRATCH-001'), 20, 4, 40);

-- =====================================================
-- 11. INSERT DỮ LIỆU CHO INVENTORY TRANSACTIONS
-- =====================================================
INSERT INTO InventoryTransactions (ProductID, TransactionType, Quantity, UnitPrice, TotalValue, ReferenceNumber, Notes, CreatedBy) VALUES
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), 'Import', 50, 300000, 15000000, 'IMP-001', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'), 'Import', 80, 120000, 9600000, 'IMP-002', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'), 'Import', 30, 250000, 7500000, 'IMP-003', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'), 'Import', 100, 80000, 8000000, 'IMP-004', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'), 'Import', 200, 15000, 3000000, 'IMP-005', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'), 'Import', 40, 200000, 8000000, 'IMP-006', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-LEASH-001'), 'Import', 25, 80000, 2000000, 'IMP-007', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'), 'Import', 10, 500000, 5000000, 'IMP-008', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-BOWL-001'), 'Import', 60, 40000, 2400000, 'IMP-009', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'), 'Import', 15, 120000, 1800000, 'IMP-010', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-COLLAR-001'), 'Import', 40, 25000, 1000000, 'IMP-011', 'Nhập hàng lần đầu', 1),
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-SCRATCH-001'), 'Import', 20, 100000, 2000000, 'IMP-012', 'Nhập hàng lần đầu', 1);

-- =====================================================
-- 12. INSERT DỮ LIỆU CHO PROMOTIONS
-- =====================================================
INSERT INTO Promotions (PromotionCode, PromotionName, Description, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, UsageLimit, UsedCount, StartDate, EndDate, IsActive) VALUES
('WELCOME10', 'Chào mừng khách hàng mới', 'Giảm 10% cho đơn hàng đầu tiên', 'Percentage', 10, 500000, 100000, 100, 0, '2024-01-01', '2024-12-31', 1),
('SAVE50K', 'Tiết kiệm 50k', 'Giảm 50k cho đơn hàng từ 500k', 'FixedAmount', 50000, 500000, 50000, 200, 0, '2024-01-01', '2024-12-31', 1),
('VIP20', 'Khách hàng VIP', 'Giảm 20% cho khách hàng VIP', 'Percentage', 20, 1000000, 200000, 50, 0, '2024-01-01', '2024-12-31', 1);

-- =====================================================
-- 13. INSERT DỮ LIỆU CHO PRODUCT PROMOTIONS
-- =====================================================
INSERT INTO ProductPromotions (ProductID, PromotionID) VALUES
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'WELCOME10')), -- Royal Canin Adult áp dụng mã WELCOME10
((SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'WELCOME10')), -- Pedigree Adult áp dụng mã WELCOME10
((SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'SAVE50K')), -- Hill's Science Diet áp dụng mã SAVE50K
((SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'WELCOME10')), -- Whiskas Adult áp dụng mã WELCOME10
((SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'SAVE50K')), -- Felix Wet Food áp dụng mã SAVE50K
((SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'VIP20')), -- Royal Canin Kitten áp dụng mã VIP20
((SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'VIP20')), -- Chuồng chó áp dụng mã VIP20
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'SAVE50K')), -- Khay cát mèo áp dụng mã SAVE50K
((SELECT ProductID FROM Products WHERE ProductCode = 'CAT-SCRATCH-001'), (SELECT PromotionID FROM Promotions WHERE PromotionCode = 'VIP20')); -- Cột mài vuốt áp dụng mã VIP20

-- =====================================================
-- 14. INSERT DỮ LIỆU CHO ORDERS
-- =====================================================
INSERT INTO Orders (OrderNumber, UserID, OrderDate, StatusID, PaymentMethodID, SubTotal, ShippingFee, DiscountAmount, TotalAmount, ShippingAddress, Notes, CreatedDate) VALUES
('ORD-2024-001', 1, '2024-01-15 10:30:00', 4, 1, 600000, 30000, 0, 630000, '123 Đường ABC, Phường 1, Quận 1, TP. Hồ Chí Minh', 'Giao hàng vào buổi chiều', '2024-01-15 10:30:00'),
('ORD-2024-002', 2, '2024-01-16 14:20:00', 3, 2, 1200000, 50000, 100000, 1150000, '789 Đường DEF, Phường 3, Quận 3, TP. Hồ Chí Minh', 'Thanh toán qua VNPay', '2024-01-16 14:20:00'),
('ORD-2024-003', 1, '2024-01-18 09:15:00', 2, 1, 800000, 40000, 0, 840000, '123 Đường ABC, Phường 1, Quận 1, TP. Hồ Chí Minh', 'Cần giao nhanh', '2024-01-18 09:15:00'),
('ORD-2024-004', 2, '2024-01-20 16:45:00', 1, 2, 450000, 25000, 0, 475000, '789 Đường DEF, Phường 3, Quận 3, TP. Hồ Chí Minh', 'Đơn hàng mới', '2024-01-20 16:45:00');

-- =====================================================
-- 15. INSERT DỮ LIỆU CHO ORDER ITEMS
-- =====================================================
INSERT INTO OrderItems (OrderID, ProductID, Quantity, UnitPrice, TotalPrice) VALUES
-- Đơn hàng 1
(1, (SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), 1, 400000, 400000), -- Royal Canin Adult
(1, (SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'), 2, 100000, 200000), -- Whiskas Adult

-- Đơn hàng 2
(2, (SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'), 2, 150000, 300000), -- Pedigree Adult
(2, (SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'), 10, 20000, 200000), -- Felix Wet Food
(2, (SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'), 1, 700000, 700000), -- Chuồng chó

-- Đơn hàng 3
(3, (SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'), 1, 350000, 350000), -- Hill's Science Diet
(3, (SELECT ProductID FROM Products WHERE ProductCode = 'DOG-LEASH-001'), 2, 120000, 240000), -- Dây dắt chó
(3, (SELECT ProductID FROM Products WHERE ProductCode = 'DOG-BOWL-001'), 1, 60000, 60000), -- Bát ăn chó

-- Đơn hàng 4
(4, (SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'), 1, 280000, 280000), -- Royal Canin Kitten
(4, (SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'), 1, 180000, 180000); -- Khay cát mèo

-- ====================================================
-- 16. INSERT DỮ LIỆU CHO ORDER STATUS HISTORY
-- =====================================================
INSERT INTO OrderStatusHistory (OrderID, OldStatusID, NewStatusID, ChangedBy, ChangedDate, Notes) VALUES
-- Lịch sử đơn hàng 1
(1, NULL, 1, 1, '2024-01-15 10:30:00', 'Đơn hàng được tạo'),
(1, 1, 2, 1, '2024-01-15 11:00:00', 'Bắt đầu xử lý'),
(1, 2, 3, 1, '2024-01-15 14:00:00', 'Đang giao hàng'),
(1, 3, 4, 1, '2024-01-15 16:30:00', 'Đã giao hàng thành công'),

-- Lịch sử đơn hàng 2
(2, NULL, 1, 2, '2024-01-16 14:20:00', 'Đơn hàng được tạo'),
(2, 1, 2, 1, '2024-01-16 15:00:00', 'Bắt đầu xử lý'),
(2, 2, 3, 1, '2024-01-17 09:00:00', 'Đang giao hàng'),

-- Lịch sử đơn hàng 3
(3, NULL, 1, 1, '2024-01-18 09:15:00', 'Đơn hàng được tạo'),
(3, 1, 2, 1, '2024-01-18 10:00:00', 'Bắt đầu xử lý'),

-- Lịch sử đơn hàng 4
(4, NULL, 1, 2, '2024-01-20 16:45:00', 'Đơn hàng được tạo');

-- =====================================================
-- 17. INSERT DỮ LIỆU CHO CART (giỏ hàng hiện tại)
-- =====================================================
INSERT INTO Cart (UserID, ProductID, Quantity, AddedDate) VALUES
(1, (SELECT ProductID FROM Products WHERE ProductCode = 'CAT-COLLAR-001'), 1, '2024-01-21 10:00:00'), -- User 1 có vòng cổ mèo trong giỏ
(1, (SELECT ProductID FROM Products WHERE ProductCode = 'CAT-SCRATCH-001'), 1, '2024-01-21 10:05:00'), -- User 1 có cột mài vuốt trong giỏ
(2, (SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'), 2, '2024-01-21 14:30:00'), -- User 2 có Royal Canin Adult trong giỏ
(2, (SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'), 5, '2024-01-21 14:35:00'); -- User 2 có Felix Wet Food trong giỏ

-- =====================================================
-- 18. INSERT DỮ LIỆU CHO BANNERS
-- =====================================================
INSERT INTO Banners (BannerName, ImageURL, LinkURL, AltText, Position, SortOrder, StartDate, EndDate, IsActive) VALUES
('Banner chính trang chủ', '/images/banners/homepage-banner-1.jpg', '/products', 'Khuyến mãi thức ăn thú cưng', 'Homepage', 1, '2024-01-01', '2024-12-31', 1),
('Banner sản phẩm mới', '/images/banners/new-products-banner.jpg', '/products?filter=new', 'Sản phẩm mới cho thú cưng', 'Homepage', 2, '2024-01-01', '2024-12-31', 1),
('Banner khuyến mãi', '/images/banners/promotion-banner.jpg', '/promotions', 'Giảm giá lên đến 50%', 'Homepage', 3, '2024-01-01', '2024-12-31', 1),
('Banner thức ăn chó', '/images/banners/dog-food-banner.jpg', '/category/1', 'Thức ăn cho chó', 'Category', 1, '2024-01-01', '2024-12-31', 1),
('Banner thức ăn mèo', '/images/banners/cat-food-banner.jpg', '/category/2', 'Thức ăn cho mèo', 'Category', 2, '2024-01-01', '2024-12-31', 1);

-- =====================================================
-- 19. INSERT DỮ LIỆU CHO CONTACT MESSAGES
-- =====================================================
INSERT INTO ContactMessages (FullName, Email, Phone, Subject, Message, Status, CreatedDate) VALUES
('Nguyễn Văn C', 'nguyenvanc@email.com', '0912345678', 'Hỏi về sản phẩm', 'Tôi muốn hỏi về thức ăn cho chó con, có sản phẩm nào phù hợp không?', 'New', '2024-01-20 09:00:00'),
('Trần Thị D', 'tranthid@email.com', '0987654321', 'Khiếu nại', 'Tôi đã đặt hàng nhưng chưa nhận được, có thể kiểm tra giúp tôi không?', 'Read', '2024-01-19 15:30:00'),
('Lê Văn E', 'levane@email.com', '0909876543', 'Tư vấn', 'Tôi có con mèo 6 tháng tuổi, nên cho ăn thức ăn gì?', 'Replied', '2024-01-18 11:20:00');

-- =====================================================
-- 20. CẬP NHẬT DỮ LIỆU INVENTORY SAU KHI BÁN HÀNG
-- =====================================================
-- Cập nhật số lượng tồn kho sau khi bán hàng
UPDATE Inventory SET QuantityInStock = QuantityInStock - 1 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'RC-ADULT-001'); -- Royal Canin Adult
UPDATE Inventory SET QuantityInStock = QuantityInStock - 2 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'WHISKAS-ADULT-001'); -- Whiskas Adult
UPDATE Inventory SET QuantityInStock = QuantityInStock - 2 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'PED-ADULT-001'); -- Pedigree Adult
UPDATE Inventory SET QuantityInStock = QuantityInStock - 10 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'FELIX-WET-001'); -- Felix Wet Food
UPDATE Inventory SET QuantityInStock = QuantityInStock - 1 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'DOG-CAGE-001'); -- Chuồng chó
UPDATE Inventory SET QuantityInStock = QuantityInStock - 1 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'HILLS-ADULT-001'); -- Hill's Science Diet
UPDATE Inventory SET QuantityInStock = QuantityInStock - 2 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'DOG-LEASH-001'); -- Dây dắt chó
UPDATE Inventory SET QuantityInStock = QuantityInStock - 1 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'DOG-BOWL-001'); -- Bát ăn chó
UPDATE Inventory SET QuantityInStock = QuantityInStock - 1 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'RC-KITTEN-001'); -- Royal Canin Kitten
UPDATE Inventory SET QuantityInStock = QuantityInStock - 1 WHERE ProductID = (SELECT ProductID FROM Products WHERE ProductCode = 'CAT-LITTER-001'); -- Khay cát mèo

-- =====================================================
-- 21. CẬP NHẬT DỮ LIỆU CUSTOMER PROFILES
-- =====================================================
-- Cập nhật thông tin khách hàng sau khi mua hàng
UPDATE CustomerProfiles 
SET TotalOrders = TotalOrders + 2, 
    TotalSpent = TotalSpent + 1470000,
    Points = Points + 147
WHERE UserID = 1;

UPDATE CustomerProfiles 
SET TotalOrders = TotalOrders + 2, 
    TotalSpent = TotalSpent + 1625000,
    Points = Points + 162
WHERE UserID = 2;

-- =====================================================
-- HOÀN THÀNH INSERT DỮ LIỆU
-- =====================================================
PRINT 'Đã hoàn thành việc insert dữ liệu mẫu cho hệ thống Pet Shop!';
PRINT 'Dữ liệu bao gồm:';
PRINT '- 2 khách hàng (UserID 1 và 2)';
PRINT '- 12 sản phẩm đa dạng';
PRINT '- 4 đơn hàng với lịch sử trạng thái';
PRINT '- Giỏ hàng hiện tại';
PRINT '- Khuyến mãi và banner';
PRINT '- Tin nhắn liên hệ';
PRINT '- Quản lý tồn kho';
