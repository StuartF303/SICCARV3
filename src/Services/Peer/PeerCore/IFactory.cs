using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Core
{
    public interface IFactory<T>
    {

        public T Create();

    }
}
