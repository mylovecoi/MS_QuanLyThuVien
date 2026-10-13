import React, { useState, useEffect } from 'react';
import { Modal } from '@/components/ui/Modal';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { LOAI_FILE_OPTIONS } from '@/utils/constants';
import { toErrorText } from '@/utils/apiError';
import { fileApi } from '../api/fileApi';

export const FileEditModal = ({ isOpen, onClose, fileData, onSuccess }) => {
  const [fileName, setFileName] = useState('');
  const [moTa, setMoTa] = useState('');
  const [loaiFile, setLoaiFile] = useState('0');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (fileData) {
      setFileName(fileData.fileName || '');
      setMoTa(fileData.moTa || '');
      setLoaiFile(fileData.loaiFile !== undefined ? String(fileData.loaiFile) : '0');
      setError('');
    }
  }, [fileData]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!fileData?.id) return;

    setIsLoading(true);
    setError('');

    try {
      await fileApi.updateFile(fileData.id, {
        fileName,
        moTa,
        loaiFile: parseInt(loaiFile, 10),
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError(toErrorText(err, 'Cập nhật thông tin tệp tin thất bại.'));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Cập nhật thông tin tệp tin"
      maxWidth="550px"
      footer={
        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', width: '100%' }}>
          <Button variant="secondary" onClick={onClose} disabled={isLoading}>
            Hủy
          </Button>
          <Button variant="primary" onClick={handleSubmit} isLoading={isLoading}>
            Lưu thay đổi
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

        <Input
          label="Tên tệp tin"
          value={fileName}
          onChange={(e) => setFileName(e.target.value)}
          placeholder="Nhập tên tệp tin..."
          required
        />

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
            Mô tả
          </label>
          <textarea
            value={moTa}
            onChange={(e) => setMoTa(e.target.value)}
            rows={3}
            placeholder="Nhập mô tả tệp tin..."
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
