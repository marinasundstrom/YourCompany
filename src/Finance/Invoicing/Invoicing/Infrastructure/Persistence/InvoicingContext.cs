﻿using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Entities;
using YourBrand.Invoicing.Infrastructure.Persistence.Interceptors;
using YourBrand.Invoicing.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Infrastructure.Persistence;

public class InvoicingContext(
    DbContextOptions<InvoicingContext> options,
    ITenantContext tenantContext) : DbContext(options), IInvoicingContext
{
    public TenantId? TenantId => tenantContext.TenantId;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoicingContext).Assembly);

        modelBuilder.ConfigureDomainModel(configurator =>
        {
            configurator.AddTenancyFilter(() => TenantId);
            configurator.AddSoftDeleteFilter();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

    public DbSet<Invoice> Invoices { get; set; } = null!;

    public DbSet<InvoiceStatus> InvoiceStatuses { get; set; } = null!;

    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<IHasDomainEvents>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        if (!entities.Any())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var domainEvents = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = domainEvent.Id,
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}