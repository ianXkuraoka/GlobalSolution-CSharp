using GlobalSolution.src.Enums;
using GlobalSolution.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSolution.src.Services
{
    /// <summary>
    /// Gerencia o registro e finalização de falhas de energia.
    /// </summary>
    public class GerenciadorFalhas
    {
        private List<FalhaEnergia> _falhas;
        private readonly GerenciadorEventos _gerenciadorEventos;

        /// <summary>
        /// Inicializa uma nova instância do gerenciador de falhas.
        /// </summary>
        /// <param name="gerenciadorEventos">Gerenciador de eventos para log.</param>
        public GerenciadorFalhas(GerenciadorEventos gerenciadorEventos)
        {
            _falhas = new List<FalhaEnergia>();
            _gerenciadorEventos = gerenciadorEventos ?? throw new ArgumentNullException(nameof(gerenciadorEventos));
        }

        /// <summary>
        /// Registra uma nova falha de energia.
        /// </summary>
        /// <param name="regiao">Região afetada.</param>
        /// <param name="tipo">Tipo da falha.</param>
        /// <param name="descricao">Descrição da falha.</param>
        public void RegistrarFalha(string regiao, TipoFalha tipo, string descricao)
        {
            try
            {
                ValidarDadosFalha(regiao, descricao);

                var falha = new FalhaEnergia
                {
                    Regiao = regiao.Trim(),
                    Tipo = tipo,
                    Descricao = descricao.Trim()
                };

                _falhas.Add(falha);
                _gerenciadorEventos.RegistrarEvento(TipoEvento.FalhaEnergia,
                    $"Falha de energia registrada em {regiao}: {tipo}", falha.Id);

                Console.WriteLine($"Falha de energia registrada: {regiao} - {tipo}");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao registrar falha: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Finaliza uma falha de energia com base no ID.
        /// </summary>
        /// <param name="falhaId">ID da falha a ser finalizada.</param>
        public void FinalizarFalha(string falhaId)
        {
            try
            {
                var falha = _falhas.FirstOrDefault(f => f.Id == falhaId);
                if (falha == null)
                    throw new ArgumentException($"Falha com ID {falhaId} não encontrada");

                if (falha.DataFim.HasValue)
                    throw new InvalidOperationException("Falha já foi finalizada");

                falha.DataFim = DateTime.Now;
                Console.WriteLine($"Falha finalizada. Duração: {falha.DuracaoFalha?.TotalHours:F2} horas");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao finalizar falha: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtém uma lista de falhas de energia que ainda não foram finalizadas.
        /// </summary>
        /// <returns>Lista de falhas ativas.</returns>
        public List<FalhaEnergia> ObterFalhasAtivas()
        {
            return _falhas.Where(f => !f.DataFim.HasValue).ToList();
        }

        /// <summary>
        /// Retorna todas as falhas registradas (ativas e finalizadas).
        /// </summary>
        /// <returns>Lista de todas as falhas.</returns>
        public List<FalhaEnergia> ObterTodasFalhas() => new List<FalhaEnergia>(_falhas);

        /// <summary>
        /// Valida os dados fornecidos para o registro de falha.
        /// </summary>
        /// <param name="regiao">Região da falha.</param>
        /// <param name="descricao">Descrição da falha.</param>
        private void ValidarDadosFalha(string regiao, string descricao)
        {
            if (string.IsNullOrWhiteSpace(regiao))
                throw new ArgumentException("Região não pode estar vazia");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição não pode estar vazia");
        }
    }
}
