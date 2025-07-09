using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFConFin.Data;
using WFConFin.Models;


namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstadoController : Controller
    {
        private readonly WFConFinDbContext _context;

        public EstadoController(WFConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEstados()
        {//task -> processamento em multi thread
            try
            {
                var result = _context.Estado.ToList();

                return Ok(result);
            }
            catch (Exception ex) { 
                return BadRequest($"Erro na listagem de estados. Exceção: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostEstados([FromBody] Estado estado)
        {
            try
            {
                await _context.Estado.AddAsync(estado);
                var valor =await _context.SaveChangesAsync();

                return valor == 1 ? Ok("Sucesso. Estado incluido.") : BadRequest("Estado não incluído.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não incluido. Exceção: {ex.Message}");
            }
        }


        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutEstado([FromBody] Estado estado)
        {
            try
            {
                _context.Estado.Update(estado);
                var valor = await _context.SaveChangesAsync();

                return valor == 1 ? Ok("Sucesso. Estado alterado.") : BadRequest("Estado não alterado.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não alterado. Exceção: {ex.Message}");
            }
        }

        [HttpDelete("{sigla}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteEstado([FromRoute] string sigla )
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);
                if(estado!.Sigla == sigla && !string.IsNullOrEmpty(sigla))
                {
                    _context.Estado.Remove(estado);
                    var valor = await _context.SaveChangesAsync();

                    return valor == 1 ? Ok("Sucesso. Estado excluido com sucesso.") : BadRequest("Erro. Estado não excluido.");
                } else
                {
                    return NotFound("Erro. Estado não existe.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, estado não alterado. Exceção: {ex.Message}");
            }
        }

        [HttpGet("{sigla}")]
        public async Task<IActionResult> GetEstado([FromRoute] string sigla)
        {
            try
            {
                var estado = await _context.Estado.FindAsync(sigla);
                if (estado!.Sigla == sigla && !string.IsNullOrEmpty(sigla))
                {
                    return Ok(estado);
                }
                else
                {
                    return NotFound("Erro. Estado não existe.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de estado. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetEstadoPesquisa([FromQuery] string valor)
        {
            try
            {
                /* SELECT * FROM ESTADO WHERE SIGLA LIKE '%VALOR&' OR NOME LIKE '%VALOR&'; */

                //QUERY CRITERIA
                var lista = from o in _context.Estado.ToList()
                            where o.Sigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                //Fazendo pelo Entity 
                //_context.Estado.Where(o => o.Sigla.ToUpper().Contains(valor.ToUpper()) || o.Nome.ToUpper().Contains(valor.ToUpper())).ToList();

                //Fazendo pelo Expression
                //Expression<Func<Estado, bool>> expressao = o => true;
                //expressao = o => o.Sigla.ToUpper().Contains(valor.ToUpper()) || o.Nome.ToUpper().Contains(valor.ToUpper());

                //lista = _context.Estado.Where(expressao).ToList();

                return Ok(lista);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de estado. Exceção: {ex.Message}");
            }
        }


        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetEstadoPaginacao([FromQuery] string valor, int skip,int take, bool ordemDesc)
        {
            try
            {
              
                var lista = from o in _context.Estado.ToList()
                            where o.Sigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                if (ordemDesc)
                {
                    lista = from o in lista orderby o.Nome descending
                            select o;
                }
                else
                {
                    lista = from o in lista orderby o.Nome ascending
                            select o;
                }

                var qtde = lista.Count();
                lista = lista
                            .Skip(skip)
                            .Take(take)
                            .ToList();

                var paginacaoResponse = new PaginacaoResponse<Estado>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de estado. Exceção: {ex.Message}");
            }
        }
    }
}
