import React from 'react';

class ScProgressIndicatior extends React.Component {

    getClass() {
        var cssClass = "progress-indicator-panel margin-top-40";
        return this.props.show ? cssClass + " show" : "";
    }

    render() {
        return (
            <div className={this.getClass()} >
                <div className="pip-overlay">
                    <div className="pip-loader">
                        <div className="pip-square-1"></div>
                        <div className="pip-square-2"></div>
                        <div className="pip-square-3"></div>
                        <div className="pip-square-4"></div>
                        <div className="pip-square-5"></div>
                        <div className="pip-square-6"></div>
                        <div className="pip-square-7"></div>
                        <div className="pip-square-8"></div>
                        <div className="pip-square-9"></div>
                    </div>
                </div>
                <div className="pip-background"></div>
            </div >
        );
    }
}

export default ScProgressIndicatior;
