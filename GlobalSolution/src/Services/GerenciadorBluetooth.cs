using GlobalSolution.src.Enums;
using GlobalSolution.src.Models;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalSolution.src.Services
{
    /// <summary>
    /// Gerencia os dispositivos Bluetooth conectados, permitindo adicionar, desconectar e sincronizar dados entre eles.
    /// </summary>
    public class GerenciadorBluetooth
    {
        private List<DispositivoBluetooth> _dispositivos;
        private readonly GerenciadorEventos _gerenciadorEventos;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GerenciadorBluetooth"/>.
        /// </summary>
        /// <param name="gerenciadorEventos">Gerenciador de eventos para registrar logs e erros.</param>
        /// <exception cref="ArgumentNullException">Se o gerenciadorEventos for nulo.</exception>
        public GerenciadorBluetooth(GerenciadorEventos gerenciadorEventos)
        {
            _dispositivos = new List<DispositivoBluetooth>();
            _gerenciadorEventos = gerenciadorEventos ?? throw new ArgumentNullException(nameof(gerenciadorEventos));
        }

        /// <summary>
        /// Adiciona um novo dispositivo Bluetooth à lista de dispositivos ativos.
        /// </summary>
        /// <param name="nome">Nome do dispositivo Bluetooth.</param>
        /// <param name="endereco">Endereço MAC do dispositivo Bluetooth.</param>
        /// <exception cref="ArgumentException">Se nome ou endereço estiverem vazios ou nulos.</exception>
        /// <exception cref="InvalidOperationException">Se o dispositivo já estiver conectado.</exception>
        public void AdicionarDispositivo(string nome, string endereco)
        {
            try
            {
                ValidarDadosDispositivo(nome, endereco);

                var dispositivo = new DispositivoBluetooth
                {
                    Nome = nome.Trim(),
                    Endereco = endereco.Trim(),
                    Ativo = true
                };

                _dispositivos.Add(dispositivo);
                _gerenciadorEventos.RegistrarEvento(TipoEvento.ConexaoBluetooth,
                    $"Dispositivo {nome} adicionado à rede", dispositivo.Id);

                Console.WriteLine($"Dispositivo {nome} conectado à rede local");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao adicionar dispositivo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sincroniza dados para todos os dispositivos Bluetooth ativos, após verificar a integridade dos dados pelo checksum.
        /// </summary>
        /// <param name="dados">Dados a serem sincronizados.</param>
        /// <param name="checksum">Checksum para verificação de integridade dos dados.</param>
        /// <exception cref="ArgumentException">Se os dados estiverem vazios ou nulos.</exception>
        /// <exception cref="InvalidOperationException">Se a verificação do checksum falhar.</exception>
        public void SincronizarDados(string dados, string checksum)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dados))
                    throw new ArgumentException("Dados não podem estar vazios");

                if (!VerificarIntegridade(dados, checksum))
                    throw new InvalidOperationException("Falha na verificação de integridade dos dados");

                foreach (var dispositivo in _dispositivos.Where(d => d.Ativo))
                {
                    dispositivo.DadosCompartilhados.Add(dados);
                    dispositivo.UltimaConexao = DateTime.Now;
                }

                Console.WriteLine($"Dados sincronizados em {_dispositivos.Count(d => d.Ativo)} dispositivos");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro na sincronização: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtém a lista de dispositivos Bluetooth atualmente ativos.
        /// </summary>
        /// <returns>Lista de dispositivos ativos.</returns>
        public List<DispositivoBluetooth> ObterDispositivosAtivos()
        {
            return _dispositivos.Where(d => d.Ativo).ToList();
        }

        /// <summary>
        /// Desconecta um dispositivo Bluetooth pelo seu identificador.
        /// </summary>
        /// <param name="dispositivoId">Identificador único do dispositivo a ser desconectado.</param>
        /// <exception cref="ArgumentException">Se o dispositivo com o ID fornecido não for encontrado.</exception>
        public void DesconectarDispositivo(string dispositivoId)
        {
            try
            {
                var dispositivo = _dispositivos.FirstOrDefault(d => d.Id == dispositivoId);
                if (dispositivo == null)
                    throw new ArgumentException($"Dispositivo com ID {dispositivoId} não encontrado");

                dispositivo.Ativo = false;
                Console.WriteLine($"Dispositivo {dispositivo.Nome} desconectado");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao desconectar dispositivo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Valida os dados do dispositivo antes da conexão, garantindo nome e endereço válidos e que o dispositivo não esteja duplicado.
        /// </summary>
        /// <param name="nome">Nome do dispositivo.</param>
        /// <param name="endereco">Endereço MAC do dispositivo.</param>
        /// <exception cref="ArgumentException">Se nome ou endereço forem inválidos.</exception>
        /// <exception cref="InvalidOperationException">Se o dispositivo já estiver conectado.</exception>
        private void ValidarDadosDispositivo(string nome, string endereco)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do dispositivo não pode estar vazio");

            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentException("Endereço do dispositivo não pode estar vazio");

            if (_dispositivos.Any(d => d.Endereco == endereco))
                throw new InvalidOperationException("Dispositivo já conectado à rede");
        }

        /// <summary>
        /// Verifica a integridade dos dados comparando o hash SHA256 dos dados com o checksum fornecido.
        /// </summary>
        /// <param name="dados">Dados a serem verificados.</param>
        /// <param name="checksum">Checksum esperado para comparação.</param>
        /// <returns>True se o checksum corresponder ao hash dos dados; caso contrário, false.</returns>
        private bool VerificarIntegridade(string dados, string checksum)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(dados));
                var hashString = Convert.ToBase64String(hash);
                return hashString == checksum;
            }
        }

        /// <summary>
        /// Gera o checksum SHA256 para os dados fornecidos, codificado em Base64.
        /// </summary>
        /// <param name="dados">Dados para gerar o checksum.</param>
        /// <returns>Checksum em Base64.</returns>
        public string GerarChecksum(string dados)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(dados));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
