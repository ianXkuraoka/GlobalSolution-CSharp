using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Models
{
    /// <summary>
    /// Representa um usuário do sistema, contendo credenciais de autenticação.
    /// </summary>
    internal class Usuario
    {
        public string Nome { get; set; }
        public string SenhaHash { get; set; }
    }
}
