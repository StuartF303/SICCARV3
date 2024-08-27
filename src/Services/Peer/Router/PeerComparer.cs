using Siccar.Network.Peers.Core;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#nullable enable

namespace Siccar.Network.Router
{
    public class PeerComparer : IEqualityComparer<Peer>
    {
        /// <summary>
        /// Our Peer comparitor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(Peer? x, Peer? y)
        {
            if (x == null && y == null)
                return true;

            if (x?.Id == y?.Id)
                return true;

            if (x?.PeerName.ToLower() == y?.PeerName.ToLower())
                return true;

            if (x?.IPEndPoint != null)
                if (x?.IPEndPoint == y?.IPEndPoint)
                    return true;

            if (x?.URIEndPoint == y?.URIEndPoint)
                return true;

            // so if we got this far then they are a match

            return false;
        }

        /// <summary>
        /// Required by the Interface
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode([DisallowNull] Peer obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
