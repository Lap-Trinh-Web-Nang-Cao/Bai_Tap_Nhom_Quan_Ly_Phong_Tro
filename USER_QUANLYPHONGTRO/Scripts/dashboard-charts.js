// ===== CHART.JS CONFIGURATION =====
if (typeof Chart !== 'undefined') {
    Chart.defaults.font.family = "'Inter', 'Segoe UI', 'Roboto', sans-serif";
    Chart.defaults.font.size = 12;
    Chart.defaults.color = '#757575';
}

// ===== REVENUE CHART =====
let revenueChart;

function initRevenueChart() {
    const ctx = document.getElementById('revenueChart');
    if (!ctx || typeof Chart === 'undefined') return;

    const loadChart = function(data) {
        if (!data || !data.labels) {
            console.error('Invalid revenue chart data');
            return;
        }

        revenueChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Doanh thu (VNĐ)',
                    data: data.values,
                    borderColor: '#1E88E5',
                    backgroundColor: 'rgba(30, 136, 229, 0.1)',
                    borderWidth: 3,
                    fill: true,
                    tension: 0.4,
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    pointBackgroundColor: '#1E88E5',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        padding: 12,
                        titleFont: {
                            size: 13,
                            weight: '600'
                        },
                        bodyFont: {
                            size: 14,
                            weight: '700'
                        },
                        callbacks: {
                            label: function (context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                label += new Intl.NumberFormat('vi-VN').format(context.parsed.y) + 'đ';
                                return label;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            }
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        },
                        ticks: {
                            callback: function (value) {
                                return (value / 1000000).toFixed(0) + 'M';
                            },
                            font: {
                                size: 11
                            }
                        }
                    }
                }
            }
        });

        window.revenueChart = revenueChart;
    };

    // Use jQuery if available, otherwise use fetch
    if (typeof $ !== 'undefined') {
        $.ajax({
            url: '/ChuTro/GetRevenueChart',
            type: 'GET',
            data: { filter: '30days' },
            success: loadChart,
            error: function (error) {
                console.error('Error loading revenue chart:', error);
                if (ctx && ctx.parentElement) {
                    ctx.parentElement.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #757575;">Không thể tải dữ liệu biểu đồ</div>';
                }
            }
        });
    } else {
        fetch('/ChuTro/GetRevenueChart?filter=30days')
            .then(response => response.json())
            .then(loadChart)
            .catch(error => {
                console.error('Error loading revenue chart:', error);
                if (ctx && ctx.parentElement) {
                    ctx.parentElement.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #757575;">Không thể tải dữ liệu biểu đồ</div>';
                }
            });
    }
}

// ===== VIEWS CHART =====
let viewsChart;

function initViewsChart() {
    const ctx = document.getElementById('viewsChart');
    if (!ctx || typeof Chart === 'undefined') return;

    const loadChart = function(data) {
        if (!data || !data.labels) {
            console.error('Invalid views chart data');
            return;
        }

        viewsChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Lượt xem',
                    data: data.values,
                    backgroundColor: [
                        'rgba(76, 175, 80, 0.8)',
                        'rgba(255, 152, 0, 0.8)',
                        'rgba(33, 150, 243, 0.8)',
                        'rgba(156, 39, 176, 0.8)',
                        'rgba(244, 67, 54, 0.8)'
                    ],
                    borderColor: [
                        '#4CAF50',
                        '#FF9800',
                        '#2196F3',
                        '#9C27B0',
                        '#F44336'
                    ],
                    borderWidth: 2,
                    borderRadius: 8,
                    barThickness: 50
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        padding: 12,
                        titleFont: {
                            size: 13,
                            weight: '600'
                        },
                        bodyFont: {
                            size: 14,
                            weight: '700'
                        },
                        callbacks: {
                            label: function (context) {
                                return 'Lượt xem: ' + context.parsed.y;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            font: {
                                size: 11
                            }
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        },
                        ticks: {
                            stepSize: 10,
                            font: {
                                size: 11
                            }
                        }
                    }
                }
            }
        });

        window.viewsChart = viewsChart;
    };

    // Use jQuery if available, otherwise use fetch
    if (typeof $ !== 'undefined') {
        $.ajax({
            url: '/ChuTro/GetViewsChart',
            type: 'GET',
            success: loadChart,
            error: function (error) {
                console.error('Error loading views chart:', error);
                if (ctx && ctx.parentElement) {
                    ctx.parentElement.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #757575;">Không thể tải dữ liệu biểu đồ</div>';
                }
            }
        });
    } else {
        fetch('/ChuTro/GetViewsChart')
            .then(response => response.json())
            .then(loadChart)
            .catch(error => {
                console.error('Error loading views chart:', error);
                if (ctx && ctx.parentElement) {
                    ctx.parentElement.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100%; color: #757575;">Không thể tải dữ liệu biểu đồ</div>';
                }
            });
    }
}

// ===== INITIALIZE CHARTS ON PAGE LOAD =====
if (typeof $ !== 'undefined') {
    $(document).ready(function () {
        setTimeout(function () {
            initRevenueChart();
            initViewsChart();
        }, 500);
    });
} else {
    document.addEventListener('DOMContentLoaded', function () {
        setTimeout(function () {
            initRevenueChart();
            initViewsChart();
        }, 500);
    });
}

// ===== RESPONSIVE CHART RESIZE =====
window.addEventListener('resize', function () {
    if (window.revenueChart && typeof window.revenueChart.resize === 'function') {
        window.revenueChart.resize();
    }
    if (window.viewsChart && typeof window.viewsChart.resize === 'function') {
        window.viewsChart.resize();
    }
});