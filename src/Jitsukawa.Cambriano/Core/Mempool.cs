using Jitsukawa.Cambriano.Entity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jitsukawa.Cambriano.Core
{
    /// <summary>
    /// Transações não confirmadas.
    /// </summary>
    public class Mempool
    {
        private readonly Network network;
        private readonly Settings settings;
        private readonly HttpClient http = new();

        public List<Transaction> Transactions { get; set; } = new();

        public Mempool(Network network, IOptions<Settings> configuration)
        {
            this.network = network;
            settings = configuration.Value;
        }

        /// <summary>
        /// Adiciona uma transação ao Mempool.
        /// </summary>
        public async Task<bool> Add(Transaction transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction.Id))
            {
                transaction.Id = Guid.NewGuid().ToString();
                transaction.DateTime = DateTime.Now;

                await Propagate(transaction);
            }
            else if (Transactions.Any(t => t.Id == transaction.Id))
            {
                return false;
            }

            Transactions.Add(transaction);

            return true;
        }

        /// <summary>
        /// Próximas candidatas do Mempool a integrarem o próximo bloco da cadeia.
        /// </summary>
        public Transaction[] Next(List<Block> chain)
        {
            return Transactions
                .Where(t => !chain.Any(c => c.Transactions.Any(ct => ct.Id == t.Id)))
                .Take(settings.BlockSize)
                .ToArray();
        }

        /// <summary>
        /// Remove as transações que expiraram.
        /// </summary>
        public void GarbageCollector()
        {
            Transactions = Transactions
                .Where(t => t.DateTime.AddMinutes(settings.TransactionLifeTime) > DateTime.Now)
                .ToList();
        }

        /// <summary>
        /// Propaga uma transação na rede.
        /// </summary>
        public async Task Propagate(Transaction transaction)
        {
            var json = JsonSerializer.Serialize(transaction);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var fail = false;
            foreach (var node in network.Nodes)
            {
                try
                {
                    await http.PostAsync($"{node}mempool", content);
                }
                catch
                {
                    fail = true;
                }
            }

            if (fail)
            {
                throw new ApplicationException("Não foi possível propagar a transação para todos os nodos registrados.");
            }
        }
    }
}
