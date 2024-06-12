using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    //IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParams);

    Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams);
    Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams);
    Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int categoriaId);

    #region Implementação específica de Repository
    //IQueryable<Produto>? GetProdutos();
    //Produto GetProduto(int id);
    //Produto Create(Produto produto);
    //bool Update(Produto produto);
    //bool Delete(int id);
    #endregion
}
