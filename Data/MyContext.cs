using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using System;
using Microsoft.Azure.Cosmos;
using System.Net;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


// Create a DbContext for the users collection
public class MyContext : IdentityDbContext<IdentityUser>
{
    public MyContext(DbContextOptions<MyContext> options)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>(model =>
        {
            model.HasPartitionKey(nameof(IdentityRole.Id));
            model.Property(p => p.ConcurrencyStamp).IsETagConcurrency();
        });


        builder.Entity<IdentityUser>(model =>
        {
            model.HasPartitionKey(nameof(IdentityUser.Id));
            model.Property(p => p.ConcurrencyStamp).IsETagConcurrency();
        });
    }
}
