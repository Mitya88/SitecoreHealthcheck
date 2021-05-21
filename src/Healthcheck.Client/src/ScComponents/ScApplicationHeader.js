import React from 'react';

class ScApplicationHeader extends React.Component {
    // Used from Sitecore SPEAK3 library
    render() {
        return (
            <div className="page-app-header">
                <header className="app-header">
                  
                    <div className="app-header-content">
                        <h3 className="">{this.props.title}</h3>
                        <h5 className="">{this.props.subTitle}</h5>
                    </div>
                </header>
            </div>
        );
    }
}

export default ScApplicationHeader;
