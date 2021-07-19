using System;
using System.ComponentModel.DataAnnotations.Schema;

using Payments.Domain.Common;

namespace Payments.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}