using Microsoft.EntityFrameworkCore;
using TbspRpgDataLayer.Entities.LanguageSources;
using TbspRpgDataLayer.Entities;

namespace TbspRpgDataLayer
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        #region Adventure

        public DbSet<Adventure> Adventures { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<En> SourcesEn { get; set; }
        public DbSet<Esp> SourcesEsp { get; set; }
        public DbSet<Script> Scripts { get; set; }
        public DbSet<AdventureObject> AdventureObjects { get; set; }
        public DbSet<AdventureObjectGameState> AdventureObjectGameStates { get; set; }

        #endregion

        #region Game

        public DbSet<Game> Games { get; set; }
        public DbSet<Content> Contents { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region Adventure

            modelBuilder.Entity<Adventure>().HasKey(a => a.Id);
            modelBuilder.Entity<Adventure>().Property(a => a.Id).IsRequired();

            modelBuilder.Entity<Location>().HasKey(a => a.Id);
            modelBuilder.Entity<Location>().Property(a => a.Id).IsRequired();
            
            modelBuilder.Entity<Route>().HasKey(a => a.Id);
            modelBuilder.Entity<Route>().Property(a => a.Id).IsRequired();
            
            modelBuilder.Entity<AdventureObject>().HasKey(a => a.Id);
            modelBuilder.Entity<AdventureObject>().Property(a => a.Id).IsRequired();
            
            modelBuilder.Entity<AdventureObjectGameState>().HasKey(a => a.Id);
            modelBuilder.Entity<AdventureObjectGameState>().Property(a => a.Id).IsRequired();
            
            modelBuilder.Entity<Script>().HasKey(s => s.Id);
            modelBuilder.Entity<Script>().Property(s => s.Id).IsRequired();
            modelBuilder.Entity<Script>().HasIndex(s => s.Content);

            modelBuilder.Entity<Adventure>()
                .HasMany(a => a.Locations)
                .WithOne(l => l.Adventure)
                .HasForeignKey(l => l.AdventureId);

            modelBuilder.Entity<Adventure>()
                .HasMany(a => a.Scripts)
                .WithOne(s => s.Adventure)
                .HasForeignKey(s => s.AdventureId);

            modelBuilder.Entity<Script>()
                .HasMany(s => s.AdventureInitializations)
                .WithOne(a => a.InitializationScript)
                .HasForeignKey(a => a.InitializationScriptId);

            modelBuilder.Entity<Script>()
                .HasMany(s => s.AdventureTerminations)
                .WithOne(a => a.TerminationScript)
                .HasForeignKey(a => a.TerminationScriptId);

            modelBuilder.Entity<Location>()
                .HasMany(l => l.Routes)
                .WithOne(r => r.Location)
                .HasForeignKey(r => r.LocationId);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.DestinationLocation)
                .WithMany()
                .HasForeignKey(r => r.DestinationLocationId);

                //language sources
            modelBuilder.Entity<En>().HasKey(e => e.Id);
            modelBuilder.Entity<En>().Property(e => e.Id).IsRequired();
            
            modelBuilder.Entity<Esp>().HasKey(e => e.Id);
            modelBuilder.Entity<Esp>().Property(e => e.Id).IsRequired();

            modelBuilder.Entity<En>().Ignore(en => en.Language);
            modelBuilder.Entity<Esp>().Ignore(esp => esp.Language);

            #endregion

            #region Game

            modelBuilder.Entity<Game>().HasKey(a => a.Id);
            modelBuilder.Entity<Game>().Property(a => a.Id).IsRequired();
            
            modelBuilder.Entity<Content>().HasKey(a => a.Id);
            modelBuilder.Entity<Content>().Property(a => a.Id).IsRequired();
            
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Contents)
                .WithOne(c => c.Game)
                .HasForeignKey(c => c.GameId);

            modelBuilder.Entity<Game>().Ignore(game => game.GameStateJson);

            #endregion

            #region Relations

            modelBuilder.Entity<Script>()
                .HasMany(s => s.Includes)
                .WithMany(s => s.IncludedIn);
            
            modelBuilder.Entity<Adventure>()
                .HasMany(a => a.Games)
                .WithOne(g => g.Adventure)
                .HasForeignKey(g => g.AdventureId);
            
            modelBuilder.Entity<Location>()
                .HasMany(l => l.Games)
                .WithOne(g => g.Location)
                .HasForeignKey(g => g.LocationId);

            #endregion
        }
    }
}