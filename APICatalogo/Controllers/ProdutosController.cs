using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))] // Aplica a convenção de resposta em todos os métodos action do controlador
    public class ProdutosController : ControllerBase
    {
        //private readonly IProdutoRepository _produtoRepository;
        //private readonly IRepository<Produto> _repository;

        //public ProdutosController(IProdutoRepository produtoRepository, IRepository<Produto> repository)
        //{
        //    _repository = repository;
        //    _produtoRepository = produtoRepository;
        //}

        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.Count,
                produtos.PageSize,
                produtos.PageCount,
                produtos.TotalItemCount,
                produtos.HasNextPage,
                produtos.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet("produtos/{id}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

            if (produtos is null)
                return NotFound();

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);

            return ObterProdutos(produtos);
        }

        [HttpGet("filter/preco/pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFilterParams)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFilterParams);

            return ObterProdutos(produtos);
        }

        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <returns>Uma lista de objetos Produto</returns>
        //[Authorize(Policy = "UserOnly")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            try
            {
                var produtos = await _uof.ProdutoRepository.GetAllAsync();
                if (produtos is null)
                    return NotFound("Produtos não encontrados");

                var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

                return Ok(produtosDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtém um produto pelo seu ID
        /// </summary>
        /// <param name="id">ID do produto a ser localizado</param>
        /// <returns>Um objeto Produto</returns>
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            if (id == null || id <= 0)
            {
                return BadRequest("Id de produto inválido");
            }

            var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
            if (produto is null)
                return NotFound("Produto não encontrado");

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var novoProduto = _uof.ProdutoRepository.Create(produto);

            await _uof.CommitAsync();

            var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public async Task<ActionResult<ProdutoDTO>> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDto)
        {
            if (patchProdutoDto is null || id <= 0)
                return BadRequest();

            var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDto.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
                return BadRequest(ModelState);

            _mapper.Map(produtoUpdateRequest, produto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.CommitAsync();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
            await _uof.CommitAsync();

            var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

            return Ok(produtoAtualizadoDto);
            
            /*
            //bool atualizado = _repository.Update(produto);

            //if (atualizado)
            //{
            //    return Ok(produto);
            //}
            //else
            //{
            //    return StatusCode(500, $"Erro ao atualizar o produto de id {id}");
            //}
            */
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound();

            var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
            await _uof.CommitAsync();

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDto);

            /*
            //var deletado = _repository.Delete(id);

            //if (deletado)
            //{
            //    return Ok($"O produto {id} foi excluído");
            //}
            //else
            //{
            //    return NotFound($"Produto de id {id} não encontrado");
            //}
            */
        }
    }
}
