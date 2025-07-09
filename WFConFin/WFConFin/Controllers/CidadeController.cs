using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CidadeController : Controller
    {

        private readonly WFConFinDbContext _context;

        public CidadeController(WFConFinDbContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetCidades()
        {
            try
            {
                //Include traz no json os dados do Estado, omo se estivesse fazendo um join, conforme escrito na model
                //var result = _context.Cidade.Include(x => x.Estado).ToList();

                var result = _context.Cidade.ToList();
                return Ok(result);

            } catch (Exception ex)
            { 
                return BadRequest($"Erro na listagem de cidades. Exceção {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostCidade([FromBody] Cidade cidade)
        {
            try
            {
                await _context.Cidade.AddAsync(cidade);
                var valor = await _context.SaveChangesAsync();

                return valor == 1 ? Ok("Sucesso, cidade cadastrada.") : BadRequest("Erro no cadastro da cidade");

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de cidade : {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutCidade([FromBody] Cidade cidade)
        {
            try
            {
                _context.Cidade.Update(cidade);
                var valor = await _context.SaveChangesAsync();

                return valor == 1 ? Ok("Sucesso, cidade alterada.") : BadRequest("Erro na alteração da cidade");

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusãso de cidade. Exceção {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteCidade([FromRoute] Guid id)
        {
            try
            {
               var cidade = await _context.Cidade.FindAsync(id);

                if(cidade != null)
                {
                    _context.Cidade.Remove(cidade);
                    var valor = await _context.SaveChangesAsync();

                    return valor == 1 ? Ok("Sucesso, cidade deletada.") : BadRequest("Erro ao deletar cidade");
                } else
                {
                    return BadRequest("Erro. Cidade não existe.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusãso de cidade. Exceção {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCidade([FromRoute] Guid id)
        {
            try
            {
                var cidade = await _context.Cidade.FindAsync(id);

                return cidade != null ? Ok(cidade) : BadRequest("Erro ao consultar cidade");
               

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na consulta de cidade. Exceção {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetCidadePesquisa([FromQuery] string valor)
        {
            try
            {
                //QUERY CRITERIA
                var lista = from o in _context.Cidade.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de estado. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetCidadePaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {

                var lista = from o in _context.Cidade.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                            select o;

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.Nome descending
                            select o;
                }
                else
                {
                    lista = from o in lista
                            orderby o.Nome ascending
                            select o;
                }

                var qtde = lista.Count();
                lista = lista
                            .Skip(skip)
                            .Take(take)
                            .ToList();

                var paginacaoResponse = new PaginacaoResponse<Cidade>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de cidade. Exceção: {ex.Message}");
            }
        }

    }
}
