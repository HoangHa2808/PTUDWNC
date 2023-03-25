using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Subscribers;

public interface ISubscriberRepository
{
    //Task<IList<Subscriber>> GetSubscribersAsync(CancellationToken cancellationToken = default);

    // Tìm người theo dõi = ID
    Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default);

    // Tìm người theo dõi = Email
    Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Tìm danh sách người theo dõi
    Task<IPagedList<Subscriber>> SearchSubscribersAsync(IPagingParams pagingParams, string keywords, State subscribeStatus, CancellationToken cancellationToken = default);

    // Đăng ký theo dõi
    Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default);

    // Huỷ đăng ký theo dõi
    Task<bool> UnsubscribeAsync(string email, string reason, CancellationToken cancellationToken = default);

    // Chặn 1 người theo dõi
    Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default);

    // Xoá 1 người theo dõi
    Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default);

}
