using System;
using System.Reflection;
using System.Threading.Tasks;

using MassTransit;

using Warehouse.Application.Common.Interfaces;
using Warehouse.Contracts;
using Warehouse.Domain.Common;

namespace Warehouse.Infrastructure.Services
{
    class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}