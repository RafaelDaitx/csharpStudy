using Microsoft.AspNetCore.Mvc;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {

        private static List<Estado> listaEstados = [];

        [HttpGet("estado")]
        public IActionResult GetEstados()
        {           
            return Ok(listaEstados);
        }

        [HttpPost("estado")]
        public IActionResult PostEstado([FromBody] Estado estado)
        {
            listaEstados.Add(estado);
            return Ok("Estado cadastrado. ");
        }

        [HttpGet]  
        public IActionResult GetInformacao()
        {
            var result = "retorno chamada";
            return Ok(result);
        }

        [HttpGet("info2")]
        public IActionResult GetInformacao2()
        {
            var result = "retorno chamada2";
            return Ok(result);
        }

        [HttpGet("info3/{valor}")]
        public IActionResult GetInformacao3([FromRoute] string valor)
        {
            var result = "retorno chamada2";
            return Ok(valor);
        }

        [HttpPost("info4")]
        public IActionResult GetInformacao4([FromHeader] string valor)
        {
            var result = "retorno chamada4 - valor: "+ valor;
            return Ok(result);
        }


        [HttpGet("info5")]
        public IActionResult GetInformacao5([FromHeader] string valor)
        {
            var result = "retorno chamada5 - valor: " + valor;
            return Ok(result);
        }

        [HttpPost("info6")]
        public IActionResult PostInformacao5([FromBody] Corpo corpo)
        {
            var result = "retorno chamada5 - valor: " + corpo.valor;
            return Ok(result);
        }
    }

    public class Corpo
    {
        public string valor { get; set; }
    }
}
