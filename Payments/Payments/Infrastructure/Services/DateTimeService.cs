using System;

using Payments.Application.Common.Interfaces;

namespace Payments.Infrastructure.Services
{
    class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}