using System;
using System.Collections.Generic;
using System.Text;

namespace AutoInject.Test.Data
{
    public class TransientScopeSingletonService:ITransient,IScoped,ISingleton
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class ScopeTransient : IScoped, ITransient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class ScopeSingleton : IScoped, ISingleton
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }


    public class SingletonTransient : IScoped, ISingleton
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
