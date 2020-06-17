using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Runtime.ConstrainedExecution;

namespace simpleCache
{
    /// <summary>
    /// DownloadCache contains a list of DownloadCacheItems, and handles finding free slots for you
    /// </summary>
    class DownloadCache
    {
        private DownloadCacheItem[] cache;
        private readonly int length;

        /// <summary>
        /// Makes a new DownloadCache
        /// </summary>
        /// <param name="items">Size of document cache</param>
        public DownloadCache(int items)
        {
            /* No one would do this, surely */
            if (items < 0 || items > 1000)
            {
                throw new Exception("Ridiculous thing to do.");
            }            
            this.cache = new DownloadCacheItem[items];
            this.length = items;
        }

        /// <summary>
        /// Checks that parameter is within range and returns item
        /// </summary>
        /// <param name="i">item to return</param>
        /// <returns>String</returns>
        public string getCache(int i)
        {
            if (i >= 0 && i < length)
                return this.cache[i].getData();
            /* What are you trying to pull? */
            return null;
        }

        /// <summary>
        /// Makes sure the item is empty and sets the item
        /// </summary>
        /// <param name="i">address of item</param>
        /// <param name="url">URL to pull and cache</param>
        /// <returns>address of item, or -1 if the item wasn't empty before</returns>
        public int setCache(int i, String url)
        {
            if (this.cache[i] != null || i < 0 || i >= this.length)
                return -1;
            this.cache[i] = new DownloadCacheItem(url);
            return i;
        }

        /// <summary>
        /// Finds the next free space, and if it's full deletes the least frequently accessed cache and returns that.
        /// </summary>
        /// <returns>Address of first empty space</returns>
        public int nextFree()
        {
            int i = 0, j;
            /* 1>>30 is a Very Big Number */
            int lowest_frequency = 1>>30, least_frequent_index = -1;


            while (this.cache[i] != null)
            {
                i++;
                if (i == this.length)
                {
                    for (j = 0; j < this.length; j++)
                    {
                        if (this.cache[j].getAccessed() < lowest_frequency)
                        {
                            lowest_frequency = this.cache[j].getAccessed();
                            least_frequent_index = j;
                        }
                    }
                    /* "Delete" the item */
                    this.cache[least_frequent_index] = null;
                    /* Cache is full, deleted the least frequent! */
                    return least_frequent_index;
                }
            }
            return i;
        }
    }
    class DownloadCacheItem
    {
        private readonly String location;
        private string data;
        private int accessed;

        public DownloadCacheItem(String location)
        {
            this.location = location;
            this.accessed = 0;
        }

        public string getData()
        {
            if (this.data != null)
            {
                this.accessed++;
                /* This is called a cache hit- we have the data already! */
                return this.data;
            }

            /* This is called a cache miss- we don't have the data so need to fetch it */

            using (WebClient client = new WebClient())
            {
                this.data = client.DownloadString(this.location);
            }

            return this.data;
        }

        public int getAccessed()
        {
            return this.accessed;
        }
    }
    class Program
    {
        static void timestamp(string msg)
        {
            Console.WriteLine(DateTime.Now + " " + msg);
        }

        static void Main(string[] args)
        {
            DownloadCache cache = new DownloadCache(2);
            int first, second;

            timestamp("About to make the cache"); // Prints the current timestamp to the console alongside a strings
            first = cache.nextFree(); // Finds the next available space, or returns the least used cache if there is no space available
            cache.setCache(first, "https://people.freebsd.org/~crees/removed_ports/index.xml"); // Sets the cache to a url
            timestamp("Cache made, now let's try to access it and get the first thirty characters..."); // Prints the timestamp to the console so that the time taken to cache the webpage can be recorded
            Console.WriteLine(cache.getCache(first).Substring(0, 30)); // Prints the first 30 characters from the cache to the console
            timestamp("Let's try again..."); // Prints the timestamp agan
            Console.WriteLine(cache.getCache(first).Substring(0, 30)); // Again, accesses the cache
            timestamp("Woah, that was quicker!"); // Then records the timestamp to compare how long it took to load
            second = cache.nextFree(); // Ditto line 140
            cache.setCache(second, "https://google.com"); // Caches another url
            Console.WriteLine(cache.getCache(second).Substring(0, 30));
            Console.WriteLine(cache.getCache(second).Substring(0, 30));
            Console.WriteLine(cache.getCache(second).Substring(0, 30));
            Console.WriteLine(cache.getCache(second).Substring(0, 30));
            /* Find the next free one and see which one has been deleted- which one should be deleted? */
            
        }
    }
}
