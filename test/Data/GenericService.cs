using System;
using System.Collections.Generic;
using System.Text;

namespace AutoInject.Test.Data
{
    interface IGenericType<T>:IScoped
    {
    }

    public class GenericType<T> : IGenericType<T>
    {

    }

    interface IGenericType2<T1, T2> : ITransient { }

    public class GenericTyp2<T1, T2> : IGenericType2<T1, T2>
    {
    }
}
