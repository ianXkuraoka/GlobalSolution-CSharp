using GlobalSolution.src.Enums;
using GlobalSolution.src.Services;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmergencyMonitoringSystem.Services;

namespace EmergencyMonitoringSystem
{

    /// <summary>
    /// Classe principal do sistema de monitoramento de emergências, responsável por inicializar 
    /// os gerenciadores e coordenar as funcionalidades do sistema.
    /// </summary>
    public class SistemaMonitoramentoEmergencia
    {
        private readonly GerenciadorEventos _gerenciadorEventos;
        private readonly GerenciadorPessoas _gerenciadorPessoas;
        private readonly GerenciadorBluetooth _gerenciadorBluetooth;
        private readonly GerenciadorFalhas _gerenciadorFalhas;
        private readonly GeradorRelatorios _geradorRelatorios;

        /// <summary>
        /// Construtor da classe que inicializa todos os gerenciadores utilizados no sistema.
        /// </summary>
        public SistemaMonitoramentoEmergencia()
        {
            _gerenciadorEventos = new GerenciadorEventos();
            _gerenciadorPessoas = new GerenciadorPessoas(_gerenciadorEventos);
            _gerenciadorBluetooth = new GerenciadorBluetooth(_gerenciadorEventos);
            _gerenciadorFalhas = new GerenciadorFalhas(_gerenciadorEventos);
            _geradorRelatorios = new GeradorRelatorios(_gerenciadorPessoas, _gerenciadorFalhas,
                                                     _gerenciadorBluetooth, _gerenciadorEventos);
        }

        /// <summary>
        /// Inicializa o sistema e registra o evento de inicialização.
        /// </summary>
        public void IniciarSistema()
        {
            Console.WriteLine("=== SISTEMA DE MONITORAMENTO DE EMERGÊNCIA ===");
            Console.WriteLine("Sistema iniciado com sucesso!");
            Console.WriteLine();

            _gerenciadorEventos.RegistrarEvento(TipoEvento.RegistroPessoa, "Sistema inicializado");
        }

