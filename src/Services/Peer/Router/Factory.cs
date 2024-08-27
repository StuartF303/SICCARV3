using Siccar.Network.Peers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Router
{
    /// <summary>
    /// A Factory function for use as a ServiceCollection Extension
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Factory<T> : IFactory<T>
    {
        private readonly Func<T> _initFunc;

        public Factory(Func<T> initFunc)
        {
            _initFunc = initFunc;
        }

        public T Create()
        {
            return _initFunc();
        }
    }
}
