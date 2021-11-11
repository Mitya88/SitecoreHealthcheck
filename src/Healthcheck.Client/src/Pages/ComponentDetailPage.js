import React from 'react';
import { useParams } from "react-router-dom";
import { useLocation, useNavigate } from "react-router-dom";
import TableView from '../Components/TableView';
import ScApplicationHeader from '../ScComponents/ScApplicationHeader';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';
import { withRouter } from "react-router";
import _ from 'lodash';

class ComponentDetailPage extends React.Component {
 

  state = {
    isLoading: false,
    groups: [],
    stateFilter: 'All',
    groupName:''
  }

  componentDidMount() {
    console.log(this.props.match);
    this.load();
  }

  load = () => {
    this.setState({ isLoading: true });

    fetch('/api/sitecore/api/ssc/healthcheck/get?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        var selectedGroup = _.find(data, {GroupId:this.props.match.params.id})
        this.setState({ groups: [selectedGroup], isLoading: false, groupName:selectedGroup.GroupName });
      });
  }

  refresh = component => () => {

    var onlyState = component.Status === "Waiting" ? true : false;
    this.setState({ isLoading: true });

    fetch('/api/sitecore/api/ssc/healthcheck/component?id=' + component.Id + '&sc_site=shell&onlystate=' + onlyState)
      .then(data => data.json())
      .then(data => {

        var group = _.find(this.state.groups, { Components: [{ Id: component.Id }] });

        var index = _.findIndex(group.Components, { Id: component.Id });

        group.Components.splice(index, 1, data);
        
        console.log(group);

        this.setState({ isLoading: false });
      });
  }

  render() {
    return (
      <main className="page-main">
        <ScApplicationHeader title="Advanced Sitecore Healthcheck 3.0" subTitle={this.state.groupName} />
        <div className="page-content-section">
          <div className="page-content">
            <div className="page-action-bar" >              
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

export default withRouter(ComponentDetailPage);
