﻿using System;

namespace Jitsukawa.Cambriano.Entity
{
    /// <summary>
    /// Bloco da cadeia.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Índice do bloco.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Data e horário em que o bloco foi criado.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Identificador do nodo que minerou o bloco.
        /// </summary>
        public string Node { get; set; } = null!;

        /// <summary>
        /// Prova de trabalho.
        /// </summary>
        public int Nonce { get; set; }

        /// <summary>
        /// Conteúdo do bloco.
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// Hash do bloco anterior.
        /// </summary>
        public string PreviousHash { get; set; } = null!;
    }
}
