using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WFConFin.Data;
using WFConFin.Models;
using WFConFin.Services;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : Controller
    {

        private readonly WFConFinDbContext _context;
        private readonly TokenService _service;

        public UsuarioController(WFConFinDbContext context, TokenService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UsuarioLogin usuarioLogin)
        {
            var usuario = _context.Usuario.Where(x => x.Login == usuarioLogin.Login).FirstOrDefault();
            if (usuario == null) return NotFound("Usuário inválido.");

            if (usuario.Password != usuarioLogin.Password) return BadRequest("Senha inválida.");

            var token = _service.GerarToken(usuario);

            usuario.Password = "";

            var result = new UsuarioResponse()
            {
                Usuario = usuario,
                Token = token
            };

            return Ok(result);
            
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var result = _context.Usuario.ToList();
                return result != null ? Ok(result) : NotFound("Não há usuários cadastrados.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de usuários {ex.Message}.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            try
            {
                var listUsuario = _context.Usuario.Where(x => x.Login == usuario.Login).ToList();

                if (listUsuario.Count > 0)
                {
                    return BadRequest($"Login inválido.");
                }

                await _context.Usuario.AddAsync(usuario);
                var valor = await _context.SaveChangesAsync();
                return valor == 1 ? Ok("Usuario salvo com sucesso.") : BadRequest("Erro, Usuario não incluído");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na inclusão de Usuarios {ex.Message}.");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Gerente,Empregado")]
        public async Task<IActionResult> PutUsuario([FromBody] Usuario usuario)
        {
            try
            {
                _context.Usuario.Update(usuario);
                var valor = await _context.SaveChangesAsync();
                return valor == 1 ? Ok("Conta alterada com sucesso.") : BadRequest("Erro, usuario não alterada");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de Usuarios {ex.Message}.");
            }
        }


        [HttpDelete]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteUsuario([FromRoute] int id)
        {
            try
            {
                Usuario usuario = await _context.Usuario.FindAsync(id);
                if (usuario != null)
                {
                    _context.Usuario.Remove(usuario);
                    var valor = await _context.SaveChangesAsync();
                    return valor == 1 ? Ok("usuario removida com sucesso.") : BadRequest("Erro, usuario não removida.");

                }
                else
                {
                    return BadRequest("Erro, usuario não removida");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na alteração de Conta {ex.Message}.");
            }

        }

        public override bool Equals(object obj)
        {
            return obj is UsuarioController controller &&
                   EqualityComparer<WFConFinDbContext>.Default.Equals(_context, controller._context);
        }
    }
}