import React, { useState } from 'react';
import { Modal } from '@/components/ui/Modal';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { LOAI_FILE_OPTIONS } from '@/utils/constants';
import { toErrorText } from '@/utils/apiError';
import { fileApi } from '../api/fileApi';
import { UploadCloud, FileText } from 'lucide-react';

export const FileUploadModal = ({ isOpen, onClose, onSuccess }) => {
  const [selectedFile, setSelectedFile] = useState(null);
  const [moTa, setMoTa] = useState('');
  const [loaiFile, setLoaiFile] = useState('0');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleFileChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
      setError('');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!selectedFile) {
      setError('Vui lòng chọn tập tin cần tải lên.');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const formData = new FormData();
      formData.append('File', selectedFile);
      formData.append('MoTa', moTa);
      formData.append('LoaiFile', parseInt(loaiFile, 10));

      await fileApi.uploadFile(formData);
      setSelectedFile(null);
      setMoTa('');
      setLoaiFile('0');
      onSuccess();
      onClose();
    } catch (err) {
      setError(toErrorText(err, 'Tải tệp tin lên thất bại.'));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Tải lên tệp tin mới"
      maxWidth="550px"
      footer={
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', width: '100%' }}>
          <Button variant="secondary" onClick={onClose} disabled={isLoading}>
            Hủy
          </Button>
          <Button variant="primary" onClick={handleSubmit} isLoading={isLoading}>
            Tải lên
          </Button>
        </div>
      }
    >
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
        {error && (
          <div
            style={{
              padding: '10px 14px',
              backgroundColor: 'rgba(239, 68, 68, 0.1)',
              border: '1px solid var(--error-color)',
              borderRadius: '8px',
              color: 'var(--error-color)',
              fontSize: '0.9rem',
            }}
          >
            {error}
          </div>
        )}

        <div
          style={{
            border: '2px dashed var(--border-color)',
            borderRadius: '12px',
            padding: '24px',
            textAlign: 'center',
            backgroundColor: 'var(--bg-input)',
            cursor: 'pointer',
            transition: 'border-color 0.2s',
          }}
          onClick={() => document.getElementById('file-upload-input').click()}
        >
          <input
            id="file-upload-input"
            type="file"
            onChange={handleFileChange}
            style={{ display: 'none' }}
          />
          <UploadCloud size={40} color="var(--accent-color)" style={{ marginBottom: '8px' }} />
          <p style={{ fontWeight: 600, margin: '4px 0', color: 'var(--text-primary)' }}>
            {selectedFile ? selectedFile.name : 'Nhấp vào đây để chọn tệp tin'}
          </p>
          <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', margin: 0 }}>
            {selectedFile
              ? `Kích thước: ${(selectedFile.size / (1024 * 1024)).toFixed(2)} MB`
              : 'Hỗ trợ PDF, Word, Excel, Hình ảnh, Zip...'}
          </p>
        </div>

        <Select
          label="Loại tệp tin"
          value={loaiFile}
          onChange={(e) => setLoaiFile(e.target.value)}
          options={LOAI_FILE_OPTIONS}
        />

        <div>
          <label
            style={{
              display: 'block',
              fontSize: '0.85rem',
              color: 'var(--text-secondary)',
              marginBottom: '6px',
            }}
          >
            Mô tả tệp tin
          </label>
          <textarea
            value={moTa}
            onChange={(e) => setMoTa(e.target.value)}
            rows={3}
            placeholder="Nhập mô tả tóm tắt hoặc từ khóa gợi nhớ..."
            style={{
              width: '100%',
              padding: '10px 14px',
              borderRadius: '10px',
              border: '1px solid var(--border-color)',
              backgroundColor: 'var(--bg-card)',
              color: 'var(--text-primary)',
              fontFamily: 'inherit',
              fontSize: '0.95rem',
              resize: 'vertical',
            }}
          />
        </div>
      </form>
    </Modal>
  );
};
