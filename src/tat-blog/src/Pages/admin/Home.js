import { useEffect, useState } from "react";
import { getDashboard } from "../../services/BlogRepository";
import Loading from "../../components/Loading";

export default function AdminHome() {
    const [dashboard, setDashboard] = useState([]),
        [isVisibleLoading, setIsVisibleLoading] = useState(true);


    useEffect(() => {
        document.title = 'Thống kê';

        getDashboard().then(data => {
            console.log("data:")
            console.log(data)
            if (data)
                setDashboard(data);
            else
                setDashboard([]);
            setIsVisibleLoading(false);
        });
    }, []);
    return (
        <>
            <h1>Danh sách thống kê </h1>
            {isVisibleLoading ? <Loading /> :
                <div className="main">
                    <main className="content">
                        <div className="container-fluid p-0">

                            {/* <h1 className="h3 mb-3"><strong>Bảng điều khiển</strong> Dashboard</h1> */}

                            <div className="row">
                                <div className="col-xl-12 col-xxl-12 d-flex">
                                    <div className="w-100">
                                        <div className="row" style={{ padding: "20px" }}>
                                            <div className="col-sm-4">
                                                <div className="card">
                                                    <div className="card-body">
                                                        <div className="row">
                                                            <div className="col mt-0">
                                                                <h5 className="card-title">Tổng số bài viết</h5>
                                                            </div>

                                                            <div className="col-auto">
                                                                <div className="stat text-primary">
                                                                    <i className="align-middle" data-feather="truck"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <h1 className="mt-1 mb-3">{dashboard.totalPosts}</h1>
                                                        <div className="mb-0">
                                                            <span className="text-danger"> <i className="mdi mdi-arrow-bottom-right"></i></span>
                                                            <span className="text-muted">Since last week</span>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div className="card">
                                                    <div className="card-body">
                                                        <div className="row">
                                                            <div className="col mt-0">
                                                                <h5 className="card-title">Số lượng tác giả</h5>
                                                            </div>

                                                            <div className="col-auto">
                                                                <div className="stat text-primary">
                                                                    <i className="align-middle" data-feather="users"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <h1 className="mt-1 mb-3">{dashboard.totalAuthors}</h1>
                                                        <div className="mb-0">
                                                            <span className="text-success"> <i className="mdi mdi-arrow-bottom-right"></i></span>
                                                            <span className="text-muted">Since last week</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div className="col-sm-4">
                                                <div className="card">
                                                    <div className="card-body">
                                                        <div className="row">
                                                            <div className="col mt-0">
                                                                <h5 className="card-title">Số lượng chủ đề</h5>
                                                            </div>

                                                            <div className="col-auto">
                                                                <div className="stat text-primary">
                                                                    <i className="align-middle" data-feather="dollar-sign"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <h1 className="mt-1 mb-3">{dashboard.totalCategories}</h1>
                                                        <div className="mb-0">
                                                            <span className="text-success"> <i className="mdi mdi-arrow-bottom-right"></i></span>
                                                            <span className="text-muted">Since last week</span>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div className="card">
                                                    <div className="card-body">
                                                        <div className="row">
                                                            <div className="col mt-0">
                                                                <h5 className="card-title">Số lượng người theo dõi</h5>
                                                            </div>

                                                            <div className="col-auto">
                                                                <div className="stat text-primary">
                                                                    <i className="align-middle" data-feather="shopping-cart"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <h1 className="mt-1 mb-3">{dashboard.totalSubscribers}</h1>
                                                        <div className="mb-0">
                                                            <span className="text-danger"> <i className="mdi mdi-arrow-bottom-right"></i></span>
                                                            <span className="text-muted">Since last week</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div className="col-sm-4">
                                                <div className="card">
                                                    <div className="card-body">
                                                        <div className="row">
                                                            <div className="col mt-0">
                                                                <h5 className="card-title">Số bài viết chưa xuất bản</h5>
                                                            </div>

                                                            <div className="col-auto">
                                                                <div className="stat text-primary">
                                                                    <i className="align-middle" data-feather="dollar-sign"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <h1 className="mt-1 mb-3">{dashboard.totalUnpublishedPosts}</h1>
                                                        <div className="mb-0">
                                                            <span className="text-success"> <i className="mdi mdi-arrow-bottom-right"></i></span>
                                                            <span className="text-muted">Since last week</span>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div className="card">
                                                    <div className="card-body">
                                                        <div className="row">
                                                            <div className="col mt-0">
                                                                <h5 className="card-title">Số lượng bình luận đang chờ phê duyệt</h5>
                                                            </div>

                                                            <div className="col-auto">
                                                                <div className="stat text-primary">
                                                                    <i className="align-middle" data-feather="shopping-cart"></i>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <h1 className="mt-1 mb-3">{dashboard.totalUnapprovedComments}</h1>
                                                        <div className="mb-0">
                                                            <span className="text-danger"> <i className="mdi mdi-arrow-bottom-right"></i></span>
                                                            <span className="text-muted">Since last week</span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div className="row" style={{ padding: "0 24px 0 24px" }}>
                                <div className="col-12 col-lg-8 col-xxl-9 d-flex">
                                    <div className="card flex-fill">
                                        <div className="card-header">

                                            <h5 className="card-title mb-0">Số lượng người mới theo dõi đăng ký</h5>
                                            {dashboard.totalNewSubscriberToday}
                                        </div>
                                        <table className="table table-hover my-0">
                                            <thead>
                                                <tr>
                                                    <th>Name</th>
                                                    <th className="d-none d-xl-table-cell">Start Date</th>
                                                    <th className="d-none d-xl-table-cell">End Date</th>
                                                    <th>Status</th>
                                                    <th className="d-none d-md-table-cell">Assignee</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>Project Apollo</td>
                                                    <td className="d-none d-xl-table-cell">01/01/2021</td>
                                                    <td className="d-none d-xl-table-cell">31/06/2021</td>
                                                    <td><span className="badge bg-success">Done</span></td>
                                                    <td className="d-none d-md-table-cell">Vanessa Tucker</td>
                                                </tr>
                                                <tr>
                                                    <td>Project Fireball</td>
                                                    <td className="d-none d-xl-table-cell">01/01/2021</td>
                                                    <td className="d-none d-xl-table-cell">31/06/2021</td>
                                                    <td><span className="badge bg-danger">Cancelled</span></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div className="col-12 col-lg-4 col-xxl-3 d-flex">
                                    <div className="card flex-fill w-100">
                                        <div className="card-header">
                                            <h5 className="card-title mb-0">Browser Usage</h5>
                                        </div>
                                        <div className="card-body d-flex">
                                            <div className="align-self-center w-100">
                                                <div className="py-3">
                                                    <div className="chart chart-xs">
                                                        <canvas id="chartjs-dashboard-pie"></canvas>
                                                    </div>
                                                </div>
                                                <table className="table mb-0">
                                                    <tbody>
                                                        <tr>
                                                            <td>Tác giả</td>
                                                            <td className="text-end">{dashboard.totalAuthors}</td>
                                                        </tr>
                                                        <tr>
                                                            <td>Bài viết</td>
                                                            <td className="text-end">{dashboard.totalPosts}</td>
                                                        </tr>
                                                        <tr>
                                                            <td>Chủ đề</td>
                                                            <td className="text-end">{dashboard.totalCategories}</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </main>

                </div>
            }
        </>
    );
}