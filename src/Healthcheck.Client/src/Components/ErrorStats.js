import React from 'react';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class ErrorStats extends React.Component {

  state = {
    errors: [],
    isLoading: true
  }

  componentDidMount() {
    fetch('/sitecore/api/ssc/healthcheck/lasterrors?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ errors: data, isLoading: false });
      });
  }

  render() {
    return (
      <>
        <div className="col-sm-8">
          <div className="card mb-3" >
            <h5 className="center">Errors in the last 24h</h5>
            <div className="card-block main-menu-item allow-scroll">
              {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
                <table className="table table-striped table-hover">
                  <thead>
                    <tr>
                      <th>Component Name</th>
                      <th>Message</th>
                      <th>Date</th>
                    </tr>
                  </thead>
                  <tbody>

                    {this.state.errors.map((error, index) =>
                      <tr key={index}>
                        <td>{error.ComponentName}</td>
                        <td>{error.Message}</td>
                        <td>{error.Date}</td>
                      </tr>
                    )}

                  </tbody>
                </table>
              }
            </div>
          </div>
        </div>
      </>
    );
  }
}

export default ErrorStats;
