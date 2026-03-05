using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get;  set; } = Guid.NewGuid();
    }
}
