using System.Security.Cryptography;
using System.Text;

namespace Jitsukawa.Cambriano.Core
{
    public class Crypto
    {
        /// <summary>
        /// Calcula o hash em hexadecimal.
        /// </summary>
        /// <param name="hashAlgorithm">Algoritmo de cálculo.</param>
        /// <param name="content">Conteúdo para o cálculo.</param>
        /// <returns>Hash em hexadecimal.</returns>
        public string ComputeHash(HashAlgorithm hashAlgorithm, string content)
        {
            var sb = new StringBuilder();

            // Gera o hash do conteúdo
            var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(content));

            // Converte cada byte do hash em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
