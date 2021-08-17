import ScApplicationHeader from '../ScComponents/ScApplicationHeader';
import React from 'react';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';
import ScIcon from '../ScComponents/ScIcon';
import _ from 'lodash';
import TableView from '../Components/TableView';

class ComponentsPage extends React.Component {

  filterStates = ["All", "Healthy only", "Non-Healthy only"]

  state = {
    isLoading: false,
    groups: [],
    stateFilter: 'All'
  }

  componentDidMount() {
    this.load();
  }

  refresh = component => () => {

    var onlyState = component.Status == "Waiting" ? true : false;
    this.setState({ isLoading: true });

    fetch('/sitecore/api/ssc/healthcheck/component?id=' + component.Id + '&sc_site=shell&onlystate=' + onlyState)
      .then(data => data.json())
      .then(data => {

        var group = _.find(this.state.groups, { Components: [{ Id: component.Id }] });

        var index = _.findIndex(group.Components, { Id: component.Id });

        group.Components.splice(index, 1, data);

        console.log(group);

        this.setState({ isLoading: false });
      });
  }

  changeStateFilter = e => {
    this.setState({ stateFilter: e.target.value });
    this.load();
  }

  load = () => {
    this.setState({ isLoading: true });

    fetch('/sitecore/api/ssc/healthcheck/get?sc_site=shell')
      .then(data => data.json())
      .then(data => {

        // data = data.filter((component) => component.Status
        this.setState({ groups: data, isLoading: false });
      });
  }

  run = () => {
    this.setState({ isLoading: true });

    fetch('/sitecore/api/ssc/healthcheck/run?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        this.setState({ groups: data, isLoading: false });
      });
  }

  clearErrors = () => {
    this.setState({ isLoading: true });

    fetch('/sitecore/api/ssc/healthcheck/errors/clear?sc_site=shell')
      .then(data => data.json())
      .then(data => this.setState({ groups: data, isLoading: false }))
  }

  render() {
    return (
      <main className="page-main">
        <ScApplicationHeader title="Advanced Sitecore Healthcheck 3.0" subTitle="Components" />
        <div className="page-content-section">
          <div className="page-content">
            <div className="page-action-bar" >
              <div className="action-bar">
                <div className="action-bar-left">
                  <button onClick={this.load} className="btn btn-primary">Refresh</button>
                  <button onClick={this.run} className="btn btn-secondary" >Re run</button>
                  <a className="btn btn-secondary" href="/sitecore/api/ssc/healthcheck/exportstate?sc_site=shell">Export state</a>
                  <button onClick={this.clearErrors} className="btn btn-primary" >Clean Errors</button>                
                  
                </div>
                <div className="action-bar-right">
                 
                </div>
              </div>
            </div>

            
            <div className="p-4">
              <ScProgressIndicatior show={this.state.isLoading} />
              <TableView groups={this.state.groups} refresh={this.refresh} />
            </div>

          </div>
        </div>
      </main >

    );
  }
}

export default ComponentsPage;
