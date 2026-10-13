import React, { useEffect, useMemo, useState } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import {
  AlertTriangle,
  BarChart3,
  Bell,
  Bookmark,
  BookCopy,
  BookMarked,
  BookOpen,
  Bot,
  ClipboardList,
  Clock,
  Compass,
  Construction,
  ChevronDown,
  ChevronLeft,
  ChevronRight,
  Feather,
  FileBarChart,
  FolderOpen,
  HardDrive,
  Inbox,
  Library,
  Lightbulb,
  LineChart,
  Mail,
  MapPin,
  MessageSquare,
  MessagesSquare,
  PackagePlus,
  PieChart,
  Radio,
  Repeat,
  Search,
  Shield,
  ShoppingCart,
  Star,
  Tags,
  Truck,
  Undo2,
  User,
} from 'lucide-react';
import { NAV_SECTIONS } from '@/config/navigation';
import { useAuth } from '@/hooks/useAuth';
import { useToast } from '@/context/ToastContext';

const ICONS = {
  AlertTriangle,
  BarChart3,
  Bell,
  Bookmark,
  BookCopy,
  BookMarked,
  BookOpen,
  Bot,
  ClipboardList,
  Clock,
  Compass,
  Feather,
  FileBarChart,
  FolderOpen,
  HardDrive,
  Inbox,
  Library,
  Lightbulb,
  LineChart,
  Mail,
  MapPin,
  MessageSquare,
  MessagesSquare,
  PackagePlus,
  PieChart,
  Radio,
  Repeat,
  Search,
  Shield,
  ShoppingCart,
  Star,
  Tags,
  Truck,
  Undo2,
  User,
};

const collectOpenIds = (items, pathname, acc = new Set()) => {
  items.forEach((item) => {
    if (!item.children?.length) return;
    const childActive = item.children.some(
      (child) =>
        (child.path && pathname.startsWith(child.path)) ||
        (child.children && hasActiveDescendant(child, pathname))
    );
    if (childActive || item.defaultOpen) {
      acc.add(item.id);
    }
    collectOpenIds(item.children, pathname, acc);
  });
  return acc;
};

const hasActiveDescendant = (item, pathname) => {
  if (item.path && pathname.startsWith(item.path)) return true;
  return (item.children || []).some((child) => hasActiveDescendant(child, pathname));
};

const firstRealPath = (item) => {
  if (item.path && !item.comingSoon) return item.path;
  for (const child of item.children || []) {
    const found = firstRealPath(child);
    if (found) return found;
  }
  return null;
};

