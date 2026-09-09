# 🎨 DESIGN SYSTEM GUIDELINES - MICROSERVICE PLATFORM

Tài liệu Hướng dẫn Hệ thống Thiết kế (Design System) này quy định các tiêu chuẩn giao diện người dùng (UI/UX) chuẩn hóa dành cho ứng dụng **Microservice Platform**. Tất cả các Lập trình viên và AI Assistant phải tuân thủ nghiêm ngặt quy tắc này để bảo đảm tính thống nhất tuyệt đối về thẩm mỹ, màu sắc, khoảng cách, phong cách nút bấm (Buttons), ô nhập liệu (Inputs), thẻ (Cards) và hiệu ứng chuyển động.

---

## 1. 🌈 Bảng Màu & Design Tokens (Color Palette)

Ứng dụng hỗ trợ 2 chế độ Giao diện: **Light Mode** (Mặc định) và **Dark Mode** (Xám tối pha xanh Slate `#0F172A`).
**Quy tắc:** Không sử dụng màu đen tuyền `#000000` cho nền tối.

### 🔹 Tokens trong CSS Variables (`src/assets/styles.css`)

```css
/* Light Theme */
:root, [data-theme="light"] {
  --bg-primary: #f8fafc;
  --bg-card: #ffffff;
  --bg-input: #f1f5f9;
  --text-primary: #0f172a;
  --text-secondary: #64748b;
  --accent-color: #4f46e5;      /* Indigo Primary */
  --accent-hover: #4338ca;
  --accent-light: rgba(79, 70, 229, 0.12);
  --border-color: #e2e8f0;
  --error-color: #dc2626;
  --success-color: #16a34a;
  --warning-color: #d97706;
  --shadow-color: rgba(148, 163, 184, 0.12);
}

/* Dark Theme */
[data-theme="dark"] {
  --bg-primary: #0f172a;        /* Slate 900 */
  --bg-card: #1e293b;           /* Slate 800 */
  --bg-input: #334155;          /* Slate 700 */
  --text-primary: #f8fafc;      /* Slate 50 */
  --text-secondary: #94a3b8;    /* Slate 400 */
  --accent-color: #6366f1;      /* Indigo Accent Dark */
  --accent-hover: #4f46e5;
  --accent-light: rgba(99, 102, 241, 0.2);
  --border-color: #334155;
  --error-color: #ef4444;
  --success-color: #10b981;
  --warning-color: #f59e0b;
  --shadow-color: rgba(15, 23, 42, 0.4);
}
```

---

## 2. 🔤 Font chữ & Typography

- **Primary Font**: `'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif`
- **Tỉ lệ Tiêu đề (Headings)**:
  - `h1`: `1.875rem` (30px), `font-weight: 700`
  - `h2`: `1.5rem` (24px), `font-weight: 600`
  - `h3`: `1.25rem` (20px), `font-weight: 600`
  - `body`: `0.95rem` (15.2px), `line-height: 1.5`
  - `label / caption`: `0.85rem` (13.6px), `color: var(--text-secondary)`

---

## 3. 🔳 Quy chuẩn Nút bấm (Button Variants)

Mọi Button trong hệ thống **BẮT BUỘC** phải dùng component `<Button />` từ `@/components/ui/Button` hoặc sử dụng đúng class CSS quy định bên dưới.

### Các biến thể (Variants):

| Variant | Class CSS | Công dụng | Màu sắc |
| :--- | :--- | :--- | :--- |
| **Primary** | `.btn.btn-primary` | Hành động chính (Lưu, Xác nhận, Đăng nhập) | Gradient `linear-gradient(135deg, var(--accent-color), var(--accent-hover))` |
| **Secondary**| `.btn.btn-secondary`| Hành động phụ (Hủy, Quay lại) | Nền `var(--bg-input)`, chữ `var(--text-primary)`, viền `var(--border-color)` |
| **Outline**  | `.btn.btn-outline`  | Viền mảnh, trong suốt | Nền trong suốt, viền `var(--accent-color)`, chữ `var(--accent-color)` |
| **Danger**   | `.btn.btn-danger`   | Xóa, Hủy tài khoản | Nền `var(--error-color)`, chữ `#ffffff` |
| **Ghost**    | `.btn.btn-ghost`    | Nút icon, Link button | Không viền/nền, hover đổi sang `var(--accent-light)` |

### Kích thước (Sizes):
- `sm`: Padding `6px 12px`, Font `0.85rem`
- `md`: Padding `10px 18px`, Font `0.95rem` (Mặc định)
- `lg`: Padding `14px 24px`, Font `1.05rem`

### Trạng thái (States):
- **Hover**: Shift nhẹ translateY(-1px) đối với `btn-primary`, tăng độ sáng với shadow.
- **Disabled / Loading**: `opacity: 0.6`, `cursor: not-allowed`, hiển thị spinner loading icon.

---

## 4. 📝 Quy chuẩn Form Control (Inputs, Selects)

Component `<Input />` và `<Select />` tuân thủ:
- **Border radius**: `10px` (`var(--radius-md)`)
- **Focus state**: `outline: none`, `border-color: var(--accent-color)`, `box-shadow: 0 0 0 3px var(--accent-light)`
- **Error state**: Viền đỏ `var(--error-color)`, dòng thông báo lỗi ngay bên dưới cỡ chữ `0.75rem`.

---

## 5. 🏷️ Quy chuẩn Badge & Card

### Thẻ dữ liệu (Card):
- Border radius: `16px` (`var(--radius-lg)`)
- Background: `var(--bg-card)`
- Shadow: `0 10px 30px var(--shadow-color)`
- Border: `1px solid var(--border-color)`

### Nhãn trạng thái (Badge):
- Variants: `primary`, `success`, `warning`, `danger`, `secondary`
- Shape: `border-radius: 9999px` (Pill format) hoặc `6px`.

---

## 6. 🤖 Quy tắc cho AI Generator / Developer

1. **Không tạo thêm style inline cho Button/Input**: Luôn dùng UI components chuẩn tại `@/components/ui`.
2. **Không tự bịa mã màu Hex mới ngoài Tokens**: Chỉ dùng `var(--accent-color)`, `var(--bg-card)`, v.v.
3. **Mỗi Component mới** tạo ra đều phải kiểm tra hiển thị mượt mà trên cả 2 chế độ Light/Dark Theme.
