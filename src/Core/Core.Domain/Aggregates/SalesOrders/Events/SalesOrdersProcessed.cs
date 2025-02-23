﻿namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrders.Events;

public record SalesOrdersProcessed(List<MedSalesOrder> SuccessfullOrders, List<string> FailedOrders) : IDomainEvent { }
