using GlobalSolution.src.Enums;
using GlobalSolution.src.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GlobalSolution.src.Services
{
    /// <summary>
    /// Gerencia eventos do sistema, permitindo registro, consulta e exportação.
    /// </summary>
    public class GerenciadorEventos
    {
        private List<EventoSistema> _eventos;

        /// <summary>
        /// Inicializa uma nova instância do gerenciador de eventos.
        /// </summary>
        public GerenciadorEventos()
        {
            _eventos = new List<EventoSistema>();
        }

        /// <summary>
        /// Registra um novo evento no sistema.
        /// </summary>
        /// <param name="tipo">Tipo do evento.</param>
        /// <param name="descricao">Descrição do evento.</param>
        /// <param name="objetoId">ID do objeto relacionado (opcional).</param>
        public void RegistrarEvento(TipoEvento tipo, string descricao, string objetoId = null)
        {
            var evento = new EventoSistema
            {
                Id = Guid.NewGuid().ToString(),
                Tipo = tipo,
                Descricao = descricao,
                ObjetoId = objetoId,
                Timestamp = DateTime.Now
            };

            _eventos.Add(evento);
        }

        /// <summary>
        /// Obtém os eventos registrados, podendo filtrar por tipo e data de início.
        /// </summary>
        /// <param name="tipo">Tipo do evento para filtrar (opcional).</param>
        /// <param name="dataInicio">Data inicial para filtrar eventos (opcional).</param>
        /// <returns>Lista de eventos filtrados e ordenados pela data decrescente.</returns>
        public List<EventoSistema> ObterEventos(TipoEvento? tipo = null, DateTime? dataInicio = null)
        {
            var query = _eventos.AsQueryable();

            if (tipo.HasValue)
                query = query.Where(e => e.Tipo == tipo.Value);

            if (dataInicio.HasValue)
                query = query.Where(e => e.Timestamp >= dataInicio.Value);

            return query.OrderByDescending(e => e.Timestamp).ToList();
        }

        /// <summary>
        /// Exporta os logs de eventos para um arquivo de texto.
        /// </summary>
        /// <param name="caminhoArquivo">Caminho do arquivo para salvar os logs.</param>
        public void ExportarLogs(string caminhoArquivo)
        {
            try
            {
                var logs = _eventos.OrderByDescending(e => e.Timestamp)
                                  .Select(e => $"{e.Timestamp:yyyy-MM-dd HH:mm:ss} [{e.Tipo}] {e.Descricao}")
                                  .ToList();

                File.WriteAllLines(caminhoArquivo, logs);
                Console.WriteLine($"Logs exportados para: {caminhoArquivo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar logs: {ex.Message}");
                throw;
            }
        }
    }
}
