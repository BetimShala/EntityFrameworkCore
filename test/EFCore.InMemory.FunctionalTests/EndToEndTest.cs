// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Microsoft.EntityFrameworkCore
{
    public class EndToEndInMemoryTest : IClassFixture<InMemoryFixture>
    {
        public EndToEndInMemoryTest(InMemoryFixture fixture) => Fixture = fixture;

        protected InMemoryFixture Fixture { get; }

        [Fact]
        public void Can_use_different_entity_types_end_to_end()
        {
            Can_add_update_delete_end_to_end<Private>();
            Can_add_update_delete_end_to_end<object>();
            Can_add_update_delete_end_to_end<List<Private>>();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Private
        {
        }

        private void Can_add_update_delete_end_to_end<T>()
            where T : class, new()
        {
            var type = typeof(T);
            var modelBuilder = new ModelBuilder(InMemoryConventionSetBuilder.Build());
            modelBuilder.Entity<T>(eb =>
            {
                eb.Property<int>("Id");
                eb.Property<string>("Name");
            });

            var optionsBuilder = new DbContextOptionsBuilder()
                .UseModel(modelBuilder.Model)
                .UseInMemoryDatabase(nameof(EndToEndInMemoryTest))
                .UseInternalServiceProvider(Fixture.ServiceProvider);

            T entity;
            using (var context = new DbContext(optionsBuilder.Options))
            {
                var entry = context.Entry(new T());
                entity = entry.Entity;

                entry.Property("Id").CurrentValue = 42;
                entry.Property("Name").CurrentValue = "The";

                entry.State = EntityState.Added;

                context.SaveChanges();
            }

            using (var context = new DbContext(optionsBuilder.Options))
            {
                var entityFromStore = context.Set<T>().Single();
                var entityEntry = context.Entry(entityFromStore);

                Assert.NotSame(entity, entityFromStore);
                Assert.Equal(42, entityEntry.Property("Id").CurrentValue);
                Assert.Equal("The", entityEntry.Property("Name").CurrentValue);

                entityEntry.Property("Name").CurrentValue = "A";

                context.Update(entityFromStore);

                context.SaveChanges();
            }

            using (var context = new DbContext(optionsBuilder.Options))
            {
                var entityFromStore = context.Set<T>().Single();
                var entry = context.Entry(entityFromStore);

                Assert.Equal("A", entry.Property("Name").CurrentValue);

                context.Remove(entityFromStore);

                context.SaveChanges();
            }

            using (var context = new DbContext(optionsBuilder.Options))
            {
                Assert.Equal(0, context.Set<T>().Count());
            }
        }
    }
}
