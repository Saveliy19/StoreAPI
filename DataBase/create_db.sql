DROP TABLE IF EXISTS StoreProduct;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Store;

CREATE TABLE Store (
	StoreID INT NOT NULL GENERATED ALWAYS AS IDENTITY primary key, -- ���������� ��� ��������
	NAME VARCHAR(50) NOT NULL, -- �������� ��������
	ADDRESS VARCHAR(200) NOT NULL -- ����� ��������
);

CREATE TABLE Product (
	NAME VARCHAR(100) PRIMARY KEY -- �������� ��������
);

CREATE TABLE StoreProduct (
	StoreID	INT NOT NULL, 
	ProductName VARCHAR(100),
	PRICE DECIMAL (7, 2) NOT NULL, -- ���� ������ � ��������
	QUANTITY INT NOT NULL -- ���������� ������ � ��������
	PRIMARY KEY(StoreId, ProductName)
);