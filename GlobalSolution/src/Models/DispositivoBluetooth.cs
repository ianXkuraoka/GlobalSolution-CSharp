using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Models
{
    /// <summary>
    /// Representa um dispositivo Bluetooth detectado ou registrado pelo sistema.
    /// </summary>
    public class DispositivoBluetooth
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public bool Ativo { get; set; }
        public DateTime UltimaConexao { get; set; }
        public List <string> DadosCompartilhados { get; set; }

        /// <summary>
        /// Inicializa uma nova instância do dispositivo Bluetooth com ID único e configurações padrão.
        /// </summary>
        public DispositivoBluetooth()
        {
           Id = Guid.NewGuid().ToString();
            DadosCompartilhados = new List<string>();
            UltimaConexao = DateTime.Now;
        }
    }
}
