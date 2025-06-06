using GlobalSolution.src.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Models
{
    /// <summary>
    /// Representa uma pessoa registrada no sistema, incluindo dados pessoais e localização.
    /// </summary>
    public class Pessoa
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string BiometriaFacial { get; set; }
        public DateTime UltimaLocalizacao { get; set; }
        public Localizacao Posicao { get; set; }
        public StatusPessoa Status {  get; set; }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="Pessoa"/> com valores padrão.
        /// </summary>
        public Pessoa() 
        {
            Id = Guid.NewGuid().ToString();
            Status = StatusPessoa.Desconhecido;
            UltimaLocalizacao = DateTime.Now;
        }
    }
}
