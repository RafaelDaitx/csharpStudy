using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFConFin.Data;
using WFConFin.Models;

namespace WFConFin.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PessoaController : Controller
    {

        private readonly WFConFinDbContext _context;

        public PessoaController(WFConFinDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPessoa()
        {
            try
            {
                var result = _context.Pessoa.ToList();
                return result != null ? Ok(result) : NotFound("Não há pessoas cadastradas.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de pessoas {ex.Message}.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                await _context.Pessoa.AddAsync(pessoa);
                var valor = await _context.SaveChangesAsync();
                return valor == 1 ? Ok("Pessoa salva com sucesso.") : BadRequest("Erro, pessoa não incluída");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de pessoas: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                _context.Pessoa.Update(pessoa);
                var valor = await _context.SaveChangesAsync();
                return valor == 1 ? Ok("Pessoa alterada com sucesso.") : BadRequest("Erro, pessoa não alterada");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de pessoas {ex.Message}.");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeletePessoa([FromRoute] int id)
        {
            try
            {
                Pessoa pessoa = await _context.Pessoa.FindAsync(id);
                if (pessoa != null)
                {
                    _context.Pessoa.Remove(pessoa);
                    var valor = await _context.SaveChangesAsync();
                    return valor == 1 ? Ok("Pessoa removida com sucesso.") : BadRequest("Erro, pessoa não removida.");

                }
                else {
                    return BadRequest("Erro, pessoa não removida");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de pessoas {ex.Message}.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPessoa([FromRoute] Guid id)
        {
            try
            {
                var pessoa = await _context.Pessoa.FindAsync(id);

                return pessoa != null ? Ok(pessoa) : BadRequest("Erro ao consultar pessoa");


            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na consulta de pessoa. Exceção {ex.Message}");
            }
        }


        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetPessoaPesquisa([FromQuery] string valor)
        {
            try
            {
                //QUERY CRITERIA
                var lista = from o in _context.Pessoa.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Telefone.ToUpper().Contains(valor.ToUpper())
                            || o.Email.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de pesssoa. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetPessoaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {

                var lista = from o in _context.Pessoa.ToList()
                            where o.Nome.ToUpper().Contains(valor.ToUpper())
                            || o.Telefone.ToUpper().Contains(valor.ToUpper())
                            || o.Email.ToUpper().Contains(valor.ToUpper())
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

                var paginacaoResponse = new PaginacaoResponse<Pessoa>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de pessoa. Exceção: {ex.Message}");
            }
        }
    }
}
