import React from 'react';
import ScIcon from '../ScComponents/ScIcon'
import { Link } from 'react-router-dom'
import Moment from 'react-moment'
import ErrorToolTip from './ErrorToolTip';

class ComponentState extends React.Component {

  getClass(component) {
    if (component.Status === "Healthy") {
      return "green";
    }
    else if (component.Status === "Warning") {
      return "orange";
    }
    else if (component.Status === "Error") {
      return "red";
    }
    else {
      return "black"
    }
  }

  getStateClass(component) {
    if (component.Status === "Healthy") {
      return "healthy-state";
    }
    else if (component.Status === "Warning") {
      return "warning-state";
    }
    else if (component.Status === "Error") {
      return "error-state";
    }
    else {
      return "unknown-state";
    }
  }

  render() {
    let healthyMessage;
    if (this.props.component.Status === 'Healthy') {
      healthyMessage = <p className="healthy-message" title={this.props.component.HealthyMessage}>{this.props.component.HealthyMessage}</p>
    }
    else {
      healthyMessage = <p className="healthy-message">&nbsp;</p>
    }

    let componentClass = 'col-md-2 flex-correction';

    if (this.props.component.Display === false) {
      componentClass += " hide";
    }

    let exportLink = '/sitecore/api/ssc/healthcheck/csv?id=' + this.props.component.Id + '&componentName=' + this.props.component.Name + '&sc_site=shell'
    return (
      <div className={componentClass} onDoubleClick={this.props.Refresh}>
        <div className="card mb-3">
          <div className="card-block main-menu-item no-padding" >
            <div className={this.getStateClass(this.props.component)}>
              <span className="component-title">{this.props.component.Name}</span>
              <span className="error-count">{this.props.component.ErrorCount}</span>
            </div>
            <div className="p-4">
              <div className="health-icon center" >
                {this.props.component.Status === "Healthy" ? <ScIcon color="green" icon="navigate_check" size="xxxl" /> : ""}
                {this.props.component.Status === "Warning" ? <ScIcon color="orange" icon="about" size="xxxl" /> : ""}
                {this.props.component.Status === "Error" ? <ScIcon color="red" icon="heart_broken" size="xxxl" /> : ""}
                {this.props.component.Status === "Waiting" ? <ScIcon color="black" icon="alarmclock" size="xxxl" /> : ""}
              </div>
              <p><Link to={`/detail/${this.props.component.Id}`} >Detail</Link></p>
              <p>
                <Moment
                  date={this.props.component.LastCheckTime}
                  parse="YYYY-MM-DDTHH:mm:s"
                  format="MMM-D HH:mm" />
              </p>

              {healthyMessage}

              <div>
                <Link to={exportLink}>Download logs</Link>
              </div>
              <div className="tooltip2">Issues
                <ErrorToolTip component={this.props.component} exportLink={exportLink} />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default ComponentState;
