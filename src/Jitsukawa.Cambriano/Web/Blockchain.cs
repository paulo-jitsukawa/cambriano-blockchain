using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        /// Cria e adiciona blocos à cadeia.
        /// </summary>
        [HttpPost("Create")]
        public IActionResult CreateBlocks([FromBody] string[] contents)
        {
            var result = "Criados os blocos: ";

            foreach (var content in contents)
            {
                var index = blockchain.CreateBlock(content);

                logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Adicionado o bloco de índice {index}.");

                result += $"{index}, ";
            }

            result = $"{result.TrimEnd(new char[] { ' ', ',' })}.";

            return Ok(result);
        }
    }
}
