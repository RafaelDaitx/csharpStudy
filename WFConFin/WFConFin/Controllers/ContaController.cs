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
    public class ContaController : Controller
    {

        private readonly WFConFinDbContext _context;

        public ContaController(WFConFinDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetConta()
        {
            try
            {
                var result = _context.Conta.ToList();
                return result != null ? Ok(result) : NotFound("Não há contas cadastradas.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de contas {ex.Message}.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostConta([FromBody] Conta conta)
        {
            try
            {
                await _context.Conta.AddAsync(conta);
                var valor = await _context.SaveChangesAsync();
                return valor == 1 ? Ok("Conta salva com sucesso.") : BadRequest("Erro, Conta não incluída");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de pessoas {ex.Message}.");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutConta([FromBody] Conta conta)
        {
            try
            {
                _context.Conta.Update(conta);
                var valor = await _context.SaveChangesAsync();
                return valor == 1 ? Ok("Conta alterada com sucesso.") : BadRequest("Erro, Conta não alterada");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de pessoas {ex.Message}.");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteConta([FromRoute] int id)
        {
            try
            {
                Conta conta = await _context.Conta.FindAsync(id);
                if (conta != null)
                {
                    _context.Conta.Remove(conta);
                    var valor = await _context.SaveChangesAsync();
                    return valor == 1 ? Ok("Conta removida com sucesso.") : BadRequest("Erro, Conta não removida.");

                }
                else
                {
                    return BadRequest("Erro, Conta não removida");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de Conta {ex.Message}.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConta([FromRoute] Guid id)
        {
            try
            {
                var conta = await _context.Conta.FindAsync(id);

                return conta != null ? Ok(conta) : BadRequest("Erro ao consultar conta");


            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na consulta de conta. Exceção {ex.Message}");
            }
        }


        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetContaPesquisa([FromQuery] string valor)
        {
            try
            {
                //QUERY CRITERIA
                var lista = from o in _context.Conta.Include(o => o.Pessoa).ToList()
                            where o.Descricao.ToUpper().Contains(valor.ToUpper())
                            || o.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de conta. Exceção: {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetContaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Conta.Include(o => o.Pessoa).ToList()
                            where o.Descricao.ToUpper().Contains(valor.ToUpper())
                            || o.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.DataPagamento descending
                            select o;
                }
                else
                {
                    lista = from o in lista
                            orderby o.DataPagamento ascending
                            select o;
                }

                var qtde = lista.Count();
                lista = lista
                            .Skip(skip)
                            .Take(take)
                            .ToList();

                var paginacaoResponse = new PaginacaoResponse<Conta>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa (pag) de Conta. Exceção: {ex.Message}");
            }
        }


        [HttpGet("Pessoa/{pessoaId}")]
        public async Task<IActionResult> GetContasPessoa([FromRoute] Guid pessoaId)
        {
            try
            {
                //QUERY CRITERIA
                var lista = from o in _context.Conta.ToList()
                            where o.PessoaId == pessoaId
                            select o;

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro, consulta de pesquisa de conta por pessoa. Exceção: {ex.Message}");
            }
        }
    }
}
