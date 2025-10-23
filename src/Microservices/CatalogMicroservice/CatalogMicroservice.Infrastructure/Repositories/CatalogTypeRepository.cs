using System.Data;
using Microservice.Catalog.Common.Models;
using Microservice.Catalog.Infrastructure.Helpers;
using Microservice.Catalog.Infrastructure.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microservice.Catalog.Infrastructure.Repositories;

internal class CatalogTypeRepository: ICatalogTypeRepository
{
    internal const string CONNECTION_STRING_KEY = "catalog-type";

    private readonly string _connectionString;
    private readonly ILogger _logger;

    public CatalogTypeRepository(IKeyedServiceProvider keyedServiceProvider, ILogger<CatalogBrandRepository> logger)
    {
        string? connectionString = keyedServiceProvider.GetKeyedService<string>(CONNECTION_STRING_KEY);
        if (connectionString is null)
            throw new InvalidOperationException("Could not create CatalogTypeRepository due to missing connection string");

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
    public async Task<CatalogType?> GetCatalogTypeAsync(int typeId, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
SELECT T.Id, T.[Type] from [CatalogTypes] T
WHERE T.Id = @{nameof(typeId)}
";
        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;

            command.AddParameterValue($"@{nameof(typeId)}", SqlDbType.Int, typeId);

            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            CatalogType? type = null;

            if (await reader.ReadAsync(cancellationToken))
            {
                int i = 0;
                type = new CatalogType
                {
                    Id = reader.GetInt32(i++),
                    Type = reader.GetString(i++)
                };
            }

            return type;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch catalog type with id = {TypeId} due to internal error",
                typeId
            );

            throw e;
        }
    }

    public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync(CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
SELECT T.Id, T.[Type] from [CatalogTypes]
";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;
            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            List<CatalogType> result = [];
            while (await reader.ReadAsync(cancellationToken))
            {
                int i = 0;
                CatalogType type = new CatalogType
                {
                    Id = reader.GetInt32(i++),
                    Type = reader.GetString(i++)
                };
                result.Add(type);
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch catalog types due to internal error"
            );

            throw e;
        }
    }
}
