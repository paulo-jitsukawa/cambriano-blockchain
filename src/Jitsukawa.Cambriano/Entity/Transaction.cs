using System;
using System.ComponentModel.DataAnnotations;

namespace Jitsukawa.Cambriano.Entity
{
    /// <summary>
    /// Registro de transferência de valor monetário.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Identificador da transação.
        /// </summary>
        [RegularExpression("^[a-z0-9]{8}\\-[a-z0-9]{4}\\-[a-z0-9]{4}\\-[a-z0-9]{4}\\-[a-z0-9]{12}$", ErrorMessage = "Não preencha o campo Id manualmente.")]
        public string? Id { get; set; }

        /// <summary>
        /// Data e horário do registro da transação no Mempool.
        /// </summary>
        [Required(ErrorMessage = "Transação sem data e horário.")]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Remetente da transferência.
        /// </summary>
        [Required(ErrorMessage = "Transferência sem remetente.")]
        public string Sender { get; set; } = null!;

        /// <summary>
        /// Destinatário da transferência.
        /// </summary>
        [Required(ErrorMessage = "Transferência sem destinatário.")]
        public string Receiver { get; set; } = null!;

        /// <summary>
        /// Valor transferido.
        /// </summary>
        [Required(ErrorMessage = "Transferência sem valor.")]
        [Range(0.000000001, double.MaxValue, ErrorMessage = "Valor transferido inválido.")]
        public decimal Amount { get; set; }
    }
}
