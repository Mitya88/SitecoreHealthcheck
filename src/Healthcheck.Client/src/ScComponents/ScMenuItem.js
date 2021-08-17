import React from 'react';
import { NavLink } from 'react-router-dom'

class ScMenuItem extends React.Component {

  state = {
    isOpened: false
  }

  changeState = () => {
    this.setState({ isOpened: !this.state.isOpened })
  }

  render() {
    return (
      <>
        <li className="menu-item">
          <NavLink to={this.props.href} key={this.props.menukey}  activeClassName="active">
            {this.props.children}
          </NavLink>
        </li>
      </>
    );
  }
}

export default ScMenuItem;
