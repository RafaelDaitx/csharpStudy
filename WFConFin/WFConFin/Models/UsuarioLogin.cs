using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class UsuarioLogin
    {

        [Required(ErrorMessage = "O campo Login é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Nome precisa ter entre 3 e 20 caracteres.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Nome precisa ter entre 3 e 20 caracteres.")]
        public string Password { get; set; }
    }
}
