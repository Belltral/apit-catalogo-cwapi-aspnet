using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using X.PagedList;

namespace APICatalogo.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
    {
        var produtos = await GetAllAsync();
        var produtosOrdenados = produtos.OrderBy(p => p.ProdutoId).AsQueryable();

        //var resultado = PagedList<Produto>.ToPagedList(produtosOrdenados, produtosParams.PageNumber, produtosParams.PageSize);

        var resultado = await produtos.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);

        return resultado;
    }

    public async Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams)
    {
        var produtos = await GetAllAsync();

        if(produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
        {
            if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco > produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
            else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
            {
                produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
            }
        }

        //var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos.AsQueryable(), produtosFiltroParams.PageNumber, produtosFiltroParams.PageSize);

        var produtosFiltrados = await produtos.ToPagedListAsync(produtosFiltroParams.PageNumber, produtosFiltroParams.PageSize);

        return produtosFiltrados;
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int categoriaId)
    {
        var produtos = await GetAllAsync();

        var produtosCategoria = produtos.Where(c => c.CategoriaId == categoriaId);

        return produtosCategoria;
    }


    #region Implementação específica de Repository
    //private readonly AppDbContext _context;

    //public ProdutosRepository(AppDbContext context)
    //{
    //    _context = context;
    //}

    //public IQueryable<Produto>? GetProdutos()
    //{
    //    return _context.Produtos;
    //}

    //public Produto GetProduto(int id)
    //{
    //    var produto = _context.Produtos?.FirstOrDefault(p => p.ProdutoId == id);

    //    if (produto is null)
    //        throw new InvalidOperationException("Produto não encontrado");

    //    return produto;
    //}

    //public Produto Create(Produto produto)
    //{
    //    if (produto is null)
    //        throw new InvalidOperationException("O produto é null");

    //    _context.Add(produto);
    //    _context.SaveChanges();

    //    return produto;
    //}

    //public bool Update(Produto produto)
    //{
    //    if (produto is null)
    //        throw new InvalidOperationException("O produto é null");

    //    if (_context.Produtos.Any(p => p.ProdutoId == produto.ProdutoId))
    //    {
    //        _context.Produtos.Update(produto);
    //        _context.SaveChanges();

    //        return true;
    //    }

    //    return false;
    //}

    //public bool Delete(int id)
    //{
    //    var produto = _context.Produtos.Find(id);

    //    if (produto is not null)
    //    {
    //        _context.Produtos.Remove(produto);
    //        _context.SaveChanges();

    //        return true;
    //    }

    //    return false;
    //}
    #endregion
}
