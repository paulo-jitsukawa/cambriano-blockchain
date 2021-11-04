using System;
using System.Collections.Generic;

namespace Jitsukawa.Cambriano.Core
{
    /// <summary>
    /// Conexão com outros nodos da rede.
    /// </summary>
    public class Network
    {
        /// <summary>
        /// Identificador do nodo.
        /// </summary>
        public string NodeId { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Nodos conectados.
        /// </summary>
        public HashSet<string> Nodes { get; private set; } = new();

        /// <summary>
        /// Conecta nodos.
        /// </summary>
        /// <param name="uris">Endereços dos nodos.</param>
        public void AddNodes(List<Uri> uris) => uris.ForEach(u => Nodes.Add(u.AbsoluteUri));

        /// <summary>
        /// Desconecta um nodo.
        /// </summary>
        /// <param name="uri">Endereço do nodo.</param>
        public void DeleteNode(Uri uri) => Nodes.RemoveWhere(n => n.Equals(uri.AbsoluteUri));
    }
}
