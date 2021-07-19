using System;

namespace Payments.Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}