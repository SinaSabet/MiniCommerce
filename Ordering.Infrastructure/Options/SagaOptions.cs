using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Options
{
    public class SagaOptions
    {
        public int PaymentTimeoutMinutes { get; set; } = 5;

    }
}
