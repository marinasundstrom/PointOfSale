using System;

using Billing.Application.Common.Interfaces;

namespace Billing.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}