import React from 'react';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class UpcomingExpirations extends React.Component {

  state = {
    expirations: [],
    isLoading: true
  }

  componentDidMount() {
    fetch('/api/sitecore/api/ssc/healthcheck/upcomingexpirations?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ expirations: data, isLoading: false });
      });
  }

  render() {
    return (
      <>
        <div className="col-sm-4">
          <div className="card mb-3" >
            <h5 className="center">Upcoming expirations</h5>
            <div className="card-block main-menu-item allow-scroll">
              {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
                <table className="table table-striped table-hover">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Type</th>
                      <th>Expiration</th>
                    </tr>
                  </thead>
                  <tbody>

                    {this.state.expirations.map((expiration, index) =>
                      <tr key={index}>
                        <td>{expiration.Name}</td>
                        <td>{expiration.Type}</td>
                        <td>{expiration.Date}</td>
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

export default UpcomingExpirations;
