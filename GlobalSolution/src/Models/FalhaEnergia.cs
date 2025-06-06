using GlobalSolution.src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Models
{
    /// <summary>
    /// Representa uma ocorrência de falha de energia registrada pelo sistema.
    /// </summary>
    public class FalhaEnergia
    {
        public string Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Regiao { get; set; }
        public TipoFalha Tipo { get; set; }
        public int PessoasAfetadas { get; set; }
        public string Descricao { get; set; }

        /// <summary>
        /// Inicializa uma nova instância da falha de energia com valores padrão.
        /// </summary>
        public FalhaEnergia()
        {
            Id = Guid.NewGuid().ToString();
            DataInicio = DateTime.Now;
        }

        /// <summary>
        /// Duração da falha de energia, calculada com base no início e fim (se disponível).
        /// </summary>
        public TimeSpan? DuracaoFalha => DataFim?.Subtract(DataInicio);
    }
}
