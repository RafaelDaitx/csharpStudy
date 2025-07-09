using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class Usuario
    {

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo Nome precisa ter entre 3 e 200 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Login é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Nome precisa ter entre 3 e 20 caracteres.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Nome precisa ter entre 3 e 20 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo Função é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O campo Nome precisa ter entre 3 e 20 caracteres.")]
        public string Funcao { get; set; }

        public Usuario()
        {
            Id = Guid.NewGuid();
        }
    }
}
