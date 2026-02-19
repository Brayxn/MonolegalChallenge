import React from 'react';

interface TabsPanelProps {
  tabs: { label: string; icon?: React.ReactNode; content: React.ReactNode }[];
  value: number;
  onChange: (idx: number) => void;
}

const TabsPanel: React.FC<TabsPanelProps> = ({ tabs, value, onChange }) => (
  <div>
    <div className="tabs">
      {tabs.map((tab, idx) => (
        <button
          key={tab.label}
          className={`tab${value === idx ? ' active' : ''}`}
          onClick={() => onChange(idx)}
        >
          {tab.icon && <span style={{marginRight:8}}>{tab.icon}</span>}{tab.label}
        </button>
      ))}
    </div>
    {tabs.map((tab, idx) => (
      <div key={tab.label} className={`tab-content${value === idx ? ' active' : ''}`}>{tab.content}</div>
    ))}
  </div>
);

export default TabsPanel;
