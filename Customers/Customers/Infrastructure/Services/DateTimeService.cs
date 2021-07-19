using System;

using Customers.Application.Common.Interfaces;

namespace Customers.Infrastructure.Services
{
    class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}