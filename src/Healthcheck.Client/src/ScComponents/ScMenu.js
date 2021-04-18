import React from 'react';

class ScMenu extends React.Component {
  render() {
    return (
      <nav>
        <nav className="menu">
          {this.props.children}
        </nav>
      </nav>
    );
  }
}

export default ScMenu;
