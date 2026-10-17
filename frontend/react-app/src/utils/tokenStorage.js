// Lưu token ở localStorage (ghi nhớ đăng nhập) hoặc sessionStorage (chỉ trong
// phiên trình duyệt). "Remember me" quyết định dùng store nào; các nơi khác chỉ
// đọc qua helper này để không rải rác window.localStorage khắp codebase.

const TOKEN_KEY = 'token';
const REFRESH_KEY = 'refreshToken';

const isBrowser = () => typeof window !== 'undefined';

/** Xác định store đang giữ token hiện tại (ưu tiên localStorage). */
const activeStore = () => {
  if (!isBrowser()) return null;
  if (window.localStorage.getItem(TOKEN_KEY)) return window.localStorage;
  if (window.sessionStorage.getItem(TOKEN_KEY)) return window.sessionStorage;
  return window.localStorage;
};

export const tokenStorage = {
  getToken() {
    if (!isBrowser()) return null;
    return window.localStorage.getItem(TOKEN_KEY) || window.sessionStorage.getItem(TOKEN_KEY);
  },

  getRefreshToken() {
    if (!isBrowser()) return null;
    return window.localStorage.getItem(REFRESH_KEY) || window.sessionStorage.getItem(REFRESH_KEY);
  },

  /**
   * Lưu cặp token vào store tương ứng với lựa chọn "ghi nhớ".
   * Luôn dọn store còn lại để tránh token cũ "hồi sinh".
   */
  setTokens({ accessToken, refreshToken }, remember) {
    if (!isBrowser()) return;
    const store = remember ? window.localStorage : window.sessionStorage;
    const other = remember ? window.sessionStorage : window.localStorage;

    store.setItem(TOKEN_KEY, accessToken);
    if (refreshToken) store.setItem(REFRESH_KEY, refreshToken);

    other.removeItem(TOKEN_KEY);
    other.removeItem(REFRESH_KEY);
  },

  /** Cập nhật access token sau khi refresh, giữ nguyên store đang dùng. */
  setAccessToken(accessToken) {
    const store = activeStore();
    if (store) store.setItem(TOKEN_KEY, accessToken);
  },

  /** Xóa token ở cả hai store. */
  clear() {
    if (!isBrowser()) return;
    for (const store of [window.localStorage, window.sessionStorage]) {
      store.removeItem(TOKEN_KEY);
      store.removeItem(REFRESH_KEY);
    }
  },
};
