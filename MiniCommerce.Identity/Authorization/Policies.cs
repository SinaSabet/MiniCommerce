using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommerce.Identity.Authorization
{
    public static class Policies
    {
        public const string Customer = "customer";
        public const string Admin = "admin";
        public const string WarehouseManager = "warehouse-manager";
        public const string PaymentOperator = "payment-operator";
    }
}
