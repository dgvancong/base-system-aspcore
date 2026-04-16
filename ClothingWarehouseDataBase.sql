-- Sử dụng database hiện có
CREATE DATABASE ClothingWarehouse;
USE ClothingWarehouse;
GO

-- ========== BẢNG SẢN PHẨM ==========
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,          -- ID sản phẩm (tự động tăng, khóa chính)
    ProductCode NVARCHAR(50) NOT NULL UNIQUE,         -- Mã sản phẩm (VD: SP001, ÁO123) - không trùng
    ProductName NVARCHAR(200) NOT NULL,               -- Tên sản phẩm (VD: Áo thun trắng cổ tròn)
    SupplierName NVARCHAR(200) NOT NULL,              -- Tên nhà cung cấp (VD: Công ty A, Công ty B) - bắt buộc
    CategoryName NVARCHAR(100) NOT NULL,              -- Loại sản phẩm (VD: Áo, Quần, Váy, Áo khoác) - bắt buộc
    Color NVARCHAR(255),                              -- Màu sắc (có thể nhập nhiều màu, cách nhau bằng dấu phẩy, VD: Trắng,Đen,Xám)
    Size NVARCHAR(50),                                -- Kích thước (VD: S, M, L, XL hoặc 27,28,29 hoặc FreeSize)
    PurchasePrice DECIMAL(18,2) NOT NULL,             -- Giá nhập vào (đơn vị VNĐ, VD: 50000)
    SellingPrice DECIMAL(18,2) NOT NULL,              -- Giá bán ra (đơn vị VNĐ, VD: 120000)
    QuantityInStock INT NOT NULL DEFAULT 0,           -- Số lượng tồn kho hiện tại (mặc định 0)
    Description NVARCHAR(MAX),                        -- Mô tả chi tiết sản phẩm (chất liệu, xuất xứ, hướng dẫn bảo quản,...)
    BrandName NVARCHAR(100) NULL,                     -- Thương hiệu (VD: Nike, Adidas, Zara, Uniqlo) - có thể để trống
    Material NVARCHAR(100) NULL,                      -- Chất liệu vải (VD: Cotton 100%, Polyester, Jean, Len)
    Gender NVARCHAR(20) NULL,                         -- Giới tính sử dụng (VD: Nam, Nữ, Unisex)
    Status NVARCHAR(20) DEFAULT N'Đang bán',          -- Trạng thái sản phẩm (Đang bán / Ngừng bán / Hết hàng)
    CreatedDate DATETIME DEFAULT GETDATE(),           -- Ngày tạo sản phẩm (mặc định ngày hiện tại)
    UpdatedDate DATETIME DEFAULT GETDATE()            -- Ngày cập nhật sản phẩm gần nhất
);
-- Thêm 1 sản phẩm mẫu: Áo thun trắng
INSERT INTO Products (
    ProductCode, ProductName, SupplierName, CategoryName, 
    Color, Size, PurchasePrice, SellingPrice, QuantityInStock, 
    Description, BrandName, Material, Gender, Status
)
VALUES (
    'SP001',                           -- Mã sản phẩm
    N'Áo thun trắng cổ tròn',          -- Tên sản phẩm
    N'Công ty TNHH Thời trang ABC',    -- Nhà cung cấp
    N'Áo',                             -- Loại sản phẩm
    N'Trắng, Đen, Xám',                -- Màu sắc (3 màu)
    N'S, M, L, XL',                    -- Kích thước (4 size)
    50000,                             -- Giá nhập: 50,000đ
    150000,                            -- Giá bán: 150,000đ
    100,                               -- Số lượng tồn kho: 100 cái
    N'Áo thun chất liệu cotton cao cấp, thoáng mát, thấm hút mồ hôi tốt', -- Mô tả
    N'Uniqlo',                         -- Thương hiệu
    N'Cotton 100%',                    -- Chất liệu
    N'Unisex',                         -- Giới tính (Nam/Nữ đều mặc được)
    N'Đang bán'                        -- Trạng thái: đang bán
);

-- Thêm 1 sản phẩm mẫu: Áo thun trắng
INSERT INTO Products (
    ProductCode, ProductName, SupplierName, CategoryName, 
    Color, Size, PurchasePrice, SellingPrice, QuantityInStock, 
    Description, BrandName, Material, Gender, Status
)
VALUES (
    'SP002',                           -- Mã sản phẩm
    N'Quần thun trắng cổ tròn',          -- Tên sản phẩm
    N'Công ty TNHH Thời trang ABC',    -- Nhà cung cấp
    N'Quần',                             -- Loại sản phẩm
    N'Trắng, Đỏ, Xám',                -- Màu sắc (3 màu)
    N'S, M, L, XL',                    -- Kích thước (4 size)
    50000,                             -- Giá nhập: 50,000đ
    150000,                            -- Giá bán: 150,000đ
    100,                               -- Số lượng tồn kho: 100 cái
    N'Quần thun chất liệu cotton cao cấp, thoáng mát, thấm hút mồ hôi tốt', -- Mô tả
    N'Gucci',                         -- Thương hiệu
    N'Cotton 100%',                    -- Chất liệu
    N'Unisex',                         -- Giới tính (Nam/Nữ đều mặc được)
    N'Đang bán'                        -- Trạng thái: đang bán
);

