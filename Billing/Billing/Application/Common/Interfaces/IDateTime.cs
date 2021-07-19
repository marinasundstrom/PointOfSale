using System;

namespace Billing.Application.Common.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }
}