using System;

using Carts.Application.Common.Interfaces;

namespace Carts.Infrastructure.Services
{
    class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}