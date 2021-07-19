using System;

using Checkout.Application.Common.Interfaces;

namespace Checkout.Infrastructure.Services
{
    class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}