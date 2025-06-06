using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Models
{
    /// <summary>
    /// Representa uma coordenada geográfica registrada pelo sistema.
    /// </summary>
    public class Localizacao
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Descricao { get; set; }
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="Localizacao"/> com valores padrão.
        /// </summary>
        public Localizacao()
        {
            Latitude = 0.0;
            Longitude = 0.0;
            Descricao = string.Empty;
            Timestamp = DateTime.Now;
        }
    }
}
