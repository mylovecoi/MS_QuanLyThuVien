import React, { useState, useEffect, useCallback } from 'react';
import { Button } from '@/components/ui/Button';
import { Badge } from '@/components/ui/Badge';
import { toErrorText } from '@/utils/apiError';
import { useToast } from '@/context/ToastContext';
import { fileApi } from '../api/fileApi';
import { FileFilterBar } from '../components/FileFilterBar';
import { FileUploadModal } from '../components/FileUploadModal';
import { FileEditModal } from '../components/FileEditModal';
import {
  Upload,
  Download,
  Edit2,
  Trash2,
  File,
  FileText,
  Image as ImageIcon,
  Video,
  Archive,
  HardDrive,
  RefreshCw,
} from 'lucide-react';

export const FileListPage = () => {
  const { showToast } = useToast();
  const [files, setFiles] = useState([]);
  const [loading, setLoading] = useState(false);
  const [downloadingId, setDownloadingId] = useState(null);
  const [error, setError] = useState('');

  // Filter state
  const [keyword, setKeyword] = useState('');
  const [loaiFile, setLoaiFile] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  // Modal states
  const [isUploadOpen, setIsUploadOpen] = useState(false);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [selectedFileForEdit, setSelectedFileForEdit] = useState(null);

  const fetchFiles = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const params = {
        page,
        pageSize: 10,
        keyword: keyword.trim() || undefined,
        loaiFile: loaiFile !== '' ? parseInt(loaiFile, 10) : undefined,
      };

      const result = await fileApi.getFiles(params);
      if (result) {
        setFiles(result.items || []);
        setTotalPages(result.totalPages || 1);
        setTotalCount(result.totalCount || 0);
      }
    } catch (err) {
      setError(toErrorText(err, 'Không thể tải danh sách tệp tin đính kèm.'));
    } finally {
      setLoading(false);
    }
  }, [page, keyword, loaiFile]);

  useEffect(() => {
    fetchFiles();
  }, [fetchFiles]);

  const handleSearch = () => {
    setPage(1);
    fetchFiles();
  };

  const handleReset = () => {
    setKeyword('');
    setLoaiFile('');
    setPage(1);
  };

  const handleDownload = async (file) => {
    setDownloadingId(file.id);
    try {
      await fileApi.downloadFile(file.id, file.fileName);
    } catch (err) {
      showToast(toErrorText(err, 'Tải tệp tin thất bại.'), { type: 'error' });
    } finally {
      setDownloadingId(null);
    }
  };

  const handleDelete = async (file) => {
    if (!window.confirm(`Bạn có chắc chắn muốn xóa tệp tin "${file.fileName}"?`)) {
      return;
    }

    try {
      await fileApi.deleteFile(file.id);
      fetchFiles();
    } catch (err) {
      alert(toErrorText(err, 'Xóa tệp tin thất bại.'));
    }
  };

  const formatFileSize = (bytes) => {
    if (!bytes || bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  const renderFileIcon = (loai) => {
    switch (loai) {
      case 0:
        return <FileText size={18} color="#3b82f6" />;
      case 1:
        return <ImageIcon size={18} color="#10b981" />;
      case 2:
        return <Video size={18} color="#f59e0b" />;
      case 3:
        return <Archive size={18} color="#8b5cf6" />;
      default:
        return <File size={18} color="#6b7280" />;
    }
  };

  const getBadgeVariant = (loai) => {
    switch (loai) {
      case 0:
        return 'primary';
      case 1:
        return 'success';
      case 2:
        return 'warning';
      case 3:
        return 'secondary';
      default:
        return 'secondary';
    }
  };

  return (
    <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '24px 16px' }}>
      {/* Header section */}
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '24px',
        }}
      >
        <div>
          <h1 style={{ fontSize: '1.875rem', fontWeight: 700, margin: 0 }}>
            Quản lý Tệp tin & Tài liệu
          </h1>
          <p style={{ color: 'var(--text-secondary)', margin: '4px 0 0 0', fontSize: '0.95rem' }}>
            Lưu trữ, tải lên, tìm kiếm và phân loại các tệp đính kèm trong hệ thống.
          </p>
        </div>

        <Button
          variant="primary"
          onClick={() => setIsUploadOpen(true)}
          leftIcon={<Upload size={18} />}
        >
          Tải tệp tin lên
        </Button>
      </div>

      {/* Filter Bar */}
      <FileFilterBar
        keyword={keyword}
        onKeywordChange={setKeyword}
        loaiFile={loaiFile}
        onLoaiFileChange={setLoaiFile}
        onSearch={handleSearch}
        onReset={handleReset}
      />

      {/* Error alert */}
      {error && (
        <div
          style={{
            padding: '12px 16px',
            backgroundColor: 'rgba(239, 68, 68, 0.1)',
            border: '1px solid var(--error-color)',
            borderRadius: '10px',
            color: 'var(--error-color)',
            marginBottom: '20px',
          }}
        >
          {error}
        </div>
      )}

      {/* File List Table */}
      <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
          <thead>
            <tr
              style={{
                backgroundColor: 'var(--bg-input)',
                borderBottom: '1px solid var(--border-color)',
                fontSize: '0.85rem',
                color: 'var(--text-secondary)',
                textTransform: 'uppercase',
                letterSpacing: '0.5px',
              }}
            >
              <th style={{ padding: '14px 20px' }}>Tệp tin</th>
              <th style={{ padding: '14px 16px' }}>Loại</th>
              <th style={{ padding: '14px 16px' }}>Kích thước</th>
              <th style={{ padding: '14px 16px' }}>Mô tả</th>
              <th style={{ padding: '14px 16px' }}>Ngày tạo</th>
              <th style={{ padding: '14px 20px', textAlign: 'right' }}>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={6} style={{ padding: '40px', textAlign: 'center', color: 'var(--text-secondary)' }}>
                  <RefreshCw size={24} className="spin" style={{ marginBottom: '8px' }} />
                  <div>Đang tải dữ liệu tệp tin...</div>
                </td>
              </tr>
            ) : files.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ padding: '40px', textAlign: 'center', color: 'var(--text-secondary)' }}>
                  <HardDrive size={36} style={{ marginBottom: '8px', opacity: 0.5 }} />
                  <div>Không tìm thấy tệp tin nào.</div>
                </td>
              </tr>
            ) : (
              files.map((file) => (
                <tr
                  key={file.id}
                  style={{
                    borderBottom: '1px solid var(--border-color)',
                    transition: 'background-color 0.15s',
                  }}
                >
                  <td style={{ padding: '14px 20px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                      {renderFileIcon(file.loaiFile)}
                      <div>
                        <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>
                          {file.fileName}
                        </div>
                        <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>
                          SHA256: {file.sha256 ? `${file.sha256.substring(0, 12)}...` : 'N/A'}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td style={{ padding: '14px 16px' }}>
                    <Badge variant={getBadgeVariant(file.loaiFile)}>
                      {file.loaiFileText || 'Khác'}
                    </Badge>
                  </td>
                  <td style={{ padding: '14px 16px', fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
                    {formatFileSize(file.fileSize)}
                  </td>
                  <td style={{ padding: '14px 16px', fontSize: '0.9rem', color: 'var(--text-secondary)', maxWidth: '250px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                    {file.moTa || '—'}
                  </td>
                  <td style={{ padding: '14px 16px', fontSize: '0.85rem', color: 'var(--text-secondary)' }}>
                    {new Date(file.createdDate).toLocaleDateString('vi-VN')}
                  </td>
                  <td style={{ padding: '14px 20px', textAlign: 'right' }}>
                    <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '6px' }}>
                      <Button
                        variant="ghost"
                        size="sm"
                        title="Tải về"
                        disabled={downloadingId === file.id}
                        onClick={() => handleDownload(file)}
                      >
                        <Download size={16} />
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        title="Sửa thông tin"
                        onClick={() => {
                          setSelectedFileForEdit(file);
                          setIsEditOpen(true);
                        }}
                      >
                        <Edit2 size={16} />
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        title="Xóa"
                        onClick={() => handleDelete(file)}
                        style={{ color: 'var(--error-color)' }}
                      >
                        <Trash2 size={16} />
                      </Button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>

        {/* Pagination */}
        {totalPages > 1 && (
          <div
            style={{
              padding: '16px 20px',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              borderTop: '1px solid var(--border-color)',
            }}
          >
            <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>
              Hiển thị tổng cộng <strong>{totalCount}</strong> tệp tin
            </div>
            <div style={{ display: 'flex', gap: '8px' }}>
              <Button
                variant="secondary"
                size="sm"
                disabled={page <= 1}
                onClick={() => setPage((p) => Math.max(1, p - 1))}
              >
                Trang trước
              </Button>
              <span style={{ display: 'flex', alignItems: 'center', padding: '0 8px', fontSize: '0.9rem' }}>
                Trang {page} / {totalPages}
              </span>
              <Button
                variant="secondary"
                size="sm"
                disabled={page >= totalPages}
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              >
                Trang sau
              </Button>
            </div>
          </div>
        )}
      </div>

      {/* Modals */}
      <FileUploadModal
        isOpen={isUploadOpen}
        onClose={() => setIsUploadOpen(false)}
        onSuccess={fetchFiles}
      />

      <FileEditModal
        isOpen={isEditOpen}
        onClose={() => {
          setIsEditOpen(false);
          setSelectedFileForEdit(null);
        }}
        fileData={selectedFileForEdit}
        onSuccess={fetchFiles}
      />
    </div>
  );
};
