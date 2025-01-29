CREATE TABLE Products (
                          Id SERIAL PRIMARY KEY,  -- ✅ Используем SERIAL вместо IDENTITY
                          Name VARCHAR(255) NOT NULL,  -- ✅ PostgreSQL использует VARCHAR вместо NVARCHAR
                          Price NUMERIC(18,2) NOT NULL,  -- ✅ PostgreSQL использует NUMERIC вместо DECIMAL
                          StockQuantity INT NOT NULL,
                          DeleteDate TIMESTAMP NULL  -- ✅ PostgreSQL использует TIMESTAMP вместо DATETIME
);

CREATE TABLE Orders (
                        Id SERIAL PRIMARY KEY,
                        OrderDate TIMESTAMP NOT NULL DEFAULT NOW(),  -- ✅ PostgreSQL использует NOW() вместо GETUTCDATE()
                        TotalPrice NUMERIC(18,2) NOT NULL,
                        DeleteDate TIMESTAMP NULL
);

CREATE TABLE OrderItems (
                            Id SERIAL PRIMARY KEY,
                            OrderId INT NOT NULL,
                            ProductId INT NOT NULL,
                            Quantity INT NOT NULL,
                            Price NUMERIC(18,2) NOT NULL,
                            FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
                            FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