CREATE INDEX IX_Products_Category ON [dbo].Products(CategoryName);
CREATE INDEX IX_Products_Brand ON [dbo].Products(BrandName);
CREATE INDEX IX_Products_Status ON [dbo].Products(Status);
CREATE INDEX IX_Products_Price ON [dbo].Products(SellingPrice);


-- ========== BẢNG ĐƠN BÁN HÀNG ==========
CREATE TABLE SalesOrders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,            -- Mã đơn hàng (tự động tăng, khóa chính)
    OrderDate DATETIME DEFAULT GETDATE(),             -- Ngày tạo đơn hàng (mặc định ngày giờ hiện tại)
    CustomerName NVARCHAR(200) NOT NULL,              -- Tên khách hàng (bắt buộc, VD: Nguyễn Văn A)
    CustomerPhone NVARCHAR(20) NULL,                  -- Số điện thoại khách hàng (VD: 0912345678)
    CustomerAddress NVARCHAR(255) NULL,               -- Địa chỉ giao hàng (VD: 123 Đường Lê Lợi, Quận 1, TP.HCM)
    TotalAmount DECIMAL(18,2) DEFAULT 0,              -- Tổng tiền của đơn hàng (tự động tính từ chi tiết đơn, đơn vị VNĐ)
    PaymentMethod NVARCHAR(50) DEFAULT N'Tiền mặt',   -- Phương thức thanh toán (Tiền mặt / Chuyển khoản / Thẻ)
    OrderStatus NVARCHAR(50) DEFAULT N'Hoàn thành',   -- Trạng thái đơn hàng (Đang xử lý / Hoàn thành / Đã hủy)
    SalesPerson NVARCHAR(100) NULL                    -- Nhân viên bán hàng (VD: Trần Thị B, Lê Văn C)
);

-- ========== BẢNG CHI TIẾT ĐƠN BÁN HÀNG ==========
CREATE TABLE SalesOrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,      -- ID chi tiết đơn hàng (tự động tăng, khóa chính)
    OrderID INT NOT NULL,                             -- Mã đơn hàng (liên kết với bảng SalesOrders)
    ProductID INT NOT NULL,                           -- Mã sản phẩm (liên kết với bảng Products)
    Quantity INT NOT NULL CHECK (Quantity > 0),       -- Số lượng sản phẩm mua (phải lớn hơn 0)
    UnitPrice DECIMAL(18,2) NOT NULL,                 -- Giá bán tại thời điểm mua (đề phòng giá sau này thay đổi, đơn vị VNĐ)
    DiscountAmount DECIMAL(18,2) DEFAULT 0,           -- Số tiền được giảm (nhập trực tiếp, VD: 50000)
    TotalAmount DECIMAL(18,2) DEFAULT 0,              -- Tổng tiền sau giảm giá
   
    -- Khóa ngoại
    FOREIGN KEY (OrderID) REFERENCES SalesOrders(OrderID),   -- Ràng buộc: OrderID phải tồn tại trong bảng SalesOrders
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)   -- Ràng buộc: ProductID phải tồn tại trong bảng Products
);

-- Đơn hàng 1: Khách Nguyễn Văn A
INSERT INTO SalesOrders (
    CustomerName, CustomerPhone, CustomerAddress, 
    TotalAmount, PaymentMethod, OrderStatus, SalesPerson
)
VALUES (
    N'Nguyễn Văn A', '0912345678', N'123 Đường Lê Lợi, Quận 1, TP.HCM',
    0, N'Chuyển khoản', N'Hoàn thành', N'Trần Thị B'
);


-- ===== ĐƠN HÀNG 1: Nguyễn Văn A mua 3 áo thun =====
-- Tính: 3 x 150,000 = 450,000 - 50,000 (giảm) = 400,000
INSERT INTO SalesOrderDetails (
    OrderID, ProductID, Quantity, UnitPrice, DiscountAmount, TotalAmount
)
VALUES (
    1, 1, 3, 150000, 50000, 400000
);


SELECT * FROM Products

SELECT * FROM SalesOrders

SELECT * FROM SalesOrderDetails


