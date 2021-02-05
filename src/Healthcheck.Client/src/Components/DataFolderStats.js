import React from 'react';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class DataFolderStats extends React.Component {

  state = {
    dataFolderStats: [],
    isLoading: true
  }

  componentDidMount() {
    fetch('/sitecore/api/ssc/healthcheck/Getdatafolderstatistics?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ dataFolderStats: data, isLoading: false });
      });
  }

  render() {
    return (
      <>
        <div className="col-sm-3">
          <div className="card mb-3">
            <h5 className="center">Data Folder disk usage</h5>
            <div className="card-block main-menu-item allow-scroll">
              {this.state.isLoading === true ? <ScProgressIndicatior show={true} /> :
                <table className="table table-striped table-hover">
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Size</th>
                    </tr>
                  </thead>
                  <tbody>

                    {this.state.dataFolderStats.map((folder, index) =>
                      <tr key={index}>
                        <td>{folder.Name}</td>
                        <td>{folder.Size}</td>
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

export default DataFolderStats;
