using APICatalogo.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests.ProdutosUnitTest;

public class DeleteProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public DeleteProdutosUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    [Fact]
    public async Task DeleteProduto_Result_OkResult()
    {
        //Assert
        var prodId = 30;

        //Act
        var result = await _controller.Delete(prodId);

        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteProduto_Result_NotFound()
    {
        //Assert
        var prodId = 99;

        //Act
        var result = await _controller.Delete(prodId);

        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
