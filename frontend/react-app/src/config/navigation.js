/**
 * Menu cây theo kiến trúc microservice thư viện.
 * - path: route đã có FE/BE
 * - comingSoon: chưa có service/UI → toast "đang phát triển"
 * - children: nhánh con (tree)
 */
export const NAV_SECTIONS = [
  {
    title: 'Tài khoản',
    items: [
      { id: 'profile', label: 'Hồ sơ', icon: 'User', path: '/profile' },
      { id: 'users', label: 'Quản lý tài khoản', icon: 'Users', path: '/users', permission: 'users.index' },
      { id: 'roles', label: 'Vai trò & quyền', icon: 'Shield', path: '/roles', permission: 'roles.index' },
    ],
  },
  {
    title: 'Nghiệp vụ thư viện',
    items: [
      {
        id: 'file-service',
        label: 'Lưu trữ tệp',
        icon: 'HardDrive',
        defaultOpen: true,
        children: [
          { id: 'files', label: 'Quản lý tệp tin', icon: 'FolderOpen', path: '/files' },
        ],
      },
      {
        id: 'catalog-service',
        label: 'Biên mục & Kho',
        icon: 'Library',
        children: [
          { id: 'catalog-bib', label: 'Biên mục sách', icon: 'BookOpen', comingSoon: true },
          { id: 'catalog-copies', label: 'Bản sách / Inventory', icon: 'BookCopy', comingSoon: true },
          { id: 'catalog-shelves', label: 'Kệ & vị trí kho', icon: 'MapPin', comingSoon: true },
          { id: 'catalog-authors', label: 'Tác giả / NXB', icon: 'Feather', comingSoon: true },
          { id: 'catalog-subjects', label: 'Chủ đề / Thể loại', icon: 'Tags', comingSoon: true },
        ],
      },
      {
        id: 'circulation-service',
        label: 'Lưu thông',
        icon: 'Repeat',
        children: [
          { id: 'loan-borrow', label: 'Mượn tài liệu', icon: 'BookMarked', comingSoon: true },
          { id: 'loan-return', label: 'Trả tài liệu', icon: 'Undo2', comingSoon: true },
          { id: 'loan-renew', label: 'Gia hạn', icon: 'Clock', comingSoon: true },
          { id: 'loan-hold', label: 'Đặt trước', icon: 'Bookmark', comingSoon: true },
          { id: 'loan-fine', label: 'Vi phạm / Phạt', icon: 'AlertTriangle', comingSoon: true },
        ],
      },
      {
        id: 'acquisition-service',
        label: 'Bổ sung tài liệu',
        icon: 'ShoppingCart',
        children: [
          { id: 'acq-suggest', label: 'Đề xuất mua', icon: 'Lightbulb', comingSoon: true },
          { id: 'acq-order', label: 'Đơn đặt hàng', icon: 'ClipboardList', comingSoon: true },
          { id: 'acq-receive', label: 'Nhập kho', icon: 'PackagePlus', comingSoon: true },
          { id: 'acq-vendor', label: 'Nhà cung cấp', icon: 'Truck', comingSoon: true },
        ],
      },
      {
        id: 'review-service',
        label: 'Tương tác & AI',
        icon: 'MessageSquare',
        children: [
          { id: 'review-rating', label: 'Đánh giá', icon: 'Star', comingSoon: true },
          { id: 'review-comment', label: 'Bình luận', icon: 'MessagesSquare', comingSoon: true },
          { id: 'review-chat', label: 'AI Chat', icon: 'Bot', comingSoon: true },
        ],
      },
      {
        id: 'notification-service',
        label: 'Thông báo',
        icon: 'Bell',
        children: [
          { id: 'notify-inbox', label: 'Hộp thư thông báo', icon: 'Inbox', comingSoon: true },
          { id: 'notify-templates', label: 'Mẫu thông báo', icon: 'Mail', comingSoon: true },
          { id: 'notify-channels', label: 'Kênh gửi', icon: 'Radio', comingSoon: true },
        ],
      },
      {
        id: 'search-service',
        label: 'Tra cứu & Discovery',
        icon: 'Search',
        children: [
          { id: 'search-catalog', label: 'Tìm kiếm tài liệu', icon: 'Search', comingSoon: true },
          { id: 'search-discover', label: 'Gợi ý khám phá', icon: 'Compass', comingSoon: true },
        ],
      },
      {
        id: 'reporting-service',
        label: 'Báo cáo & Thống kê',
        icon: 'BarChart3',
        children: [
          { id: 'report-circulation', label: 'Thống kê lưu thông', icon: 'LineChart', comingSoon: true },
          { id: 'report-inventory', label: 'Thống kê kho', icon: 'PieChart', comingSoon: true },
          { id: 'report-custom', label: 'Báo cáo tùy chỉnh', icon: 'FileBarChart', comingSoon: true },
        ],
      },
    ],
  },
];
