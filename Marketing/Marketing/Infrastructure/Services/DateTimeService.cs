using System;

using Marketing.Application.Common.Interfaces;

namespace Marketing.Infrastructure.Services;

class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}