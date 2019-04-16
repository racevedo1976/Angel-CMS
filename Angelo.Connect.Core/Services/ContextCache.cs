using Angelo.Connect.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class ContextCache<TContext> where TContext : class
    {
        private class ContextItem
        {
            public DateTime ExpUTC { get; set; }
            public TContext Context { get; set; }
        }
            
        private ConcurrentDictionary<string, ContextItem> _contextDictionary;

        public ContextCache()
        {
            _contextDictionary = new ConcurrentDictionary<string, ContextItem>();
        }

        public bool RemoveById(string id)
        {
            ContextItem item;
            return _contextDictionary.TryRemove(id, out item);
        }

        public void Save(string id, TContext context, int seconds = 1800)
        {
            Ensure.NotNullOrEmpty(id);

            var item = new ContextItem()
            {
                ExpUTC = DateTime.UtcNow.AddSeconds(seconds),
                Context = context
            };
            _contextDictionary.AddOrUpdate(id, item, (k, v) => item);
        }

        public TContext GetById(string id)
        {
            ContextItem item;
            if (string.IsNullOrEmpty(id))
                return null;
            else if (_contextDictionary.TryGetValue(id, out item) == false)
                return null;
            else if (item.ExpUTC < DateTime.UtcNow)
                return null;
            else
                return item.Context;
        }

        public void RemoveExpiredItems()
        {
            var currentUtc = DateTime.UtcNow;
            var ids = _contextDictionary
                .Where(x => x.Value.ExpUTC < currentUtc)
                .Select(x => x.Key)
                .ToList();
            foreach (var id in ids)
                RemoveById(id);
        }

    }
}
