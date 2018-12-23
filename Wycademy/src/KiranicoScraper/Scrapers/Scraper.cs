using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Scrapers
{
    abstract class Scraper
    {
        public DbManager Database { get; set; }

        public WebRequester Requester { get; set; }

        public abstract void Execute();
    }
}
