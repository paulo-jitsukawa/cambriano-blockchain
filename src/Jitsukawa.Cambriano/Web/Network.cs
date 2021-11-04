using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Web;
using NetworkCore = Jitsukawa.Cambriano.Core.Network;

namespace Jitsukawa.Cambriano.Web
{
    [ApiController]
    [Route("[controller]")]
    public class Network : ControllerBase
    {
        private readonly NetworkCore network;
        private readonly ILogger logger;

        public Network(NetworkCore network, ILogger<NetworkCore> logger)
        {
            this.network = network;
            this.logger = logger;
        }

        /// <summary>
        /// Retorna o identificador do nodo.
        /// </summary>
        [HttpGet("Id")]
        public IActionResult GetNodeId()
        {
            var id = network.NodeId;

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Nodo consultado.");

            return Ok(id);
        }

        /// <summary>
        /// Retorna a lista dos nodos conectados.
        /// </summary>
        [HttpGet]
        public IActionResult GetNodes()
        {
            var nodes = network.Nodes;

            logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Lista de nodos consultada.");

            return Ok(nodes);
        }

        /// <summary>
        /// Conecta o nodo à rede formada pelos nodos indicados.
        /// </summary>
        /// <param name="nodes">Nodos da rede.</param>
        [HttpPost]
        public IActionResult AddNodes([FromBody] List<Uri> nodes)
        {
            try
            {
                network.AddNodes(nodes);

                var urls = string.Empty;
                nodes.ForEach(n => urls = $"{urls} {n.AbsoluteUri}");
                logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Nodos adicionados:{urls}");
            }
            catch
            {
                return BadRequest("Endereço(s) de nodo(s) inválido(s).");
            }

            return Ok();
        }

        /// <summary>
        /// Exclui conexão do nodo com outro nodo da rede.
        /// </summary>
        /// <param name="node">Nodo que deve ser desconectado.</param>
        [HttpDelete("{node}")]
        public IActionResult DeleteNode(string node)
        {
            try
            {
                var url = HttpUtility.UrlDecode(node);
                var uri = new Uri(url);
                network.DeleteNode(uri);

                logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Nodo excluído: {url}");
            }
            catch
            {
                return BadRequest("Endereço de nodo inválido.");
            }

            return Ok();
        }
    }
}
