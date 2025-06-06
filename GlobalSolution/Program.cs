using EmergencyMonitoringSystem.Services;
using GlobalSolution.src;
using GlobalSolution.src.Services;

namespace EmergencyMonitoringSystem
{
    /// <summary>
    /// Classe principal que executa o programa.
    /// Contém o fluxo de autenticação e inicialização do sistema de monitoramento.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Método principal da aplicação. Gerencia login, cadastro e remoção de usuários.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando (não utilizados).</param>
        static void Main(string[] args)
        {
            var gerenciadorUsuarios = new GerenciadorUsuarios();

            while (true)
            {
                Console.WriteLine("=== SISTEMA DE MONITORAMENTO ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Cadastrar novo usuário");
                Console.WriteLine("3. Remover usuário");
                Console.WriteLine("4. Sair");
                Console.Write("Escolha uma opção: ");
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        FazerLogin(gerenciadorUsuarios);
                        break;
                    case "2":
                        CadastrarUsuario(gerenciadorUsuarios);
                        break;
                    case "3":
                        RemoverUsuario(gerenciadorUsuarios);
                        break;
                    case "4":
                        Console.WriteLine("Encerrando o programa...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida.\n");
                        break;
                }
            }
        }

        /// <summary>
        /// Executa o processo de login do usuário com tentativa limitada.
        /// </summary>
        static void FazerLogin(GerenciadorUsuarios gerenciador)
        {
            Console.WriteLine("\n=== LOGIN ===");
            int tentativas = 3;

            while (tentativas > 0)
            {
                Console.Write("Usuário: ");
                var usuario = Console.ReadLine();

                Console.Write("Senha: ");
                var senha = LerSenhaOculta();

                if (gerenciador.Autenticar(usuario, senha))
                {
                    Console.WriteLine("\nLogin bem-sucedido!\n");
                    var sistema = new SistemaMonitoramentoEmergencia();
                    sistema.IniciarSistema();
                    sistema.ExibirMenu();
                    return;
                }
                else
                {
                    tentativas--;
                    Console.WriteLine($"Usuário ou senha inválidos. Tentativas restantes: {tentativas}\n");
                }
            }

            Console.WriteLine("Número de tentativas excedido.\n");
        }

        /// <summary>
        /// Permite o cadastro de um novo usuário no sistema.
        /// </summary>
        static void CadastrarUsuario(GerenciadorUsuarios gerenciador)
        {
            Console.WriteLine("\n=== CADASTRAR NOVO USUÁRIO ===");
            Console.Write("Novo usuário: ");
            var nome = Console.ReadLine();

            Console.Write("Senha: ");
            var senha = LerSenhaOculta();

            try
            {
                gerenciador.AdicionarUsuario(nome, senha);
                Console.WriteLine("Usuário cadastrado com sucesso!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Permite a exclusão de um usuário existente.
        /// </summary>
        static void RemoverUsuario(GerenciadorUsuarios gerenciador)
        {
            Console.WriteLine("\n=== REMOVER USUÁRIO ===");
            Console.Write("Nome do usuário a remover: ");
            var nome = Console.ReadLine();

            try
            {
                gerenciador.RemoverUsuario(nome);
                Console.WriteLine("Usuário removido com sucesso!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Lê a senha digitada no console ocultando os caracteres.
        /// </summary>
        /// <returns>Senha digitada pelo usuário</returns>
        static string LerSenhaOculta()
        {
            var senha = string.Empty;
            ConsoleKey tecla;
            do
            {
                var teclaInfo = Console.ReadKey(intercept: true);
                tecla = teclaInfo.Key;

                if (tecla == ConsoleKey.Backspace && senha.Length > 0)
                {
                    senha = senha[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(teclaInfo.KeyChar))
                {
                    senha += teclaInfo.KeyChar;
                    Console.Write("*");
                }

            } while (tecla != ConsoleKey.Enter);

            Console.WriteLine();
            return senha;
        }
    }
}
