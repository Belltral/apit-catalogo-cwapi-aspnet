using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers
{
    [EnableRateLimiting("Fixed")]
    [EnableCors("OrigensComAcessoPermitido")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        //private readonly IRepository<Categoria> _repository;
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;

        //public CategoriasController(IRepository<Categoria> repository, ILogger<CategoriasController> logger)
        //{
        //    _repository = repository;
        //    _logger = logger;
        //}

        public CategoriasController(IUnitOfWork uof, ILogger<CategoriasController> logger)
        {
            _uof = uof;
            _logger = logger;
        }

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.Count,
                categorias.PageSize,
                categorias.PageCount,
                categorias.TotalItemCount,
                categorias.HasNextPage,
                categorias.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet]
        //[Authorize]
        [DisableRateLimiting]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            _logger.LogInformation("====== GET api/categorias ======");

            var categorias = await _uof.CategoriaRepository.GetAllAsync();

            if (categorias is null)
                return NotFound("Não existem categorias cadastradas");

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

            return ObterCategorias(categorias);
        }

        [HttpGet("filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas([FromQuery] CategoriasFiltroNome categoriasFiltro)
        {
            var categoriasFiltradas = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltro);

            return ObterCategorias(categoriasFiltradas);
        }

        [DisableCors]
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            _logger.LogInformation($"====== GET api/categorias/id = {id} ======");

            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            var categoriaDto = categoria?.ToCategoriaDTO();

            return Ok(categoriaDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {
            _logger.LogInformation($"====== POST api/categorias ======");

            if (categoriaDto is null)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);

            await _uof.CommitAsync();

            var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDto?.CategoriaId }, novaCategoriaDto);
        }

        [HttpPut("{id:int}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            _logger.LogInformation($"====== PUT api/categorias/id ======");

            if (id != categoriaDto.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);

            await _uof.CommitAsync();

            var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

            return Ok(categoriaAtualizadaDto);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            _logger.LogInformation($"====== DELETE api/categorias/id ======");

            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria == null)
                return NotFound("Categoria não encontrada");

            var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);

            await _uof.CommitAsync();

            var categoriaExcluidaDto = categoriaExcluida.ToCategoriaDTO();

            return Ok(categoriaExcluidaDto);
        }
    }
}
