import React from 'react';
import ScIcon from './ScIcon'

class ScMenuCategory extends React.Component {

  state = {
    isOpened: false
  }

  componentDidMount() {
    this.setState({ isOpened: this.props.isOpened });
  }

  changeState = () => {
    this.setState({ isOpened: !this.state.isOpened });
  }

  render() {
    return (
      <>
        <button className="menu-category" type="button" onClick={this.changeState}>
          <ScIcon icon={this.props.icon} />
          <span>{this.props.categoryTitle}</span>

          {this.state.isOpened === false && <ScIcon icon="navigate_down" />}
          {this.state.isOpened === true && <ScIcon icon="navigate_up" />}

        </button>
        <ul className={this.state.isOpened ? "show" : ""}>
          {this.props.children}
        </ul>
      </>
    );
  }
}

export default ScMenuCategory;
