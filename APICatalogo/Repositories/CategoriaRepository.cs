using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalogo.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
        
    }

    public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParams)
    {
        var categorias = await GetAllAsync();

        var categoriasOrdenadas = categorias.OrderBy(c => c.CategoriaId).AsQueryable();

        //var resultado = PagedList<Categoria>.ToPagedList(
        //    categoriasOrdenadas, 
        //    categoriasParams.PageNumber, 
        //    categoriasParams.PageSize);

        var resultado = await categorias.ToPagedListAsync(categoriasParams.PageNumber, categoriasParams.PageSize);

        return resultado;
    }

    public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams)
    {
        var categorias = await GetAllAsync();

        if (!string.IsNullOrEmpty(categoriasParams.Nome))
        {
            categorias = categorias.Where(c => c.Nome.Contains(categoriasParams.Nome));
        }

        // = PagedList<Categoria>.ToPagedList(
        //    categorias.AsQueryable(),
        //    categoriasParams.PageNumber,
        //    categoriasParams.PageSize);

        var categoriasFiltradas = await categorias.ToPagedListAsync(categoriasParams.PageNumber, categoriasParams.PageSize);

        return categoriasFiltradas;
    }


    #region Implementação específica de Repository
    //private readonly AppDbContext _context;

    //public CategoriaRepository(AppDbContext context)
    //{
    //    _context = context;
    //}

    //public IEnumerable<Categoria> GetCategorias()
    //{
    //    return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
    //}

    //public Categoria GetCategoria(int id)
    //{
    //    return _context.Categorias.AsNoTracking().FirstOrDefault(cat => cat.CategoriaId == id);
    //}

    //public Categoria Create(Categoria categoria)
    //{
    //    if (categoria is null)
    //        throw new ArgumentNullException(nameof(categoria));

    //    _context.Categorias?.Add(categoria);
    //    _context.SaveChanges();

    //    return categoria;
    //}

    //public Categoria Update(Categoria categoria)
    //{
    //    ArgumentNullException.ThrowIfNull(categoria);

    //    _context.Entry(categoria).State = EntityState.Modified;
    //    _context.SaveChanges();

    //    return categoria;
    //}

    //public Categoria Delete(int id)
    //{
    //    var categoria = _context.Categorias?.Find(id);

    //    ArgumentNullException.ThrowIfNull(categoria);

    //    _context.Remove(categoria);
    //    _context.SaveChanges();

    //    return categoria;
    //}
    #endregion
}