        /// <summary>
        /// Demonstra todas as funcionalidades do sistema, como registro de pessoas,
        /// falhas, dispositivos Bluetooth e geração de relatórios.
        /// </summary>
        public void DemonstrarFuncionalidades()
        {
            try
            {
                Console.WriteLine("=== DEMONSTRAÇÃO DAS FUNCIONALIDADES ===");
                Console.WriteLine();

                // 1. Registro de pessoas
                Console.WriteLine("1. REGISTRANDO PESSOAS...");
                _gerenciadorPessoas.RegistrarPessoa("João Silva", "12345678901", new DateTime(1990, 5, 15));
                _gerenciadorPessoas.RegistrarPessoa("Maria Santos", "98765432100", new DateTime(1985, 8, 22));
                _gerenciadorPessoas.RegistrarPessoa("Pedro Oliveira", "11122233344", new DateTime(1992, 12, 3));
                Console.WriteLine();

                // 2. Adicionando dispositivos Bluetooth
                Console.WriteLine("2. CONECTANDO DISPOSITIVOS BLUETOOTH...");
                _gerenciadorBluetooth.AdicionarDispositivo("Celular-João", "AA:BB:CC:DD:EE:01");
                _gerenciadorBluetooth.AdicionarDispositivo("Celular-Maria", "AA:BB:CC:DD:EE:02");
                _gerenciadorBluetooth.AdicionarDispositivo("Tablet-Pedro", "AA:BB:CC:DD:EE:03");
                Console.WriteLine();

                // 3. Registrando falhas de energia
                Console.WriteLine("3. REGISTRANDO FALHAS DE ENERGIA...");
                _gerenciadorFalhas.RegistrarFalha("Centro", TipoFalha.FalhaTotal, "Queda de árvore na rede elétrica");
                _gerenciadorFalhas.RegistrarFalha("Zona Sul", TipoFalha.Sobrecarga, "Sobrecarga na subestação");
                Console.WriteLine();

                // 4. Atualizando localizações
                Console.WriteLine("4. ATUALIZANDO LOCALIZAÇÕES...");
                var pessoas = _gerenciadorPessoas.ObterTodasPessoas();
                if (pessoas.Count > 0)
                {
                    _gerenciadorPessoas.AtualizarLocalizacao(pessoas[0].Id, -23.5505, -46.6333, "São Paulo - Centro");
                    _gerenciadorPessoas.AtualizarLocalizacao(pessoas[1].Id, -23.5629, -46.6544, "São Paulo - Vila Madalena");
                }
                Console.WriteLine();

                // 5. Sincronizando dados
                Console.WriteLine("5. SINCRONIZANDO DADOS VIA BLUETOOTH...");
                var dados = JsonSerializer.Serialize(new { pessoas = pessoas.Count, timestamp = DateTime.Now });
                var checksum = _gerenciadorBluetooth.GerarChecksum(dados);
                _gerenciadorBluetooth.SincronizarDados(dados, checksum);
                Console.WriteLine();

                // 6. Gerando relatório
                Console.WriteLine("6. GERANDO RELATÓRIO DE STATUS...");
                var relatorio = _geradorRelatorios.GerarRelatorioStatus();
                Console.WriteLine(relatorio);

                // Demonstrando tratamento de erros
                Console.WriteLine("=== DEMONSTRAÇÃO DE TRATAMENTO DE ERROS ===");
                DemonstrarTratamentoErros();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante demonstração: {ex.Message}");
                _gerenciadorEventos.RegistrarEvento(TipoEvento.Erro, $"Erro na demonstração: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza testes com dados inválidos para demonstrar o tratamento de erros no sistema.
        /// </summary>
        private void DemonstrarTratamentoErros()
        {
            Console.WriteLine("\n--- Testando validações e tratamento de erros ---");

            // Teste 1: CPF inválido
            try
            {
                _gerenciadorPessoas.RegistrarPessoa("Teste", "123", DateTime.Now.AddYears(-30));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Erro capturado - CPF inválido: {ex.Message}");
            }

            // Teste 2: Data futura
            try
            {
                _gerenciadorPessoas.RegistrarPessoa("Teste", "12345678900", DateTime.Now.AddDays(1));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Erro capturado - Data futura: {ex.Message}");
            }

            // Teste 3: Coordenadas inválidas
            try
            {
                var pessoas = _gerenciadorPessoas.ObterTodasPessoas();
                if (pessoas.Count > 0)
                    _gerenciadorPessoas.AtualizarLocalizacao(pessoas[0].Id, 200, 300, "Coordenadas inválidas");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Erro capturado - Coordenadas inválidas: {ex.Message}");
            }

            // Teste 4: Campo vazio
            try
            {
                _gerenciadorFalhas.RegistrarFalha("", TipoFalha.FalhaTotal, "Teste");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Erro capturado - Campo vazio: {ex.Message}");
            }

            // Teste 5: Dispositivo duplicado
            try
            {
                _gerenciadorBluetooth.AdicionarDispositivo("Dispositivo Teste", "AA:BB:CC:DD:EE:01"); // Endereço já existe
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"✓ Erro capturado - Dispositivo duplicado: {ex.Message}");
            }

            Console.WriteLine("--- Fim dos testes de erro ---\n");
        }

        /// <summary>
        /// Exibe o menu principal de opções e executa a funcionalidade correspondente de acordo com a escolha do usuário.
        /// </summary>
        public void ExibirMenu()
        {

            bool continuar = true;

            while (continuar)
            {
                Console.WriteLine("\n=== MENU DO SISTEMA ===");
                Console.WriteLine("1. Registrar Pessoa");
                Console.WriteLine("2. Registrar Falha de Energia");
                Console.WriteLine("3. Conectar Dispositivo Bluetooth");
                Console.WriteLine("4. Atualizar Localização");
                Console.WriteLine("5. Gerar Relatório de Status");
                Console.WriteLine("6. Exportar Logs");
                Console.WriteLine("7. Listar Pessoas em Risco");
                Console.WriteLine("8. Sincronizar Dados");
                Console.WriteLine("9. Demonstração Completa");
                Console.WriteLine("0. Sair");
                Console.Write("Escolha uma opção: ");

                try
                {
                    var opcao = Console.ReadLine();

                    switch (opcao)
                    {
                        case "1":
                            MenuRegistrarPessoa();
                            break;
                        case "2":
                            MenuRegistrarFalha();
                            break;
                        case "3":
                            MenuConectarDispositivo();
                            break;
                        case "4":
                            MenuAtualizarLocalizacao();
                            break;
                        case "5":
                            MenuGerarRelatorio();
                            break;
                        case "6":
                            MenuExportarLogs();
                            break;
                        case "7":
                            MenuListarPessoasRisco();
                            break;
                        case "8":
                            MenuSincronizarDados();
                            break;
                        case "9":
                            DemonstrarFuncionalidades();
                            break;
                        case "0":
                            continuar = false;
                            Console.WriteLine("Sistema encerrado.");
                            break;
                        default:
                            Console.WriteLine("Opção inválida!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Menu que permite o registro de uma nova pessoa no sistema.
        /// </summary>
        private void MenuRegistrarPessoa()
        {
            try
            {
                Console.Write("Nome: ");
                var nome = Console.ReadLine();

                Console.Write("CPF (11 dígitos): ");
                var cpf = Console.ReadLine();

                Console.Write("Data de nascimento (dd/MM/yyyy): ");
                var dataStr = Console.ReadLine();

                if (DateTime.TryParseExact(dataStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                {
                    _gerenciadorPessoas.RegistrarPessoa(nome, cpf, data);
                }
                else
                {
                    throw new ArgumentException("Data inválida. Use o formato dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar pessoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu para registrar uma nova falha de energia informando região, tipo e descrição.
        /// </summary>
        private void MenuRegistrarFalha()
        {
            try
            {
                Console.Write("Região: ");
                var regiao = Console.ReadLine();

                Console.WriteLine("Tipos de falha:");
                Console.WriteLine("1. Falha Total");
                Console.WriteLine("2. Falha Parcial");
                Console.WriteLine("3. Sobrecarga");
                Console.WriteLine("4. Manutenção");
                Console.WriteLine("5. Catástrofe");
                Console.Write("Escolha o tipo (1-5): ");

                var tipoStr = Console.ReadLine();
                if (!int.TryParse(tipoStr, out int tipoInt) || tipoInt < 1 || tipoInt > 5)
                {
                    throw new ArgumentException("Tipo de falha inválido");
                }

                var tipo = (TipoFalha)(tipoInt - 1);

                Console.Write("Descrição: ");
                var descricao = Console.ReadLine();

                _gerenciadorFalhas.RegistrarFalha(regiao, tipo, descricao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao registrar falha: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu para conectar um novo dispositivo Bluetooth ao sistema.
        /// </summary>
        private void MenuConectarDispositivo()
        {
            try
            {
                Console.Write("Nome do dispositivo: ");
                var nome = Console.ReadLine();

                Console.Write("Endereço MAC (formato AA:BB:CC:DD:EE:FF): ");
                var endereco = Console.ReadLine();

                _gerenciadorBluetooth.AdicionarDispositivo(nome, endereco);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar dispositivo: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu que permite atualizar a localização de uma pessoa registrada.
        /// </summary>
        private void MenuAtualizarLocalizacao()
        {
            try
            {
                var pessoas = _gerenciadorPessoas.ObterTodasPessoas();
                if (!pessoas.Any())
                {
                    Console.WriteLine("Nenhuma pessoa cadastrada.");
                    return;
                }

                Console.WriteLine("Pessoas cadastradas:");
                for (int i = 0; i < pessoas.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {pessoas[i].Nome} (CPF: {pessoas[i].CPF})");
                }

                Console.Write("Escolha uma pessoa (número): ");
                var escolhaStr = Console.ReadLine();

                if (!int.TryParse(escolhaStr, out int escolha) || escolha < 1 || escolha > pessoas.Count)
                {
                    throw new ArgumentException("Escolha inválida");
                }

                var pessoa = pessoas[escolha - 1];

                Console.Write("Latitude (-90 a 90): ");
                var latStr = Console.ReadLine();

                Console.Write("Longitude (-180 a 180): ");
                var lngStr = Console.ReadLine();

                Console.Write("Descrição (opcional): ");
                var descricao = Console.ReadLine();

                if (!double.TryParse(latStr, out double latitude) || !double.TryParse(lngStr, out double longitude))
                {
                    throw new ArgumentException("Coordenadas inválidas");
                }

                _gerenciadorPessoas.AtualizarLocalizacao(pessoa.Id, latitude, longitude, descricao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar localização: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu que gera um relatório de status do sistema baseado nos dados atuais.
        /// </summary>
        private void MenuGerarRelatorio()
        {
            try
            {
                var relatorio = _geradorRelatorios.GerarRelatorioStatus();
                Console.WriteLine(relatorio);

                Console.Write("Deseja salvar o relatório em arquivo? (s/n): ");
                var resposta = Console.ReadLine();

                if (resposta?.ToLower() == "s")
                {
                    _geradorRelatorios.SalvarRelatorio(relatorio, "relatorio_status");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu para exportar os logs registrados durante o funcionamento do sistema.
        /// </summary>
        private void MenuExportarLogs()
        {
            try
            {
                var nomeArquivo = $"logs_sistema_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                _gerenciadorEventos.ExportarLogs(nomeArquivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exportar logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu para listar as pessoas em risco com base nas informações cadastradas.
        /// </summary>
        private void MenuListarPessoasRisco()
        {
            try
            {
                var pessoasRisco = _gerenciadorPessoas.ListarPessoasEmRisco();

                if (!pessoasRisco.Any())
                {
                    Console.WriteLine("Nenhuma pessoa em situação de risco identificada.");
                    return;
                }

                Console.WriteLine($"=== PESSOAS EM RISCO ({pessoasRisco.Count}) ===");
                foreach (var pessoa in pessoasRisco)
                {
                    var tempoSemContato = DateTime.Now - pessoa.UltimaLocalizacao;
                    Console.WriteLine($"• {pessoa.Nome} (CPF: {pessoa.CPF})");
                    Console.WriteLine($"  Último contato: {pessoa.UltimaLocalizacao:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine($"  Tempo sem contato: {tempoSemContato.TotalHours:F1} horas");
                    if (pessoa.Posicao != null)
                    {
                        Console.WriteLine($"  Última localização: {pessoa.Posicao.Latitude}, {pessoa.Posicao.Longitude}");
                        if (!string.IsNullOrEmpty(pessoa.Posicao.Descricao))
                            Console.WriteLine($"  Local: {pessoa.Posicao.Descricao}");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar pessoas em risco: {ex.Message}");
            }
        }

        /// <summary>
        /// Menu para sincronizar os dados do sistema via Bluetooth.
        /// </summary>
        private void MenuSincronizarDados()
        {
            try
            {
                var dispositivosAtivos = _gerenciadorBluetooth.ObterDispositivosAtivos();
                if (!dispositivosAtivos.Any())
                {
                    Console.WriteLine("Nenhum dispositivo conectado para sincronização.");
                    return;
                }

                var dadosSincronizacao = new
                {
                    pessoas = _gerenciadorPessoas.ObterTodasPessoas().Count,
                    falhas = _gerenciadorFalhas.ObterFalhasAtivas().Count,
                    dispositivos = dispositivosAtivos.Count,
                    timestamp = DateTime.Now,
                    versao = "1.0"
                };

                var dados = JsonSerializer.Serialize(dadosSincronizacao);
                var checksum = _gerenciadorBluetooth.GerarChecksum(dados);

                _gerenciadorBluetooth.SincronizarDados(dados, checksum);

                Console.WriteLine("Dados sincronizados com sucesso!");
                Console.WriteLine($"Dispositivos sincronizados: {dispositivosAtivos.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na sincronização: {ex.Message}");
            }
        }
    }
}
