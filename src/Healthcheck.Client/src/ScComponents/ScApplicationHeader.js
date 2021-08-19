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
                        <h5 className="red">This is a static, preview UI of the new version of Sitecore Advanced Healthcheck. The complete module will be released at the end of Sept 2021!</h5>
<h5 className="red">This module is implemented by Mihaly Arvai (@mitya_1988)</h5>
                    </div>
                </header>
            </div>
        );
    }
}

export default ScApplicationHeader;
