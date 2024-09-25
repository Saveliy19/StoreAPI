DROP TABLE IF EXISTS StoreProduct;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Store;

CREATE TABLE Store (
	StoreID INT NOT NULL GENERATED ALWAYS AS IDENTITY primary key, -- уникальный код магазина
	NAME VARCHAR(50) NOT NULL, -- название магазина
	ADDRESS VARCHAR(200) NOT NULL -- адрес магазина
);

CREATE TABLE Product (
	NAME VARCHAR(100) PRIMARY KEY -- название продукта
);

CREATE TABLE StoreProduct (
	StoreID	INT NOT NULL, 
	ProductName VARCHAR(100),
	PRICE DECIMAL (7, 2) NOT NULL, -- цена товара в магазине
	QUANTITY INT NOT NULL -- количество товара в магазине
	PRIMARY KEY(StoreId, ProductName)
);