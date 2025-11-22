using TbspRpgDataLayer;
using TbspRpgDataLayer.Entities.LanguageSources;

namespace TbspRpgDatabaseSetup
{
    class Program
    {
        private static void SeedSource()
        {
            var contextFactory = new DatabaseContextFactory();
            using var context = contextFactory.CreateDbContext(Array.Empty<string>());
            // check if we have empty source for spanish and english
            var dbEnglishEmptySource = context.SourcesEn.FirstOrDefault(source => source.Key == Guid.Empty);
            // if we don't create them
            if (dbEnglishEmptySource == null)
            {
                var englishEmptySource = new En()
                {
                    Key = Guid.Empty,
                    Text = "Empty Source",
                    AdventureId = null
                };
                context.SourcesEn.Add(englishEmptySource);
            }

            var dbSpanishEmptySource = context.SourcesEsp.FirstOrDefault(source => source.Key == Guid.Empty);
            if (dbSpanishEmptySource == null)
            {
                var spanishEmptySource = new Esp()
                {
                    Key = Guid.Empty,
                    Text = "Fuente Vacia",
                    AdventureId = null
                };
                context.SourcesEsp.Add(spanishEmptySource);
            }

            context.SaveChanges();
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Database Setup Start");
            Console.WriteLine("Creating Empty Source");
            SeedSource();
            Console.WriteLine("Database Setup Complete");
        }
    }
}