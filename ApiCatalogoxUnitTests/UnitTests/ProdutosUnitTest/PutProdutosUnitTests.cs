using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests.ProdutosUnitTest;

public class PutProdutosUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

	public PutProdutosUnitTests(ProdutosUnitTestController controller)
	{
		_controller = new ProdutosController(controller.repository, controller.mapper);
	}

	[Fact]
	public async Task PutProdutos_Return_OkResult()
	{
		//Arrange
		var prodId = 30;
        var produtoAtualizado = new ProdutoDTO
        {
            ProdutoId = prodId,
            Nome = "Produto Atualizado",
            Descricao = "Descrição Atualizada",
            Preco = 10,
            ImagemUrl = "imagem.png",
            CategoriaId = 2
        };

        //Act
        var result = await _controller.Put(prodId, produtoAtualizado) as ActionResult<ProdutoDTO>;

        //Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();

    }

    [Fact]
    public async Task PutProdutos_Return_BadRequest()
    {
        //Arrange
        var prodId = 31;
        var produtoAtualizado = new ProdutoDTO
        {
            ProdutoId = 30,
            Nome = "Produto Atualizado",
            Descricao = "Descrição Atualizada",
            Preco = 10,
            ImagemUrl = "imagem.png",
            CategoriaId = 2
        };

        //Act
        var data = await _controller.Put(prodId, produtoAtualizado);

        //Assert
        data.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400);
    }
}
