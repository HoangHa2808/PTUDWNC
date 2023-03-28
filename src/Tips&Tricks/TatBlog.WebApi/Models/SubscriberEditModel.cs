using TatBlog.Core.Entities;

namespace TatBlog.WebApi.Models
{
    public class SubscriberEditModel
    {
        public string Email { get; set; }
        public DateTime SubscribedDate { get; set; }
        public DateTime? UnsubscribedDate { get; set; }
        public State SubscribeState { get; set; }
        public State UnsubscribeState { get; set; }
        public string Reasons { get; set; }
        public string Notes { get; set; }
    }
}