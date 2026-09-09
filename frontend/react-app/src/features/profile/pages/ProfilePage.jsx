import React, { useState, useEffect } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { profileApi } from '../api/profileApi';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Badge } from '@/components/ui/Badge';
import { Modal } from '@/components/ui/Modal';
import { GENDER_OPTIONS } from '@/utils/constants';
import { formatDate } from '@/utils/formatters';

export const ProfilePage = () => {
  const { user } = useAuth();
  const [profile, setProfile] = useState(null);
  const [phongBans, setPhongBans] = useState([]);
  const [chucVus, setChucVus] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [activeTab, setActiveTab] = useState('identity'); // 'identity' | 'contact' | 'organization'
  const [message, setMessage] = useState(null);

  // Form State
  const [formData, setFormData] = useState({
    maCanBo: '',
    fullName: '',
    cccd: '',
    ngaySinh: '',
    gender: 0,
    phoneNumber: '',
    coQuanPhoneNumber: '',
    workEmail: '',
    avatarUrl: '',
    diaChiHienNay: '',
    phongBanId: '',
    chucVuId: '',
  });

  useEffect(() => {
    fetchInitialData();
  }, [user]);

  const fetchInitialData = async () => {
    setLoading(true);
    try {
      // 1. Fetch danh mục Phòng ban và Chức vụ
      const [pbList, cvList] = await Promise.all([
        profileApi.getPhongBans().catch(() => []),
        profileApi.getChucVus().catch(() => []),
      ]);
      setPhongBans(pbList || []);
      setChucVus(cvList || []);

      // 2. Fetch UserProfile của User hiện tại
      if (user?.id) {
        try {
          const userProf = await profileApi.getProfileByUserId(user.id);
          if (userProf) {
            setProfile(userProf);
            populateForm(userProf);
          }
        } catch (err) {
          // Profile chưa tồn tại -> tạo mới mặc định
          const defaultProf = {
            userId: user.id,
            fullName: user.fullName || '',
            workEmail: user.email || '',
          };
          populateForm(defaultProf);
        }
      }
    } catch (err) {
      console.error('Lỗi khi tải dữ liệu profile:', err);
    } finally {
      setLoading(false);
    }
  };

  const populateForm = (data) => {
    setFormData({
      maCanBo: data.maCanBo || '',
      fullName: data.fullName || user?.fullName || '',
      cccd: data.cccd || '',
      ngaySinh: data.ngaySinh ? data.ngaySinh.split('T')[0] : '',
      gender: data.gender ?? 0,
      phoneNumber: data.phoneNumber || '',
      coQuanPhoneNumber: data.coQuanPhoneNumber || '',
      workEmail: data.workEmail || user?.email || '',
      avatarUrl: data.avatarUrl || '',
      diaChiHienNay: data.diaChiHienNay || '',
      phongBanId: data.phongBanId || '',
      chucVuId: data.chucVuId || '',
    });
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'gender' ? parseInt(value, 10) : value,
    }));
  };

  const handleSave = async (e) => {
    e.preventDefault();
    if (!user?.id) return;

    setSaving(true);
    setMessage(null);

    const payload = {
      ...formData,
      userId: user.id,
      phongBanId: formData.phongBanId || null,
      chucVuId: formData.chucVuId || null,
      ngaySinh: formData.ngaySinh ? new Date(formData.ngaySinh).toISOString() : null,
    };

    try {
      let updated;
      if (profile) {
        updated = await profileApi.updateProfileByUserId(user.id, payload);
      } else {
        updated = await profileApi.createProfile(payload);
      }
      setProfile(updated);
      setIsModalOpen(false);
      setMessage({ type: 'success', text: 'Cập nhật hồ sơ thành công!' });
    } catch (err) {
      console.error('Lỗi khi lưu profile:', err);
      setMessage({ type: 'error', text: err.message || 'Lỗi khi cập nhật hồ sơ.' });
    } finally {
      setSaving(false);
    }
  };

  const phongBanOptions = phongBans.map((pb) => ({
    value: pb.id,
    label: pb.tenPhongBan,
  }));

  const chucVuOptions = chucVus.map((cv) => ({
    value: cv.id,
    label: cv.tenChucVu,
  }));

  const getGenderText = (val) => {
    const match = GENDER_OPTIONS.find((g) => g.value === val);
    return match ? match.label : 'Chưa xác định';
  };

  if (loading) {
    return (
      <div style={{ padding: '60px', textAlign: 'center', color: 'var(--text-secondary)' }}>
        <div className="btn-spinner" style={{ margin: '0 auto 12px', width: '32px', height: '32px' }} />
        <p>Đang tải Hồ sơ Cán bộ / Nhân sự...</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '32px 16px', maxWidth: '1000px', margin: '0 auto' }}>
      {/* Toast Notification */}
      {message && (
        <div
          style={{
            padding: '12px 20px',
            borderRadius: 'var(--radius-md)',
            marginBottom: '20px',
            backgroundColor: message.type === 'success' ? 'var(--success-light)' : 'var(--error-light)',
            color: message.type === 'success' ? 'var(--success-color)' : 'var(--error-color)',
            fontWeight: 600,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
          }}
        >
          <span>{message.text}</span>
          <button
            onClick={() => setMessage(null)}
            style={{ background: 'none', color: 'inherit', fontWeight: 700 }}
          >
            ✕
          </button>
        </div>
      )}

      {/* Hero Header Card */}
      <Card className="mb-6" style={{ marginBottom: '24px' }}>
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            flexWrap: 'wrap',
            gap: '20px',
          }}
        >
          <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
            {/* Avatar Circle */}
            <div
              style={{
                width: '80px',
                height: '80px',
                borderRadius: '50%',
                background: 'linear-gradient(135deg, var(--accent-color), var(--accent-hover))',
                color: '#fff',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: '2rem',
                fontWeight: 700,
                boxShadow: '0 4px 14px var(--accent-light)',
                overflow: 'hidden',
              }}
            >
              {profile?.avatarUrl ? (
                <img src={profile.avatarUrl} alt="Avatar" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
              ) : (
                (profile?.fullName || user?.username || 'U').charAt(0).toUpperCase()
              )}
            </div>

            <div>
              <h2 style={{ fontSize: '1.75rem', fontWeight: 700, marginBottom: '6px' }}>
                {profile?.fullName || user?.fullName || 'Cán bộ Nhân sự'}
              </h2>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px', flexWrap: 'wrap' }}>
                {profile?.maCanBo && <Badge variant="secondary">Mã CB: {profile.maCanBo}</Badge>}
                {profile?.tenPhongBan && <Badge variant="primary">{profile.tenPhongBan}</Badge>}
                {profile?.tenChucVu && <Badge variant="success">{profile.tenChucVu}</Badge>}
              </div>
            </div>
          </div>

          <Button variant="primary" onClick={() => setIsModalOpen(true)}>
            ✏️ Chỉnh sửa Hồ sơ
          </Button>
        </div>
      </Card>

      {/* Navigation Tabs */}
      <div
        style={{
          display: 'flex',
          gap: '12px',
          marginBottom: '20px',
          borderBottom: '1px solid var(--border-color)',
          paddingBottom: '8px',
        }}
      >
        <button
          className={`btn ${activeTab === 'identity' ? 'btn-primary' : 'btn-ghost'}`}
          onClick={() => setActiveTab('identity')}
        >
          🪪 Định danh Cán bộ
        </button>
        <button
          className={`btn ${activeTab === 'contact' ? 'btn-primary' : 'btn-ghost'}`}
          onClick={() => setActiveTab('contact')}
        >
          📞 Liên hệ & Công vụ
        </button>
        <button
          className={`btn ${activeTab === 'organization' ? 'btn-primary' : 'btn-ghost'}`}
          onClick={() => setActiveTab('organization')}
        >
          🏢 Phòng ban & Chức vụ
        </button>
      </div>

      {/* Tab 1: Định danh Cán bộ */}
      {activeTab === 'identity' && (
        <Card title="Định danh Cá nhân / Cán bộ">
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '20px' }}>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Mã Cán bộ / Mã NV</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.maCanBo || '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Họ và tên</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.fullName || user?.fullName || '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Căn cước công dân (CCCD)</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.cccd || '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Ngày sinh</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.ngaySinh ? formatDate(profile.ngaySinh) : '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Giới tính</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {getGenderText(profile?.gender)}
              </p>
            </div>
          </div>
        </Card>
      )}

      {/* Tab 2: Liên hệ & Công vụ */}
      {activeTab === 'contact' && (
        <Card title="Thông tin Liên hệ Công vụ & Cá nhân">
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '20px' }}>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Email Công vụ (.gov.vn)</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px', color: 'var(--accent-color)' }}>
                {profile?.workEmail || user?.email || '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Số điện thoại Di động</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.phoneNumber || '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>SĐT Cơ quan / Máy lẻ</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.coQuanPhoneNumber || '---'}
              </p>
            </div>
            <div>
              <label style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Nơi ở hiện tại</label>
              <p style={{ fontSize: '1.05rem', fontWeight: 600, marginTop: '4px' }}>
                {profile?.diaChiHienNay || '---'}
              </p>
            </div>
          </div>
        </Card>
      )}

      {/* Tab 3: Sơ đồ Phòng ban & Chức vụ */}
      {activeTab === 'organization' && (
        <Card title="Tổ chức, Sơ đồ Phòng ban & Chức vụ">
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: '20px' }}>
            <div
              style={{
                padding: '16px',
                border: '1px solid var(--border-color)',
                borderRadius: 'var(--radius-md)',
                backgroundColor: 'var(--bg-input)',
              }}
            >
              <span style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>🏢 PHÒNG / BAN CÔNG TÁC</span>
              <h4 style={{ fontSize: '1.2rem', fontWeight: 700, marginTop: '8px' }}>
                {profile?.tenPhongBan || 'Chưa phân công Phòng/Ban'}
              </h4>
            </div>

            <div
              style={{
                padding: '16px',
                border: '1px solid var(--border-color)',
                borderRadius: 'var(--radius-md)',
                backgroundColor: 'var(--bg-input)',
              }}
            >
              <span style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>🎖️ CHỨC VỤ ĐẢM NHIỆM</span>
              <h4 style={{ fontSize: '1.2rem', fontWeight: 700, marginTop: '8px' }}>
                {profile?.tenChucVu || 'Chưa bổ nhiệm Chức vụ'}
              </h4>
            </div>
          </div>
        </Card>
      )}

      {/* Modal Chỉnh sửa Hồ sơ */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Cập nhật Hồ sơ Cán bộ / Nhân sự"
        maxWidth="650px"
        footer={
          <>
            <Button variant="secondary" onClick={() => setIsModalOpen(false)}>
              Hủy
            </Button>
            <Button variant="primary" onClick={handleSave} isLoading={saving}>
              Lưu thay đổi
            </Button>
          </>
        }
      >
        <form onSubmit={handleSave} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <h4 style={{ fontSize: '0.95rem', fontWeight: 700, color: 'var(--accent-color)', borderBottom: '1px solid var(--border-color)', paddingBottom: '6px' }}>
            1. Định danh Cán bộ
          </h4>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <Input
              label="Mã Cán bộ"
              name="maCanBo"
              value={formData.maCanBo}
              onChange={handleInputChange}
              placeholder="VD: CB-1029"
            />
            <Input
              label="Họ và tên"
              name="fullName"
              required
              value={formData.fullName}
              onChange={handleInputChange}
            />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '12px' }}>
            <Input
              label="Căn cước công dân"
              name="cccd"
              value={formData.cccd}
              onChange={handleInputChange}
            />
            <Input
              label="Ngày sinh"
              type="date"
              name="ngaySinh"
              value={formData.ngaySinh}
              onChange={handleInputChange}
            />
            <Select
              label="Giới tính"
              name="gender"
              options={GENDER_OPTIONS}
              value={formData.gender}
              onChange={handleInputChange}
            />
          </div>

          <h4 style={{ fontSize: '0.95rem', fontWeight: 700, color: 'var(--accent-color)', borderBottom: '1px solid var(--border-color)', paddingBottom: '6px', marginTop: '12px' }}>
            2. Thông tin Liên hệ
          </h4>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <Input
              label="Email Công vụ (.gov.vn)"
              name="workEmail"
              value={formData.workEmail}
              onChange={handleInputChange}
            />
            <Input
              label="SĐT Di động"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleInputChange}
            />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <Input
              label="SĐT Cơ quan / Máy lẻ"
              name="coQuanPhoneNumber"
              value={formData.coQuanPhoneNumber}
              onChange={handleInputChange}
            />
            <Input
              label="URL Ảnh đại diện"
              name="avatarUrl"
              value={formData.avatarUrl}
              onChange={handleInputChange}
              placeholder="https://example.com/avatar.jpg"
            />
          </div>

          <Input
            label="Nơi ở hiện tại"
            name="diaChiHienNay"
            value={formData.diaChiHienNay}
            onChange={handleInputChange}
          />

          <h4 style={{ fontSize: '0.95rem', fontWeight: 700, color: 'var(--accent-color)', borderBottom: '1px solid var(--border-color)', paddingBottom: '6px', marginTop: '12px' }}>
            3. Phòng ban & Chức vụ
          </h4>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
            <Select
              label="Phòng / Ban Công tác"
              name="phongBanId"
              options={phongBanOptions}
              value={formData.phongBanId}
              onChange={handleInputChange}
              placeholder="-- Chọn Phòng Ban --"
            />
            <Select
              label="Chức vụ Đảm nhiệm"
              name="chucVuId"
              options={chucVuOptions}
              value={formData.chucVuId}
              onChange={handleInputChange}
              placeholder="-- Chọn Chức Vụ --"
            />
          </div>
        </form>
      </Modal>
    </div>
  );
};
