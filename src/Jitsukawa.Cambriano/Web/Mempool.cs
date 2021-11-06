using Jitsukawa.Cambriano.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MempoolCore = Jitsukawa.Cambriano.Core.Mempool;

namespace Jitsukawa.Cambriano.Web
{
    [ApiController]
    [Route("[controller]")]
    public class Mempool : ControllerBase
    {
        private readonly MempoolCore mempool;
        private readonly ILogger logger;

        public Mempool(MempoolCore mempool, ILogger<MempoolCore> logger)
        {
            this.mempool = mempool;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna as transações não confirmadas.
        /// </summary>
        [HttpGet]
        public IActionResult GetMempool()
        {
            var transactions = mempool.Transactions;

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Mempool consultado.");

            return Ok(transactions);
        }

        /// <summary>
        /// Adiciona uma transação ao Mempool. A propriedade "Id" não deve ser preenchida manualmente.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            var added = transaction.Id == null;

            await mempool.Add(transaction);

            logger.LogInformation(added
                ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Transação {transaction.Id} adicionada ao Mempool."
                : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Transação {transaction.Id} propagada para o Mempool.");

            return Ok();
        }

        /// <summary>
        /// Descarta as transações expiradas - confirmadas ou não.
        /// </summary>
        [HttpDelete]
        public IActionResult GarbageCollector()
        {
            mempool.GarbageCollector();

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Descartadas do Mempool as transações expiradas.");

            return Ok();
        }
    }
}
