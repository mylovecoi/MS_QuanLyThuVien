import axiosClient from '@/services/axiosClient';
import { API_ENDPOINTS } from '@/utils/constants';
import { toErrorText } from '@/utils/apiError';

const triggerBrowserDownload = (blob, fileName) => {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName || 'download';
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
};

const blobToApiError = async (blob) => {
  if (!(blob instanceof Blob) || !blob.type?.includes('application/json')) {
    return null;
  }

  try {
    const text = await blob.text();
    const json = JSON.parse(text);
    return json?.message || json?.errors?.[0] || null;
  } catch {
    return null;
  }
};

export const fileApi = {
  getFiles: (params) =>
    axiosClient.get(API_ENDPOINTS.FILE.LIST, { params }),

  getFileById: (id) =>
    axiosClient.get(API_ENDPOINTS.FILE.BY_ID(id)),

  uploadFile: (formData) =>
    axiosClient.post(API_ENDPOINTS.FILE.UPLOAD, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }),

  updateFile: (id, data) =>
    axiosClient.put(API_ENDPOINTS.FILE.BY_ID(id), data),

  deleteFile: (id) =>
    axiosClient.delete(API_ENDPOINTS.FILE.BY_ID(id)),

  downloadFile: async (id, fileName) => {
    try {
      const blob = await axiosClient.get(API_ENDPOINTS.FILE.DOWNLOAD(id), {
        responseType: 'blob',
      });

      if (!(blob instanceof Blob) || blob.size === 0) {
        throw new Error('Không nhận được dữ liệu tệp tin.');
      }

      const jsonError = await blobToApiError(blob);
      if (jsonError) {
        throw new Error(jsonError);
      }

      triggerBrowserDownload(blob, fileName);
    } catch (err) {
      if (err?.response?.data instanceof Blob) {
        const jsonError = await blobToApiError(err.response.data);
        if (jsonError) {
          throw new Error(jsonError);
        }
      }
      throw new Error(toErrorText(err, 'Tải tệp tin thất bại.'));
    }
  },
};
