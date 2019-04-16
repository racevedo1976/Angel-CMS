using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Angelo.Connect.Models
{
    public class ClientProductApp
    {
        public ClientProductApp()
        {
            Sites = new List<Site>();
            Product = new Product();
            Client = new Client();
            AddOns = new List<ClientProductAddOn>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string ClientId { get; set; }
        public string ProductId { get; set; }
        public ProductSubscriptionType SubscriptionType { get; set; }
        public DateTime SubscriptionStartUTC { get; set; }
        public DateTime? SubscriptionEndUTC { get; set; }
        public int MaxSiteCount { get; set; }

        public Product Product { get; set; }
        public Client Client { get; set; }

        public bool IsActive {
            get
            {
                var now = DateTime.UtcNow;
                return ((SubscriptionStartUTC < now) && 
                        ((SubscriptionEndUTC == null) || (SubscriptionEndUTC > now)));
            }
        }

        public ICollection<Site> Sites { get; set; }
        public ICollection<ClientProductAddOn> AddOns { get; set; }
    }
}