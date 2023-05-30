using SbRotina.Enums;

namespace SbRotina.Models
{
    public class TarefaModel
    {
        public int Id { get; set; }

        public TipoTarefa TipoTarefa { get; set; }

        public PrioriddaeTarefa PrioriddaeTarefa { get; set; }

        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        public StatusTarefa Status { get; set; }

        public int? UsuarioId { get; set;}

        public virtual UsuarioModel? Usuario { get; set; }

    }
}
