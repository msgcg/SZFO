CREATE TABLE dbo.Books
(
    Id INT IDENTITY(1,1) PRIMARY KEY, -- Уникальный идентификатор с автоинкрементом
    Code NVARCHAR(MAX) NOT NULL,      -- Код товара
    Name NVARCHAR(MAX) NOT NULL,      -- Имя товара
    Razdel NVARCHAR(MAX) NOT NULL,    -- Раздел (категория)
    FullDescription NVARCHAR(MAX)     -- Полное описание товара
);
