using GlobalSolution.src.Models;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalSolution.src.Models;

namespace EmergencyMonitoringSystem.Services
{

    /// <summary>
    /// Classe responsável por gerenciar usuários do sistema, incluindo autenticação e controle de acesso.
    /// </summary>
    public class GerenciadorUsuarios
    {
        private List<Usuario> _usuarios;

        /// <summary>
        /// Inicializa o gerenciador de usuários com um usuário administrador padrão.
        /// </summary>
        public GerenciadorUsuarios()
        {
            _usuarios = new List<Usuario>
            {
                new Usuario { Nome = "admin", SenhaHash = GerarHash("admin") }
            };
        }

        /// <summary>
        /// Autentica um usuário com base no nome e senha fornecidos.
        /// </summary>
        /// <param name="nome">Nome do usuário.</param>
        /// <param name="senha">Senha do usuário.</param>
        /// <returns>True se a autenticação for bem-sucedida, caso contrário false.</returns>
        public bool Autenticar(string nome, string senha)
        {
            var hash = GerarHash(senha);
            return _usuarios.Any(u => u.Nome == nome && u.SenhaHash == hash);
        }

        /// <summary>
        /// Adiciona um novo usuário ao sistema.
        /// </summary>
        /// <param name="nome">Nome do novo usuário.</param>
        /// <param name="senha">Senha do novo usuário.</param>
        /// <exception cref="InvalidOperationException">Lançada se o usuário já existir.</exception>
        public void AdicionarUsuario(string nome, string senha)
        {
            if (_usuarios.Any(u => u.Nome == nome))
                throw new InvalidOperationException("Usuário já existe");

            _usuarios.Add(new Usuario { Nome = nome, SenhaHash = GerarHash(senha) });
        }

        /// <summary>
        /// Remove um usuário existente do sistema.
        /// </summary>
        /// <param name="nome">Nome do usuário a ser removido.</param>
        /// <exception cref="InvalidOperationException">Lançada se tentar remover o administrador ou se o usuário não for encontrado.</exception>
        public void RemoverUsuario(string nome)
        {
            if (nome == "admin")
                throw new InvalidOperationException("Não é possível remover o usuário administrador");

            var usuario = _usuarios.FirstOrDefault(u => u.Nome == nome);
            if (usuario != null)
                _usuarios.Remove(usuario);
            else
                throw new InvalidOperationException("Usuário não encontrado");
        }

        /// <summary>
        /// Gera um hash SHA-256 a partir de uma string fornecida.
        /// </summary>
        /// <param name="input">Texto para gerar o hash.</param>
        /// <returns>Hash em formato base64.</returns>
        private string GerarHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
