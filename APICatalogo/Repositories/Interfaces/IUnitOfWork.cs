namespace APICatalogo.Repositories.Interfaces;

public interface IUnitOfWork
{
    public IProdutoRepository ProdutoRepository { get; }
    public ICategoriaRepository CategoriaRepository { get; }
    Task CommitAsync();
}
