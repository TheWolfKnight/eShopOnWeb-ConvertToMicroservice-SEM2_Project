using System.Data;
using Microservice.Catalog.Common.Models;
using Microservice.Catalog.Infrastructure.Helpers;
using Microservice.Catalog.Infrastructure.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microservice.Catalog.Infrastructure.Repositories;

internal class CatalogItemRepository: ICatalogItemRepository
{
    internal const string CONNECTION_STRING_KEY = "catalog-repository";

    private readonly string _connectionString;
    private readonly ILogger<CatalogItemRepository> _logger;

    public CatalogItemRepository(IServiceProvider serviceProvider, ILogger<CatalogItemRepository> logger)
    {
        string? connectionString = serviceProvider.GetKeyedService<string>(CONNECTION_STRING_KEY);

        if (connectionString is null)
            throw new InvalidOperationException("Could not create CatalogItemRepository due to missing connection string");
        _connectionString = connectionString;

        _logger = logger;
    }

    public async Task EnsureDbExistsAsync(CancellationToken cancellationToken = default)
    {
        string sqlString = @"
DECLARE @insertBrands INT = 0;
DECLARE @insertTypes INT = 0;
DECLARE @insertCatalog INT = 0;

IF (DB_ID('CatalogDatabase') IS NOT NULL)
BEGIN
    PRINT 'Database ""CatalogDatabase"" exists';
END
ELSE BEGIN
    CREATE DATABASE [CatalogDatabase];
END;

USE [CatalogDatabase];

IF (OBJECT_ID('CatalogBrands') IS NOT NULL)
BEGIN
    PRINT 'Table ""CatalogBrands"" exists';
END
ELSE BEGIN
    CREATE TABLE [CatalogBrands] (
        Id INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
        Brand NVARCHAR(100) NOT NULL,
    );
    SET @insertBrands = 1;
END;

IF (OBJECT_ID('CatalogTypes') IS NOT NULL)
BEGIN
    PRINT 'Table ""CatalogTypes"" exists';
END
ELSE BEGIN
    CREATE TABLE CatalogTypes (
        Id INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
        [Type] NVARCHAR(100) NOT NULL
    );
    SET @insertTypes = 1;
END;

IF (OBJECT_ID('Catalog') IS NOT NULL)
BEGIN
    PRINT 'Table ""Catalog"" exists';
END
ELSE BEGIN
    CREATE TABLE [Catalog] (
        Id INT PRIMARY KEY NOT NULL IDENTITY(1, 1),
        [Name] VARCHAR(50) NOT NULL,
        [Description] VARCHAR(MAX),
        Price DECIMAL(18, 2) NOT NULL,
        PictureUri VARCHAR(MAX) NOT NULL,
        CatalogTypeId INT NOT NULL,
        CatalogBrandId INT NOT NULL
    );
    SET @insertCatalog = 1;
END;

IF @insertBrands = 1
BEGIN
    PRINT 'Inserting Brands';
    INSERT INTO [CatalogBrands]([Brand])
    VALUES  ('Azure'),
            ('.NET'),
            ('Visual Studio'),
            ('SQL Server'),
            ('Other');
END;

IF @insertTypes = 1
BEGIN
    PRINT 'Inserting types';
    INSERT INTO [CatalogTypes]([Type])
    VALUES  ('Mug'),
            ('T-Shirt'),
            ('Sheet'),
            ('USB Memory Stick');
END;

IF @insertCatalog = 1
BEGIN
    PRINT 'Inserting Types';
    INSERT INTO [Catalog]([Name], [Description], [Price], [PictureUri], [CatalogTypeId], [CatalogBrandId])
    VALUEs  ('.NET Bot Black Sweatshirt','.NET Bot Black Sweatshirt',19.50,'http://catalogbaseurltobereplaced/images/products/1.png',2,2),
            ('.NET Black & White Mug','.NET Black & White Mug',8.50,'http://catalogbaseurltobereplaced/images/products/2.png',1,2),
            ('Prism White T-Shirt','Prism White T-Shirt',12.00,'http://catalogbaseurltobereplaced/images/products/3.png',2,5),
            ('.NET Foundation Sweatshirt','.NET Foundation Sweatshirt',12.00,'http://catalogbaseurltobereplaced/images/products/4.png',2,2),
            ('Roslyn Red Sheet','Roslyn Red Sheet',8.50,'http://catalogbaseurltobereplaced/images/products/5.png',3,5),
            ('.NET Blue Sweatshirt','.NET Blue Sweatshirt',12.00,'http://catalogbaseurltobereplaced/images/products/6.png',2,2),
            ('Roslyn Red T-Shirt','Roslyn Red T-Shirt',12.00,'http://catalogbaseurltobereplaced/images/products/7.png',2,5),
            ('Kudu Purple Sweatshirt','Kudu Purple Sweatshirt',8.50,'http://catalogbaseurltobereplaced/images/products/8.png',2,5),
            ('Cup<T> White Mug','Cup<T> White Mug',12.00,'http://catalogbaseurltobereplaced/images/products/9.png',1,5),
            ('.NET Foundation Sheet','.NET Foundation Sheet',12.00,'http://catalogbaseurltobereplaced/images/products/10.png',3,2),
            ('Cup<T> Sheet','Cup<T> Sheet',8.50,'http://catalogbaseurltobereplaced/images/products/11.png',3,2),
            ('Prism White TShirt','Prism White TShirt',12.00,'http://catalogbaseurltobereplaced/images/products/12.png',2,5);
END;
""";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State is ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;
            await command.ExecuteNonQueryAsync(cancellationToken);

            return;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could complete database setup due to internal error"
            );

            throw e;
        }
    }

    public async Task<CatalogItem?> GetItemAsync(int itemId, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
SELECT
    C.Id,
    C.[Name],
    C.[Description],
    C.Price,
    C.PictureUri,
    C.CatalogBrandId,
    C.CatalogTypeId
FROM [Catalog] C
WHERE C.Id = @itemId
        ";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State is ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;

            command.AddParameterValue("@itemId", SqlDbType.Int, itemId);

            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            CatalogItem? result = null;

            if (await reader.ReadAsync())
            {
                int i = 0;
                result = new CatalogItem
                {
                    Id = reader.GetInt32(i++),
                    Name = reader.GetString(i++),
                    Description = reader.GetString(i++),
                    Price = reader.GetDecimal(i++),
                    PictureUri = reader.GetString(i++),
                    CatalogBrandId = reader.GetInt32(i++),
                    CatalogTypeId = reader.GetInt32(i++)
                };
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch catalog item with id = {Id} due to internal error",
                itemId
            );

            throw e;
        }
    }

    public async Task<IEnumerable<CatalogItem>> GetItemPageAsync(int pageNo, int pageSize, int? brandId, int? typeId, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
WITH [ROW] AS (
    SELECT * FROM [Catalog]
    WHERE (CatalogBrandId = @{nameof(brandId)} OR @{nameof(brandId)} IS NULL)
    AND (CatalogTypeId = @{nameof(typeId)} OR @{nameof(typeId)} IS NULL)
    EXCEPT
    SELECT TOP (@{nameof(pageSize)} * (@{nameof(pageNo)} - 1)) * FROM [Catalog]
    WHERE (CatalogBrandId = @{nameof(brandId)} OR @{nameof(brandId)} IS NULL)
    and (CatalogTypeId = @{nameof(typeId)} OR @{nameof(typeId)} IS NULL)
)
SELECT TOP (@{nameof(pageSize)}) * FROM [ROW]
";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State is ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;

            command.AddParameterValue($"@{nameof(pageNo)}", SqlDbType.Int, pageNo);
            command.AddParameterValue($"@{nameof(pageSize)}", SqlDbType.Int, pageSize);
            command.AddParameterValue($"@{nameof(brandId)}", SqlDbType.Int, brandId);
            command.AddParameterValue($"@{nameof(typeId)}", SqlDbType.Int, typeId);

            List<CatalogItem> items = [];
            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while(await reader.ReadAsync(cancellationToken))
            {
                int i = 0;
                CatalogItem item = new CatalogItem
                {
                    Id = reader.GetInt32(i++),
                    Name = reader.GetString(i++),
                    Description = reader.GetString(i++),
                    Price = reader.GetDecimal(i++),
                    PictureUri = reader.GetString(i++),
                    CatalogBrandId = reader.GetInt32(i++),
                    CatalogTypeId = reader.GetInt32(i++)
                };

                items.Add(item);
            }

            return items;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch items due to internal error"
            );

            throw e;
        }
    }

    public async Task<CatalogItem> CreateItemAsync(CreateCatalogItem item, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
INSERT INTO [Catalog]([Name], [Description], Price, PictureUri, CatalogTypeId, CatalogBrandId)
     VALUES (
        @{nameof(CreateCatalogItem.Name)},
        @{nameof(CreateCatalogItem.Description)},
        @{nameof(CreateCatalogItem.Price)},
        @{nameof(CreateCatalogItem.PictureUri)}
        @{nameof(CreateCatalogItem.CatalogTypeId)},
        @{nameof(CreateCatalogItem.CatalogBrandId)}
    )
";
        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State is ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;

            command.AddParameterValue($"@{nameof(CreateCatalogItem.Name)}", SqlDbType.VarChar, item.Name);
            command.AddParameterValue($"@{nameof(CreateCatalogItem.Description)}", SqlDbType.VarChar, item.Description);
            command.AddParameterValue($"@{nameof(CreateCatalogItem.Price)}", SqlDbType.Binary, item.Price);
            command.AddParameterValue($"@{nameof(CreateCatalogItem.PictureUri)}", SqlDbType.VarChar, item.PictureUri);
            command.AddParameterValue($"@{nameof(CreateCatalogItem.CatalogTypeId)}", SqlDbType.Int, item.CatalogTypeId);
            command.AddParameterValue($"@{nameof(CreateCatalogItem.CatalogBrandId)}", SqlDbType.Int, item.CatalogBrandId);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not insert item due to internal error"
            );

            throw e;
        }
    }

    public async Task UpdateItemAsync(CatalogItem item, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
UPDATE [Catalog]
SET [Name] = @{nameof(CatalogItem.Name)},
    [Description] = @{nameof(CatalogItem.Description)},
    Price = @{nameof(CatalogItem.Price)},
    PictureUri = @{nameof(CatalogItem.PictureUri)},
    CatalogBrandId = @{nameof(CatalogItem.CatalogBrandId)},
    CatalogTypeId = @{nameof(CatalogItem.CatalogTypeId)}
WHERE Id = @{nameof(CatalogItem.Id)}
";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State is ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;

            command.AddParameterValue($"@{nameof(CatalogItem.Id)}", SqlDbType.Int, item.Id);
            command.AddParameterValue($"@{nameof(CatalogItem.Name)}", SqlDbType.VarChar, item.Name);
            command.AddParameterValue($"@{nameof(CatalogItem.Description)}", SqlDbType.VarChar, item.Description);
            command.AddParameterValue($"@{nameof(CatalogItem.Price)}", SqlDbType.Binary, item.Price);
            command.AddParameterValue($"@{nameof(CatalogItem.PictureUri)}", SqlDbType.VarChar, item.PictureUri);
            command.AddParameterValue($"@{nameof(CatalogItem.CatalogTypeId)}", SqlDbType.Int, item.CatalogTypeId);
            command.AddParameterValue($"@{nameof(CatalogItem.CatalogBrandId)}", SqlDbType.Int, item.CatalogBrandId);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Failed to update catalog item with id = {ItemId} due to internal error",
                item.Id
            );

            throw e;
        }
    }

    public async Task DeleteItemAsync(int itemId, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
DELETE FROM [Catalog]
WHERE Id = @{nameof(itemId)}
";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            if (connection.State is ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;
            command.AddParameterValue($"@{nameof(itemId)}", SqlDbType.Int, itemId);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Failed to delete item with id = {ItemId} due to internal error",
                itemId
            );

            throw e;
        }
    }
}
