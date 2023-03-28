using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Subscribers;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly BlogDbContext _context;

    private readonly IMemoryCache _memoryCache;

    public SubscriberRepository(BlogDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
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

    public async Task<Subscriber> GetCachedSubscriberByIdAsync(int subscriberId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"subscriber.by-id.{subscriberId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetSubscriberByIdAsync(subscriberId);
            });
    }

    public async Task<Subscriber> GetCachedSubscriberByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"subscriber.by-email.{email}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetSubscriberByEmailAsync(email, cancellationToken);
            });
    }

    public async Task<IPagedList<Subscriber>> GetPagedSubscribersAsync(
           IPagingParams pagingParams,
           string name = null,
           CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                x => x.Email.Contains(name))
            .Select(a => new Subscriber()
            {
                Id = a.Id,
                Email = a.Email,
                SubscribedDate = a.SubscribedDate,
                UnsubscribedDate = a.UnsubscribedDate,
                SubscribeState = a.SubscribeState,
                UnsubscribeState = a.UnsubscribeState,
                Reasons = a.Reasons,
                Notes = a.Notes
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<IPagedList<T>> GetPagedSubscribersAsync<T>(
       Func<IQueryable<Subscriber>, IQueryable<T>> mapper,
       IPagingParams pagingParams,
       string name = null,
       CancellationToken cancellationToken = default)
    {
        var subscriberQuery = _context.Set<Subscriber>().AsNoTracking();

        if (!string.IsNullOrEmpty(name))
        {
            subscriberQuery = subscriberQuery.Where(x => x.Email.Contains(name));
        }

        return await mapper(subscriberQuery)
            .ToPagedListAsync(pagingParams, cancellationToken);
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

    public async Task<bool> IsSubscriberExistedEmail(
        int id,
           string email,
           CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
            .AnyAsync(s => s.Id != id && s.Email.Equals(email), cancellationToken);
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

    public async Task<Subscriber> CreateOrUpdateSubscriberAsync(
       Subscriber subscriber, CancellationToken cancellationToken = default)
    {
        if (subscriber.Id > 0)
        {
            _context.Set<Subscriber>().Update(subscriber);
        }
        else
        {
            _context.Set<Subscriber>().Add(subscriber);
        }
        await _context.SaveChangesAsync(cancellationToken);

        return subscriber;
    }

    //Câu 2. E : Thêm hoặc cập nhật thông tin một đăng ký
    public async Task<bool> AddOrUpdateSubscriberAsync(
         Subscriber subscriber,
         CancellationToken cancellationToken = default)
    {
        if (subscriber.Id > 0)
        {
            _context.Subscribers.Update(subscriber);
            _memoryCache.Remove($"subscriber.by-id.{subscriber.Id}");
        }
        else
        {
            _context.Subscribers.Add(subscriber);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
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