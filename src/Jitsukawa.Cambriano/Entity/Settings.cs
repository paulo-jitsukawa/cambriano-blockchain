namespace Jitsukawa.Cambriano.Entity
{
    /// <summary>
    /// Configuração do nodo.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Quantidade de transações por bloco.
        /// </summary>
        public int BlockSize { get; set; } = 5;

        /// <summary>
        /// Tempo de vida de uma transação válida.
        /// </summary>
        public int TransactionLifeTime { get; set; } = 5;
    }
}
