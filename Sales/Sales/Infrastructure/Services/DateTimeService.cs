using System;

using Sales.Application.Common.Interfaces;

namespace Sales.Infrastructure.Services
{
    class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}