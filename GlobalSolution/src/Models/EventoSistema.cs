using GlobalSolution.src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Models
{
    /// <summary>
    /// Representa um evento registrado no sistema, como ações, falhas ou interações.
    /// </summary>
    public class EventoSistema
    {
        public string Id { get; set; }
        public TipoEvento Tipo { get; set; }
        public string Descricao { get; set; }
        public string ObjetoId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
