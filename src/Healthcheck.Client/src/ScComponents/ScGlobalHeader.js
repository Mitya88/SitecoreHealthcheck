import React from 'react';
import ScGlobalLogo from './ScGlobalLogo';
import ScIcon from './ScIcon';

class ScGlobalHeader extends React.Component {
  render() {
    return (
      <header className="global-header">
        <div className="gh-content">
          <ScGlobalLogo />
          <div className="gh-app">{this.props.applicationTitle}</div>
          <div className="gh-account"></div>
          <a href="https://docs.advancedschealthcheck.com/" target="_blank" rel="noreferrer">
                    <ScIcon icon="question" size="small" color="white"/>
          </a>
        </div>
      </header>
    );
  }
}

export default ScGlobalHeader;