export const Sidebar = ({ collapsed, onToggle }) => {
  const { user } = useAuth();
  const { showComingSoon } = useToast();
  const location = useLocation();
  const [mobileOpen, setMobileOpen] = useState(false);

  const initialOpen = useMemo(
    () => collectOpenIds(NAV_SECTIONS.flatMap((s) => s.items), location.pathname),
    // chỉ seed lần đầu theo route; user tự đóng/mở sau đó
    // eslint-disable-next-line react-hooks/exhaustive-deps
    []
  );

  const [openIds, setOpenIds] = useState(initialOpen);

  useEffect(() => {
    setOpenIds((prev) => {
      const next = new Set(prev);
      collectOpenIds(NAV_SECTIONS.flatMap((s) => s.items), location.pathname, next);
      return next;
    });
  }, [location.pathname]);

  const canSee = (item) => {
    if (item.permission && !user?.permissions?.includes(item.permission)) return false;
    if (item.children?.length) {
      return item.children.some(canSee);
    }
    return true;
  };

  const toggleBranch = (id) => {
    setOpenIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleComingSoon = (item) => {
    showComingSoon(item.label);
    setMobileOpen(false);
  };

  const handleCollapsedParentClick = (item) => {
    const path = firstRealPath(item);
    if (path) return;
    handleComingSoon(item);
  };

  const renderNode = (item, depth) => {
    if (!canSee(item)) return null;

    const Icon = ICONS[item.icon] || User;
    const hasChildren = (item.children || []).some(canSee);
    const isOpen = openIds.has(item.id);
    const isBranchActive = hasActiveDescendant(item, location.pathname);

    if (hasChildren && !collapsed) {
      return (
        <div key={item.id} className="sidebar-tree-node">
          <button
            type="button"
            className={`sidebar-item sidebar-branch${isBranchActive ? ' sidebar-branch-active' : ''}`}
            style={{ paddingLeft: 12 + depth * 12 }}
            onClick={() => toggleBranch(item.id)}
            aria-expanded={isOpen}
          >
            <Icon size={18} strokeWidth={2} />
            <span className="sidebar-item-label">{item.label}</span>
            <ChevronDown
              size={14}
              className={`sidebar-tree-chevron${isOpen ? ' is-open' : ''}`}
            />
          </button>
          {isOpen && (
            <div className="sidebar-tree-children">
              {item.children.map((child) => renderNode(child, depth + 1))}
            </div>
          )}
        </div>
      );
    }

    if (hasChildren && collapsed) {
      const path = firstRealPath(item);
      if (path) {
        return (
          <NavLink
            key={item.id}
            to={path}
            className={({ isActive }) =>
              `sidebar-item${isActive || isBranchActive ? ' sidebar-item-active' : ''}`
            }
            title={item.label}
            onClick={() => setMobileOpen(false)}
          >
            <Icon size={18} strokeWidth={2} />
          </NavLink>
        );
      }

      return (
        <button
          key={item.id}
          type="button"
          className="sidebar-item sidebar-item-soon"
          title={`${item.label} (đang phát triển)`}
          onClick={() => handleCollapsedParentClick(item)}
        >
          <Icon size={18} strokeWidth={2} />
        </button>
      );
    }

    if (item.comingSoon || !item.path) {
      return (
        <button
          key={item.id}
          type="button"
          className="sidebar-item sidebar-item-soon"
          style={collapsed ? undefined : { paddingLeft: 12 + depth * 12 }}
          onClick={() => handleComingSoon(item)}
          title={collapsed ? `${item.label} (đang phát triển)` : undefined}
        >
          <Icon size={depth > 0 ? 16 : 18} strokeWidth={2} />
          {!collapsed && (
            <>
              <span className="sidebar-item-label">{item.label}</span>
              <Construction size={13} className="sidebar-soon-icon" />
            </>
          )}
        </button>
      );
    }

    return (
      <NavLink
        key={item.id}
        to={item.path}
        className={({ isActive }) =>
          `sidebar-item${isActive ? ' sidebar-item-active' : ''}`
        }
        style={collapsed ? undefined : { paddingLeft: 12 + depth * 12 }}
        title={collapsed ? item.label : undefined}
        onClick={() => setMobileOpen(false)}
      >
        <Icon size={depth > 0 ? 16 : 18} strokeWidth={2} />
        {!collapsed && <span className="sidebar-item-label">{item.label}</span>}
      </NavLink>
    );
  };

  return (
    <>
      <button
        type="button"
        className="sidebar-mobile-toggle"
        onClick={() => setMobileOpen((v) => !v)}
        aria-label="Mở menu"
      >
        ☰
      </button>

      {mobileOpen && (
        <div
          className="sidebar-backdrop"
          onClick={() => setMobileOpen(false)}
          aria-hidden
        />
      )}

      <aside
        className={`sidebar${collapsed ? ' sidebar-collapsed' : ''}${
          mobileOpen ? ' sidebar-mobile-open' : ''
        }`}
      >
        <div className="sidebar-top">
          {!collapsed && <span className="sidebar-brand-sub">Thư viện số</span>}
          <button
            type="button"
            className="sidebar-collapse-btn"
            onClick={onToggle}
            title={collapsed ? 'Mở rộng' : 'Thu gọn'}
            aria-label={collapsed ? 'Mở rộng sidebar' : 'Thu gọn sidebar'}
          >
            {collapsed ? <ChevronRight size={16} /> : <ChevronLeft size={16} />}
          </button>
        </div>

        <nav className="sidebar-nav">
          {NAV_SECTIONS.map((section) => {
            const visibleItems = section.items.filter(canSee);
            if (visibleItems.length === 0) return null;

            return (
              <div key={section.title} className="sidebar-section">
                {!collapsed && (
                  <div className="sidebar-section-title">{section.title}</div>
                )}
                <div className="sidebar-section-items">
                  {section.items.map((item) => renderNode(item, 0))}
                </div>
              </div>
            );
          })}
        </nav>
      </aside>
    </>
  );
};
