import React from 'react';

class ScIcon extends React.Component {

    getClass = () => {
        var Prefix = "si";
        return Prefix + " " + Prefix + "-" + this.props.icon + " " + Prefix + "-" + this.props.size;
    }

    render() {
        return (
            <i aria-hidden="true" className={this.getClass()} title={this.props.title} style={{ color: this.props.color }}></i>
        );
    }
}

export default ScIcon;
