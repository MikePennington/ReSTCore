using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReSTCore.Test.Fixtures
{
    /// <summary>
    /// Interface for builder contract
    /// </summary>
    public interface IBuilder<T>
    {
        T Build();
    }
}
