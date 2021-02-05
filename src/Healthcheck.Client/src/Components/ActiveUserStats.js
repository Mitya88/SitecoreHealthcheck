import React from 'react';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class ActiveUserStats extends React.Component {

  state = {
    activeUsers: [],
    isLoading: true
  }

  componentDidMount() {
    fetch('/sitecore/api/ssc/healthcheck/getactiveusers?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ activeUsers: data, isLoading: false });
      });
  }

  render() {
    return (
      <>
        <div className="col-sm-4">
          <div className="card mb-3" >
            <h5 className="center">Active Users</h5>
            <div className="card-block main-menu-item allow-scroll">
              {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
                <table className="table table-striped table-hover">
                  <thead>
                    <tr>
                      <th>User</th>
                      <th>Login</th>
                      <th>Last Request</th>
                    </tr>
                  </thead>
                  <tbody>

                    {this.state.activeUsers.map((user, index) =>
                      <tr key={index}>
                        <td>{user.UserName}</td>
                        <td>{user.Login}</td>
                        <td>{user.LastRequest}</td>
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

export default ActiveUserStats;
