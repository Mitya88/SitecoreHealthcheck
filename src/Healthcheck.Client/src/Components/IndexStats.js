import React from 'react';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class IndexStats extends React.Component {

  state = {
    indexStats: [],
    isLoading: true
  }

  componentDidMount() {
    fetch('/api/sitecore/api/ssc/healthcheck/GetIndexDetails?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ indexStats: data, isLoading: false });
      });
  }

  render() {
    return (
      <>
        <div className="col-sm-3">
          <div className="card mb-3">
            <h5 className="center">Index Statistics</h5>
            <div className="card-block main-menu-item allow-scroll">
              {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
                <table className="table table-striped table-hover">
                  <thead>
                    <tr>
                      <th>Index Name</th>
                      <th>Document Count</th>
                    </tr>
                  </thead>
                  <tbody>

                    {this.state.indexStats.map((index) =>
                      <tr key={index.IndexName}>
                        <td>{index.IndexName}</td>
                        <td>{index.IndexCount}</td>
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

export default IndexStats;
