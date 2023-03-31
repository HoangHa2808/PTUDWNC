using TatBlog.Core.Entities;

namespace TatBlog.WebApi.Models.Subscribers
{
    public class SubscriberEditModel
    {
        public string Resons { get; set; }

        public string Notes { get; set; }

        public bool IsBlock { get; set; }
    }
}