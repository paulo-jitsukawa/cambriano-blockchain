using Jitsukawa.Cambriano.Entity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;

namespace Jitsukawa.Cambriano.Core
{
    /// <summary>
    /// Cadeia de blocos e suas operações básicas.
    /// </summary>
    public class Blockchain
    {
        private readonly Crypto crypto;

        public Blockchain(Crypto crypto)
        {
            this.crypto = crypto;
            Chain.Add(GenesisBlock);
        }

        #region Block

        /// <summary>
        /// Bloco que inicia a cadeia.
        /// </summary>
        private static Block GenesisBlock => new()
        {
            Nonce = 1,
            Content = "Genesis Block",
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
        private int CreateBlock(string content, int nonce, string previousHash)
        {
            var block = new Block
            {
                Index = Chain.Count,
                DateTime = DateTime.Now,
                Nonce = nonce,
                Content = content,
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
        public int MineBlock(string content)
        {
            var previousBlock = PreviousBlock;
            var previousNonce = previousBlock.Nonce;
            var currentNonce = ProofOfWork(previousNonce);
            var previousHash = Hash(previousBlock);

            return CreateBlock(content, currentNonce, previousHash);
        }

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
