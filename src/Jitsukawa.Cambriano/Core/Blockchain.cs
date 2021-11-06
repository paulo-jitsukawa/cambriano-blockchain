using Jitsukawa.Cambriano.Entity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jitsukawa.Cambriano.Core
{
    /// <summary>
    /// Cadeia de blocos e suas operações básicas.
    /// </summary>
    public class Blockchain
    {
        private readonly Crypto crypto;
        private readonly Mempool mempool;
        private readonly Network network;
        private readonly Settings settings;
        private readonly HttpClient http = new();

        public Blockchain(Crypto crypto, Mempool mempool, Network network, IOptions<Settings> configuration)
        {
            this.crypto = crypto;
            this.mempool = mempool;
            this.network = network;
            settings = configuration.Value;
            Chain.Add(GenesisBlock);
        }

        #region Block

        /// <summary>
        /// Bloco que inicia a cadeia.
        /// </summary>
        private static Block GenesisBlock => new()
        {
            Nonce = 1,
            Node = string.Empty,
            PreviousHash = string.Empty
        };

        /// <summary>
        /// Último bloco adicionado à cadeia.
        /// </summary>
        private Block PreviousBlock => Chain[^1];

        /// <summary>
        /// Adiciona um bloco à cadeia.
        /// </summary>
        /// <param name="content">Conteúdo do bloco.</param>
        /// <param name="nonce">Nonce do bloco.</param>
        /// <param name="previousHash">Hash do último bloco.</param>
        /// <returns>Índice do bloco adicionado.</returns>
        private int CreateBlock(int nonce, string previousHash, Transaction[] transactions)
        {
            var block = new Block
            {
                Index = Chain.Count,
                DateTime = DateTime.Now,
                Node = network.NodeId,
                Nonce = nonce,
                Transactions = transactions,
                PreviousHash = previousHash
            };

            Chain.Add(block);

            return block.Index;
        }

        /// <summary>
        /// Minera um bloco com o conteúdo fornecido e o adiciona à cadeia.
        /// </summary>
        /// <param name="content">Conteúdo do bloco.</param>
        /// <returns>Índice do bloco.</returns>
        public int MineBlock()
        {
            mempool.GarbageCollector();
            var transactions = mempool.Next(Chain);

            if (transactions.Length == settings.BlockSize)
            {
                var previousBlock = PreviousBlock;
                var previousNonce = previousBlock.Nonce;
                var currentNonce = ProofOfWork(previousNonce);
                var previousHash = Hash(previousBlock);

                return CreateBlock(currentNonce, previousHash, transactions);
            }

            return -1;
        }

        /// <summary>
        /// Quantidade de blocos da cadeia que foram minerados pelo nodo.
        /// </summary>
        public int Mined => Chain.Count(c => c.Node.Equals(network.NodeId));

        #endregion
        #region Chain

        /// <summary>
        /// Cadeia de blocos.
        /// </summary>
        public List<Block> Chain { get; private set; } = new();

        /// <summary>
        /// Valida uma cadeia.
        /// </summary>
        /// <param name="chain">Cadeia a ser validada ou nulo para validar a cadeia atual.</param>
        /// <returns>Validade da cadeia.</returns>
        public bool IsChainValid(List<Block>? chain = null)
        {
            chain ??= this.Chain;
            var previousBlock = chain[0];

            using var sha256 = SHA256.Create();
            for (int i = 1; i < chain.Count; i++)
            {
                var currentBlock = chain[i];
                if (currentBlock.PreviousHash != Hash(previousBlock))
                {
                    return false;
                }

                var dificulty = Math.Pow(currentBlock.Nonce, 2) - Math.Pow(previousBlock.Nonce, 2);
                var hash = crypto.ComputeHash(sha256, dificulty.ToString());
                if (!hash.StartsWith("0000"))
                {
                    return false;
                }

                previousBlock = currentBlock;
            }

            return true;
        }

        /// <summary>
        /// Atualiza a cadeia conforme o consenso da rede.
        /// </summary>
        public async Task<bool> ChainSync()
        {
            List<Block> longestChain = Chain;

            var replaced = false;
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            var fails = 0;
            Exception? exception = null;
            foreach (var node in network.Nodes)
            {
                try
                {
                    var result = await http.GetAsync($"{node}blockchain");
                    var response = await result.Content.ReadAsStringAsync();

                    if (HttpStatusCode.OK == result.StatusCode)
                    {
                        if (JsonSerializer.Deserialize(response, typeof(List<Block>), options) is not List<Block> nodeChain)
                        {
                            throw new JsonException("Formato incorreto");
                        }

                        // Prioriza a cadeia que mais avançou
                        if (nodeChain.Count > longestChain.Count && IsChainValid(nodeChain))
                        {
                            replaced = true;
                            longestChain = nodeChain;
                        }
                    }
                }
                catch (Exception e)
                {
                    fails++;
                    exception = e;
                }
            }

            if (network.Nodes.Any() && network.Nodes.Count == fails)
            {
                throw new ApplicationException("Rede inválida.", exception);
            }

            Chain = longestChain;
            return replaced;
        }

        #endregion
        #region Crypto

        /// <summary>
        /// Computa o hash de um bloco.
        /// </summary>
        /// <param name="block">Bloco cujo hash deve ser calculado.</param>
        /// <returns>Hash do bloco.</returns>
        private string Hash(Block block)
        {
            var json = JsonSerializer.Serialize(block);

            using var sha256 = SHA256.Create();
            return crypto.ComputeHash(sha256, json);
        }

        /// <summary>
        /// Descobre o próximo Nonce.
        /// </summary>
        /// <param name="previousNonce">Nonce do último bloco.</param>
        /// <returns>Nonce encontrado.</returns>
        public int ProofOfWork(int previousNonce)
        {
            using var sha256 = SHA256.Create();

            for (int newNonce = 1; newNonce < int.MaxValue; newNonce++)
            {
                var dificulty = Math.Pow(newNonce, 2) - Math.Pow(previousNonce, 2);
                var hash = crypto.ComputeHash(sha256, dificulty.ToString());

                if (hash.StartsWith("0000"))
                {
                    return newNonce;
                }
            }

            return -1;
        }

        #endregion
    }
}
