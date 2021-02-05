import React from 'react';
import ScGlobalLogo from './ScGlobalLogo';

class ScGlobalHeader extends React.Component {
  render() {
    return (
      <header className="global-header">
        <div className="gh-content">
          <ScGlobalLogo />
          <div className="gh-app">{this.props.applicationTitle}</div>
          <div className="gh-account"></div>
        </div>
      </header>
    );
  }
}

export default ScGlobalHeader;
