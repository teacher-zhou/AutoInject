using System;
using System.Collections.Generic;
using System.Text;

namespace AutoInject.Test.Data
{
    public interface IMultiService1
    {
    }

    public interface IMultiService2
    {

    }

    public class MultiService : IMultiService1, IMultiService2,ITransient
    {
    }
}
