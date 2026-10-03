/**
 * Chuẩn hoá lỗi từ axios về một shape duy nhất để component không phải tự
 * đoán từng nhánh lỗi của axios.
 *
 * Lý do tồn tại: response interceptor của axios có nhiều nhánh reject khác
 * nhau (timeout, 401 hết refresh token, envelope của backend, lỗi mạng thuần).
 * Nếu component chỉ đọc `err.message` thì chỗ nào rơi vào nhánh không phải
 * envelope sẽ hiện ra chuỗi vô nghĩa của axios là
 * "Request failed with status code 400".
 *
 * Backend luôn trả envelope { success, message, errors[], traceId } cho cả lỗi
 * nghiệp vụ lẫn lỗi validation, nên `errors` là nơi chứa thông báo cụ thể theo
 * từng trường. `message` chỉ là câu tổng quát và thường không giúp người dùng
 * sửa được gì.
 */

/** Ép mọi kiểu `errors` về mảng chuỗi.
 *
 * Lý do: ASP.NET `ValidationProblemDetails` trả `errors` dạng object
 * `{ "Username": ["..."] }`, không phải mảng. Gọi `.join()` trên object đó sẽ
 * ném TypeError và làm mất cả thông báo lỗi.
 */
const toErrorList = (raw) => {
  if (Array.isArray(raw)) {
    return raw.filter((item) => typeof item === 'string' && item.trim() !== '');
  }

  if (raw && typeof raw === 'object') {
    return Object.entries(raw)
      .map(([field, messages]) => {
        const list = Array.isArray(messages) ? messages : [messages];
        const text = list.filter((m) => typeof m === 'string' && m.trim() !== '').join(' ');
        return text ? `${field}: ${text}` : '';
      })
      .filter(Boolean);
  }

  if (typeof raw === 'string' && raw.trim() !== '') {
    return [raw];
  }

  return [];
};

/**
 * Trả về { message, errors, status, traceId }.
 *
 * @param {unknown} err giá trị reject từ axios (đã qua interceptor hoặc chưa).
 * @param {string} fallback câu thông báo dùng khi không lấy được gì từ server.
 * @returns {{ message: string, errors: string[], status?: number, traceId?: string }}
 */
export const normalizeApiError = (err, fallback = 'Đã xảy ra lỗi.') => {
  if (!err || typeof err !== 'object') {
    return { message: fallback, errors: [] };
  }

  // Nhánh đã chuẩn hoá sẵn của interceptor: ưu tiên errors lấy từ body.
  const directErrors = toErrorList(err.errors);
  if (directErrors.length > 0) {
    return {
      message: err.message || directErrors[0],
      errors: directErrors,
      status: err.status,
      traceId: err.traceId,
    };
  }

  // Nhánh raw axios error: bóc tách từ response.data.
  const body = err.response?.data;
  if (body && typeof body === 'object') {
    const bodyErrors = toErrorList(body.errors);
    return {
      message: body.message || fallback,
      errors: bodyErrors,
      status: err.response.status,
      traceId: body.traceId,
    };
  }

  // Lỗi mạng/timeout: không có response, dùng message sẵn có của axios.
  if (err.message) {
    return { message: err.message, errors: [] };
  }

  return { message: fallback, errors: [] };
};

/** Gộp thông báo để hiển thị trong một Alert.
 *
 * Ưu tiên danh sách errors vì nó chỉ rõ trường nào sai; chỉ dùng `message` khi
 * errors rỗng (lỗi nghiệp vụ không gắn với trường nào, ví dụ sai mật khẩu).
 */
export const toErrorText = (err, fallback) => {
  const { message, errors } = normalizeApiError(err, fallback);
  return errors.length > 0 ? errors.join(' ') : message;
};