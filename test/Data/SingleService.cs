using System;
using System.Collections.Generic;
using System.Text;

namespace AutoInject.Test.Data
{
    public interface ISingleServiceWithTransient : ITransient
    {
        Guid Id { get; set; }
    }

    public interface ISingleServiceWithScoped : IScoped
    {
        Guid Id { get; set; }
    }

    public interface ISingleServiceWithSingleton : ISingleton
    {
        Guid Id { get; set; }
    }

    public class SingleServiceWithTransient : ISingleServiceWithTransient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class SingleServiceWithScoped : ISingleServiceWithScoped
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class SingleServiceWithSingleton : ISingleServiceWithSingleton
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
