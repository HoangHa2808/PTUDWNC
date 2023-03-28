using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities;

public enum State
{
    Subscribe,
    Unsubscribe,
    Block
}

public class Subscriber
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime SubscribedDate { get; set; }
    public DateTime? UnsubscribedDate { get; set; }
    public State SubscribeState { get; set; }
    public State UnsubscribeState { get; set; }
    public string Reasons { get; set; }
    public string Notes { get; set; }
}