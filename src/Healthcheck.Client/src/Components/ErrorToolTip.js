import React from 'react';
import { Link } from 'react-router-dom'
import Moment from 'react-moment'

class ErrorToolTip extends React.Component {
  exportLink() {
    let exportLink ='/sitecore/api/ssc/healthcheck/csv?id={id}&componentName={name}'.replace('{id}',this.props.component.Id).replace('{name}',this.props.component.Name);
   

    return exportLink;
  }
  render() {
    
    return (
      <div className="tooltiptext2" >
        <h3>Most recent issues</h3>
        <h4><Link to={'/sitecore/api/ssc/healthcheck/csv?id='+this.props.component.Id+'&componentName='+this.props.component.Name} >Download All</Link></h4>
        {this.props.component.ErrorList.Entries.map((error, index) =>
          <div key={index} className="row padding-left-15 margin-bottom-15">
            {error.Reason.length > 100 ? <div className="col-6" title={error.Reason} >
              {error.Reason.substr(0, 100) + '\u2026'}
            </div> :
              <div className="col-6" >
                {error.Reason}
              </div>}
            <div className="col-4">
              <Moment
                date={error.Created}
                parse="YYYY-MM-DDTHH:mm:s"
                format="MM-DD-YYYY HH:mm:ss" />
            </div>
          </div>
        )}
      </div>
    );
  }
}

export default ErrorToolTip;
