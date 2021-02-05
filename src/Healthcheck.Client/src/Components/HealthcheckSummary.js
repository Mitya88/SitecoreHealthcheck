import React from 'react';
import ScIcon from '../ScComponents/ScIcon'

class HealthcheckSummary extends React.Component {

  render() {
    return (
      <div className="card  mb-3 state-block" >
        <div className="card-block parent" style={{ backgroundColor: this.props.color }}>
          <div className="row state-row ">
            <div className="col-5 margin-top-15 center">
              <ScIcon icon={this.props.icon} title="memory_stick" size="xxl" />
            </div>
            <div className="col-7 padding-top-5" style={{ backgroundColor: this.props.secondColor }} > <h2>{this.props.data}</h2>
              <p>{this.props.text}</p></div>
          </div>
        </div>
      </div>
    );
  }
}

export default HealthcheckSummary;
