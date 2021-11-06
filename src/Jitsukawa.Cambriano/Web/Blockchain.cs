using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BlockchainCore = Jitsukawa.Cambriano.Core.Blockchain;

namespace Jitsukawa.Cambriano.Web
{
    [ApiController]
    [Route("[controller]")]
    public class Blockchain : ControllerBase
    {
        private readonly BlockchainCore blockchain;
        private readonly ILogger logger;

        public Blockchain(BlockchainCore blockchain, ILogger<BlockchainCore> logger)
        {
            this.blockchain = blockchain;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna a cadeia de blocos.
        /// </summary>
        [HttpGet]
        public IActionResult GetChain()
        {
            var chain = blockchain.Chain;

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Blockchain consultada.");

            return Ok(chain);
        }

        /// <summary>
        /// Informa se a cadeia de blocos é válida.
        /// </summary>
        [HttpGet("Valid")]
        public IActionResult IsChainValid()
        {
            var result = blockchain.IsChainValid(blockchain.Chain);

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Integridade da blockchain verificada.");

            return Ok(result);
        }

        /// <summary>
        /// Atualiza a cadeia de blocos conforme o consenso da rede.
        /// </summary>
        [HttpPost("Sync")]
        public async Task<IActionResult> ChainSync()
        {
            var replaced = await blockchain.ChainSync();

            logger.LogInformation(replaced
                ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Blockchain sincronizada: foi necessário atualizar os blocos."
                : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Blockchain sincronizada: não foi necessário aplicar mudanças.");

            return Ok();
        }

        /// <summary>
        /// Minera o próximo bloco e o adiciona à cadeia.
        /// </summary>
        [HttpPost("Mine")]
        public IActionResult MineBlock()
        {
            var index = blockchain.MineBlock();

            logger.LogInformation(index > 0
                ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Minerado bloco de índice {index}."
                : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Transações insuficientes para minerar o bloco.");

            return Ok(index);
        }

        /// <summary>
        /// Informa quantos blocos da cadeia foram minerados pelo nodo.
        /// </summary>
        [HttpGet("Mined")]
        public IActionResult MinedBlocks()
        {
            var mined = blockchain.Mined;

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Quantidade de blocos minerados consultada.");

            return Ok(mined);
        }
    }
}
