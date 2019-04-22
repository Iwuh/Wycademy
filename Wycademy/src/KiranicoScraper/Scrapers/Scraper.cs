using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Scrapers
{
    abstract class Scraper
    {
        public WebRequester Requester { get; set; }

        public abstract void Execute();
    }
}
