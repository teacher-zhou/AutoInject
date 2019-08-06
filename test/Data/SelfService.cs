using System;
using System.Collections.Generic;
using System.Text;

namespace AutoInject.Test.Data
{
    /*******************
     * The service for self injection.
     * *****************/

    class SelfServiceForTransient:ITransient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    class SelfServiceForScoped : IScoped
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
    class SelfServiceForSingleton : ISingleton
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
