using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Subscribers;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly BlogDbContext _context;

    public SubscriberRepository(BlogDbContext context)
    {
        _context = context;
    }

    // Tìm người theo dõi = ID
    public async Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
        .Where(s => s.Email.Equals(email))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IPagedList<Subscriber>> SearchSubscribersAsync(IPagingParams pagingParams, string keywords, State subscribeStatus, CancellationToken cancellationToken = default)
    {
        var subscriberR = _context.Set<Subscriber>()
                .Where(s => s.Email.Contains(keywords) ||
                            s.Reasons.ToLower().Contains(keywords.ToLower()) ||
                            s.Notes.ToLower().Contains(keywords.ToLower()) ||
                            s.SubscribeState == subscribeStatus);
        return await subscriberR.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<bool> IsExistedEmail(
           string email,
           CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
            .AnyAsync(s => s.Email.Equals(email), cancellationToken);
    }

    public async Task<IList<Subscriber>> GetSubscribersAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
             .Where(x => x.SubscribeState == State.Subscribe)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        Subscriber s = null;
        if (!string.IsNullOrWhiteSpace(email) && !IsExistedEmail(email).Result)
        {
            s = new Subscriber()
            {
                Email = email,
                SubscribedDate = DateTime.Now,
            };
            _context.Subscribers.Add(s);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
        return false;
    }

    public async Task<bool> UnsubscribeAsync(string email, string reason, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(email) && !IsExistedEmail(email).Result)
        {
            await _context.Set<Subscriber>()
                .Where(s => s.Email.Equals(email))
                .ExecuteUpdateAsync(h => h
                .SetProperty(t => t.Reasons, t => reason)
                .SetProperty(t => t.UnsubscribeState, t => State.Unsubscribe),
                cancellationToken);
        }
        return true;
    }

    public async Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default)
    {
        if (id > 0)
        {
            await _context.Set<Subscriber>()
                .Where(s => s.Id == id)
                .ExecuteUpdateAsync(h => h
                .SetProperty(t => t.Reasons, reason)
                .SetProperty(t => t.Notes, notes), cancellationToken);
        }
        return false;
    }

    public async Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
             .Where(s => s.Id == id)
             .ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<IPagedList<Subscriber>> GetPagedSubscriberAsync(
     int pageNumber = 1,
     int pageSize = 10,
     CancellationToken cancellationToken = default)
    {
        IQueryable<Subscriber> subQuery = _context.Set<Subscriber>();

        return await subQuery.ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Subscriber.Email), "DESC",
            cancellationToken);
    }

    #region Dashboard

    // Số lượng người theo dõi, số lượng người mới theo dõi đăng ký(lấy số liệu trong ngày)
    public async Task<int> CountSubAsync(
       CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountSubscriberStateAsync(
       CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
                .CountAsync(x => x.SubscribedDate.CompareTo(DateTime.Now) == 0, cancellationToken);
    }

    #endregion Dashboard
}