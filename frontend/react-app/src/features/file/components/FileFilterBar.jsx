import React from 'react';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Button } from '@/components/ui/Button';
import { LOAI_FILE_OPTIONS } from '@/utils/constants';
import { Search, RotateCcw } from 'lucide-react';

export const FileFilterBar = ({
  keyword,
  onKeywordChange,
  loaiFile,
  onLoaiFileChange,
  onSearch,
  onReset,
}) => {
  const loaiFileOptions = [
    { value: '', label: 'Tất cả loại file' },
    ...LOAI_FILE_OPTIONS,
  ];

  return (
    <div
      className="card"
      style={{
        padding: '16px 20px',
        marginBottom: '20px',
        display: 'flex',
        flexWrap: 'wrap',
        gap: '16px',
        alignItems: 'center',
      }}
    >
      <div style={{ flex: '1 1 250px' }}>
        <Input
          placeholder="Tìm kiếm theo tên tệp tin, mô tả..."
          value={keyword}
          onChange={(e) => onKeywordChange(e.target.value)}
          onKeyDown={(e) => e.key === 'Enter' && onSearch()}
        />
      </div>

      <div style={{ width: '220px' }}>
        <Select
          value={loaiFile}
          onChange={(e) => onLoaiFileChange(e.target.value)}
          options={loaiFileOptions}
        />
      </div>

      <div style={{ display: 'flex', gap: '8px' }}>
        <Button variant="primary" onClick={onSearch} leftIcon={<Search size={16} />}>
          Tìm kiếm
        </Button>
        <Button variant="secondary" onClick={onReset} leftIcon={<RotateCcw size={16} />}>
          Đặt lại
        </Button>
      </div>
    </div>
  );
};
