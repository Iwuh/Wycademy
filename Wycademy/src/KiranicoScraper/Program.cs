using KiranicoScraper.Scrapers;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KiranicoScraper
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var scrapers = new List<Scraper>()
            {
                new MonsterScraper4U(),
                new MonsterScraperGen(),
                new MonsterScraperWorld(),
                new WeaponScraper4U(),
                new WeaponScraperGen()
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var database = new DbManager())
            using (var requester = new WebRequester())
            {
                foreach (var scraper in scrapers)
                {
                    _logger.Info($"Starting scraper {scraper.GetType()}.");

                    NpgsqlTransaction transaction = null;
                    try
                    {
                        scraper.Database = database;
                        scraper.Requester = requester;

                        transaction = database.BeginTransaction();
                        scraper.Execute();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"An exception was thrown during execution of scraper {scraper.GetType()}.");
                        if (transaction != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Finished in {stopwatch.Elapsed.Minutes}m{stopwatch.Elapsed.Seconds}s, press enter to exit");
            Console.ReadLine();
        }
    }
}
