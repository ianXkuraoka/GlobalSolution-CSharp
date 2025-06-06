using GlobalSolution.src.Enums;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalSolution.src.Services
{
    /// <summary>
    /// Classe responsável por gerar e salvar relatórios de status do sistema,
    /// incluindo informações sobre pessoas, falhas e dispositivos.
    /// </summary>
    public class GeradorRelatorios
    {
        private readonly GerenciadorPessoas _gerenciadorPessoas;
        private readonly GerenciadorFalhas _gerenciadorFalhas;
        private readonly GerenciadorBluetooth _gerenciadorBluetooth;
        private readonly GerenciadorEventos _gerenciadorEventos;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GeradorRelatorios"/>.
        /// </summary>
        /// <param name="gerenciadorPessoas">Gerenciador de pessoas cadastradas.</param>
        /// <param name="gerenciadorFalhas">Gerenciador das falhas do sistema.</param>
        /// <param name="gerenciadorBluetooth">Gerenciador dos dispositivos Bluetooth ativos.</param>
        /// <param name="gerenciadorEventos">Gerenciador para registrar eventos do sistema.</param>
        /// <exception cref="ArgumentNullException">Se algum dos gerenciadores for nulo.</exception>
        public GeradorRelatorios(GerenciadorPessoas gerenciadorPessoas,
                               GerenciadorFalhas gerenciadorFalhas,
                               GerenciadorBluetooth gerenciadorBluetooth,
                               GerenciadorEventos gerenciadorEventos)
        {
            _gerenciadorPessoas = gerenciadorPessoas ?? throw new ArgumentNullException(nameof(gerenciadorPessoas));
            _gerenciadorFalhas = gerenciadorFalhas ?? throw new ArgumentNullException(nameof(gerenciadorFalhas));
            _gerenciadorBluetooth = gerenciadorBluetooth ?? throw new ArgumentNullException(nameof(gerenciadorBluetooth));
            _gerenciadorEventos = gerenciadorEventos ?? throw new ArgumentNullException(nameof(gerenciadorEventos));
        }

        /// <summary>
        /// Gera um relatório detalhado do status atual do sistema,
        /// incluindo estatísticas gerais, pessoas em risco e falhas ativas.
        /// </summary>
        /// <returns>Relatório formatado como string.</returns>
        /// <exception cref="Exception">Lança exceção caso ocorra erro na geração do relatório.</exception>
        public string GerarRelatorioStatus()
        {
            try
            {
                var pessoas = _gerenciadorPessoas.ObterTodasPessoas();
                var falhasAtivas = _gerenciadorFalhas.ObterFalhasAtivas();
                var dispositivosAtivos = _gerenciadorBluetooth.ObterDispositivosAtivos();
                var pessoasEmRisco = _gerenciadorPessoas.ListarPessoasEmRisco();

                var relatorio = new StringBuilder();
                relatorio.AppendLine("=== RELATÓRIO DE STATUS DO SISTEMA ===");
                relatorio.AppendLine($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                relatorio.AppendLine();

                relatorio.AppendLine("=== ESTATÍSTICAS GERAIS ===");
                relatorio.AppendLine($"Total de pessoas cadastradas: {pessoas.Count}");
                relatorio.AppendLine($"Pessoas em risco: {pessoasEmRisco.Count}");
                relatorio.AppendLine($"Falhas ativas: {falhasAtivas.Count}");
                relatorio.AppendLine($"Dispositivos conectados: {dispositivosAtivos.Count}");
                relatorio.AppendLine();

                if (pessoasEmRisco.Any())
                {
                    relatorio.AppendLine("=== PESSOAS EM RISCO ===");
                    foreach (var pessoa in pessoasEmRisco)
                    {
                        var tempoSemContato = DateTime.Now - pessoa.UltimaLocalizacao;
                        relatorio.AppendLine($"- {pessoa.Nome} (CPF: {pessoa.CPF})");
                        relatorio.AppendLine($"  Último contato: {pessoa.UltimaLocalizacao:yyyy-MM-dd HH:mm:ss}");
                        relatorio.AppendLine($"  Tempo sem contato: {tempoSemContato.TotalHours:F1} horas");
                        relatorio.AppendLine();
                    }
                }

                if (falhasAtivas.Any())
                {
                    relatorio.AppendLine("=== FALHAS ATIVAS ===");
                    foreach (var falha in falhasAtivas)
                    {
                        var duracao = DateTime.Now - falha.DataInicio;
                        relatorio.AppendLine($"- {falha.Regiao} ({falha.Tipo})");
                        relatorio.AppendLine($"  Início: {falha.DataInicio:yyyy-MM-dd HH:mm:ss}");
                        relatorio.AppendLine($"  Duração: {duracao.TotalHours:F1} horas");
                        relatorio.AppendLine($"  Descrição: {falha.Descricao}");
                        relatorio.AppendLine();
                    }
                }

                return relatorio.ToString();
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao gerar relatório: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Salva o relatório fornecido em um arquivo de texto com timestamp no nome.
        /// </summary>
        /// <param name="relatorio">Conteúdo do relatório a ser salvo.</param>
        /// <param name="nomeArquivo">Nome base do arquivo onde o relatório será salvo.</param>
        /// <exception cref="Exception">Lança exceção caso ocorra erro ao salvar o arquivo.</exception>
        public void SalvarRelatorio(string relatorio, string nomeArquivo)
        {
            try
            {
                var caminhoCompleto = $"{nomeArquivo}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                File.WriteAllText(caminhoCompleto, relatorio);
                Console.WriteLine($"Relatório salvo em: {caminhoCompleto}");
            }
            catch (Exception ex)
            {
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro,
                    $"Erro ao salvar relatório: {ex.Message}");
                throw;
            }
        }
    }
}
