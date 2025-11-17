using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

using CatalogMicroservice.Service;
using CatalogMicroservice.Common.Models;
using CatalogMicroservice.Infrastructure.Interfaces;
using CatalogMicroservice.Service.Interfaces;

namespace catologtest;

public class CatalogItemServiceTests
{
    private readonly Mock<ICatalogItemRepository> _itemRepo = new();
    private readonly Mock<ICatalogBrandService> _brandService = new();
    private readonly Mock<ICatalogTypeService> _typeService = new();

    private CatalogItemService CreateSut()
        => new CatalogItemService(_itemRepo.Object, _brandService.Object, _typeService.Object);

    [Fact]
    public async Task GetItemsAsync_uses_default_paging_when_invalid_values()
    {
        // Arrange
        var items = new List<CatalogItem>
        {
            new CatalogItem
            {
                Id = 1,
                Name = "Test item",
                Price = 100m,
                CatalogBrandId = 1,
                CatalogTypeId = 1
            }
        };

        _itemRepo
            .Setup(r => r.GetItemPageAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = CreateSut();

        // Act
        var result = await sut.GetItemsAsync(0, 0, null, null);

        // Assert
        Assert.Single(result);

        _itemRepo.Verify(r => r.GetItemPageAsync(
            1,                 // pageIndex < 1 -> 1
            10,                // pageSize <= 0 -> 10
            null,
            null,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateItemAsync_returns_null_when_brand_or_type_is_missing()
    {
        // Arrange
        _brandService
            .Setup(b => b.GetBrandByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatalogBrand?)null); // brand mangler

        _typeService
            .Setup(t => t.GetCatalogTypeAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CatalogType
            {
                Id = 1,
                Type = "TestType"
            });

        var sut = CreateSut();

        var create = new CreateCatalogItem
        {
            Name = "New item",
            Description = "Desc",
            Price = 123m,
            PictureUri = "test.png",
            CatalogBrandId = 1,
            CatalogTypeId = 1
        };

        // Act
        var result = await sut.CreateItemAsync(create);

        // Assert
        Assert.Null(result);
        _itemRepo.Verify(r => r.CreateItemAsync(
            It.IsAny<CreateCatalogItem>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteItemAsync_returns_false_when_item_not_found()
    {
        // Arrange
        _itemRepo
            .Setup(r => r.GetItemAsync(
                42,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatalogItem?)null); // item findes ikke

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteItemAsync(42);

        // Assert
        Assert.False(result);
        _itemRepo.Verify(r => r.DeleteItemAsync(
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
