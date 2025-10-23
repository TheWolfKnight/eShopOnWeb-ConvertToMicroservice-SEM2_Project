using System.Data;
using Microservice.Catalog.Common.Models;
using Microservice.Catalog.Infrastructure.Interfaces;
using Microservice.Catalog.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Catalog.Infrastructure.Repositories;

internal class CatalogBrandRepository: ICatalogBrandRepository
{
    internal const string CONNECTION_STRING_KEY = "catalog-brand";

    private readonly string _connectionString;
    private readonly ILogger<CatalogBrandRepository> _logger;

    public CatalogBrandRepository(IServiceProvider serviceProvider, ILogger<CatalogBrandRepository> logger)
    {
        string? connectionString = serviceProvider.GetKeyedService<string>(CONNECTION_STRING_KEY);
        if (connectionString is null)
            throw new InvalidOperationException("Could not create CatalogBrandRepository due to missing connection string");

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

    public async Task<CatalogBrand?> GetBrandByIdAsync(int brandId, CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
SELECT B.Id B.Brand FROM [CatalogBrands] B
WHERE B.Id = @{nameof(brandId)}
";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;

            command.AddParameterValue($"@{nameof(brandId)}", SqlDbType.Int, brandId);

            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            CatalogBrand? result = null;

            if (await reader.ReadAsync(cancellationToken))
            {
                int i = 0;
                result = new CatalogBrand
                {
                    Id = reader.GetInt32(i++),
                    Brand = reader.GetString(i++)
                };
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch catalog brand with id = {BrandId} due to internal error",
                brandId
            );

            throw e;
        }
    }

    public async Task<IEnumerable<CatalogBrand>> GetBrandsAsync(CancellationToken cancellationToken = default)
    {
        string sqlString = $@"
SELECT B.Id, B.Brand FROM [CatalogBrands] B
";

        try
        {
            await using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = connection.CreateCommand();

            command.CommandText = sqlString;
            List<CatalogBrand> result = [];

            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                int i = 0;
                CatalogBrand brand = new CatalogBrand
                {
                    Id = reader.GetInt32(i++),
                    Brand = reader.GetString(i++)
                };

                result.Add(brand);
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch catalog brands due to internal error"
            );

            throw e;
        }
    }
}

