using APICatalogo.Models;
using APICatalogo.Pagination;
using X.PagedList;

namespace APICatalogo.Repositories.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParams);
    Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams);

    #region Implementação específica de Repository
    //IEnumerable<Categoria> GetCategorias();
    //Categoria GetCategoria(int id);
    //Categoria Create(Categoria categoria);
    //Categoria Update(Categoria categoria);
    //Categoria Delete(int id);
    #endregion
}
