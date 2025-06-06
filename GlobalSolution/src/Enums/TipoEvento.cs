using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Enums
{
    /// <summary>
    /// Representa os tipos de eventos que podem ocorrer no sistema.
    /// </summary>
    public enum TipoEvento
    {
        RegistroPessoa,
        DeteccaoBiometrica,
        FalhaEnergia,
        ConexaoBluetooth,
        SincronizacaoNuvem,
        Erro
    }
}
