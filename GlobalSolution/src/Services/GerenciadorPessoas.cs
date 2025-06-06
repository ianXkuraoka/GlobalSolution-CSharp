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
    /// Classe responsável por gerenciar o registro e localização de pessoas.
    /// </summary>
    public class GerenciadorPessoas
    {
        private List<Pessoa> _pessoas;
        private readonly GerenciadorEventos _gerenciadorEventos;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="GerenciadorPessoas"/>.
        /// </summary>
        /// <param name="gerenciadorEventos">Gerenciador responsável por registrar eventos do sistema.</param>
        public GerenciadorPessoas(GerenciadorEventos gerenciadorEventos)
        {
            _pessoas = new List<Pessoa>();
            _gerenciadorEventos = gerenciadorEventos ?? throw new ArgumentNullException(nameof(gerenciadorEventos));
        }

        /// <summary>
        /// Registra uma nova pessoa no sistema com dados básicos e biometria.
        /// </summary>
        /// <param name="nome">Nome completo da pessoa.</param>
        /// <param name="cpf">CPF da pessoa (11 dígitos).</param>
        /// <param name="dataNascimento">Data de nascimento da pessoa.</param>
        public void RegistrarPessoa(string nome, string cpf, DateTime dataNascimento)
        {
            try
            {
                ValidarDadosPessoa(nome, cpf, dataNascimento);

                var pessoa = new Pessoa
                {
                    Nome = nome.Trim(),
                    CPF = cpf.Trim(),
                    DataNascimento = dataNascimento,
                    BiometriaFacial = GerarHashBiometria(nome, cpf)
                };

                _pessoas.Add(pessoa);
                _gerenciadorEventos.RegistrarEvento(TipoEvento.RegistroPessoa,
                    $"Pessoa {nome} registrada com sucesso", pessoa.Id);

                Console.WriteLine($"Pessoa {nome} registrada com ID: {pessoa.Id}");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao registrar pessoa: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Busca uma pessoa com base na hash da biometria facial.
        /// </summary>
        /// <param name="hashBiometria">Hash gerada da biometria facial.</param>
        /// <returns>Pessoa encontrada ou null se não encontrada.</returns>
        public Pessoa BuscarPorBiometria(string hashBiometria)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hashBiometria))
                    throw new ArgumentException("Hash da biometria não pode estar vazio");

                var pessoa = _pessoas.FirstOrDefault(p => p.BiometriaFacial == hashBiometria);

                if (pessoa != null)
                {
                    pessoa.UltimaLocalizacao = DateTime.Now;
                    _gerenciadorEventos.RegistrarEvento(TipoEvento.DeteccaoBiometrica,
                        $"Pessoa {pessoa.Nome} detectada via biometria", pessoa.Id);
                }

                return pessoa;
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro na busca biométrica: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza a localização geográfica de uma pessoa registrada.
        /// </summary>
        /// <param name="pessoaId">Identificador da pessoa.</param>
        /// <param name="latitude">Latitude da nova localização.</param>
        /// <param name="longitude">Longitude da nova localização.</param>
        /// <param name="descricao">Descrição opcional da localização.</param>
        public void AtualizarLocalizacao(string pessoaId, double latitude, double longitude, string descricao = "")
        {
            try
            {
                ValidarCoordenadas(latitude, longitude);

                var pessoa = _pessoas.FirstOrDefault(p => p.Id == pessoaId);
                if (pessoa == null)
                    throw new ArgumentException($"Pessoa com ID {pessoaId} não encontrada");

                pessoa.Posicao = new Localizacao
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Descricao = descricao
                };
                pessoa.UltimaLocalizacao = DateTime.Now;

                Console.WriteLine($"Localização atualizada para {pessoa.Nome}: {latitude}, {longitude}");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao atualizar localização: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lista pessoas cuja última localização foi registrada há mais de 2 horas.
        /// </summary>
        /// <returns>Lista de pessoas potencialmente em risco.</returns>
        public List<Pessoa> ListarPessoasEmRisco()
        {
            var tempoLimite = DateTime.Now.AddHours(-2); // 2 horas sem contato
            return _pessoas.Where(p => p.UltimaLocalizacao < tempoLimite).ToList();
        }

        /// <summary>
        /// Retorna todas as pessoas registradas no sistema.
        /// </summary>
        /// <returns>Lista de pessoas.</returns>
        public List<Pessoa> ObterTodasPessoas() => new List<Pessoa>(_pessoas);

        /// <summary>
        /// Valida os dados fornecidos para o registro de uma nova pessoa.
        /// </summary>
        /// <param name="nome">Nome da pessoa.</param>
        /// <param name="cpf">CPF da pessoa.</param>
        /// <param name="dataNascimento">Data de nascimento da pessoa.</param>
        private void ValidarDadosPessoa(string nome, string cpf, DateTime dataNascimento)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode estar vazio");

            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                throw new ArgumentException("CPF deve ter 11 dígitos");

            if (dataNascimento > DateTime.Now)
                throw new ArgumentException("Data de nascimento não pode ser futura");

            if (dataNascimento < DateTime.Now.AddYears(-120))
                throw new ArgumentException("Data de nascimento inválida");

            if (_pessoas.Any(p => p.CPF == cpf))
                throw new InvalidOperationException("CPF já cadastrado no sistema");
        }

        /// <summary>
        /// Valida se as coordenadas de latitude e longitude são válidas.
        /// </summary>
        /// <param name="latitude">Latitude da localização.</param>
        /// <param name="longitude">Longitude da localização.</param>
        private void ValidarCoordenadas(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude deve estar entre -90 e 90 graus");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude deve estar entre -180 e 180 graus");
        }

        /// <summary>
        /// Gera um hash biométrico único baseado no nome, CPF e timestamp atual.
        /// </summary>
        /// <param name="nome">Nome da pessoa.</param>
        /// <param name="cpf">CPF da pessoa.</param>
        /// <returns>Hash biométrica codificada em base64.</returns>
        private string GerarHashBiometria(string nome, string cpf)
        {
            var dados = $"{nome}{cpf}{DateTime.Now.Ticks}";
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dados));
                return Convert.ToBase64String(bytes).Substring(0, 16);
            }
        }
    }
}
