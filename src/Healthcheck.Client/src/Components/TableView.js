import React from 'react';
import ScIcon from '../ScComponents/ScIcon'
class TableView extends React.Component {

  healthyMessage(component) {
    let healthyMessage;
    if (component.Status === 'Healthy') {
      healthyMessage = <p className="margin-center" title={component.HealthyMessage}>{component.HealthyMessage}</p>
    }
    else {
      healthyMessage = <p className="margin-center">&nbsp;</p>
    }

    return healthyMessage;
  }
  render() {
    return (
      <>

        <div className="row">
          <table className="table table-striped table-hover components">
            <thead>
              <tr>
                <th className="width-1">#</th>
                <th>Group</th>
                <th>Component Name</th>
                <th className="width-1">Error Count</th>
                <th>Message</th>
                <th>#</th>
              </tr>
            </thead>
            <tbody>
              {this.props.groups.map(group =>
                group.Components.map(component =>
                  <tr key={component.Id} >
                    <td className="center">
                      {component.Status === "Healthy" ? <ScIcon color="green" icon="navigate_check" size="medium" /> : ""}
                      {component.Status === "Warning" ? <ScIcon color="orange" icon="about" size="medium" /> : ""}
                      {component.Status === "Error" ? <ScIcon color="red" icon="heart_broken" size="medium" /> : ""}
                      {component.Status === "Waiting" ? <ScIcon color="black" icon="alarmclock" size="medium" /> : ""}

                    </td>
                    <td>{group.GroupName}</td>
                    <td>{component.Name}</td>

                    <td >
                      <div className="tooltip3">
                        <p className="red bold center margin-center">{component.ErrorCount}</p>
                        {/* <div class="tooltiptext2" *ngIf="component.ErrorCount > 0">
                    <h3>Most recent issues</h3>
                    <h4><a href="/sitecore/api/ssc/healthcheck/csv?id={{component.Id}}&componentName={{component.Name}}">Download
                      All</a></h4>
                    <div class="row padding-left-15 margin-bottom-15" *ngFor='let error of component.ErrorList.Entries'>
                      <div class="col-8" title="{{error.Reason}}" *ngIf="error.Reason.length>100">
                        {{error.Reason.substr(0, 100) + '\u2026'}}
                      </div>
                      <div class="col-8" *ngIf="error.Reason.length<=100">
                        {{error.Reason}}
                      </div>
                      <div class="col-4">
                        {{error.Created  | date:'medium' }}
                      </div>
                    </div>
    
                  </div> */}

                      </div></td>
                    <td>  {this.healthyMessage(component)}

                      {/* <p class="red bold margin-center" title="{{component.ErrorList.Entries[0].Reason}}" ">
                      {{component.ErrorList.Entries[0].Reason.substr(0, 160) + '\u2026'}}
                    </p>
                    <p class="red bold margin-center" *ngIf="component.Status!='Healthy' && component.ErrorList && component.ErrorList.Entries.length > 0 && component.ErrorList.Entries[0].Reason && component.ErrorList.Entries[0].Reason.length<=160">
                      {{component.ErrorList.Entries[0].Reason}}
                    </p> */}
                    </td>

                    <td> <a href="/sitecore/api/ssc/healthcheck/csv?id={{component.Id}}&componentName={{component.Name}}">Download
                logs</a></td>
                  </tr>
                )

              )}

            </tbody>
          </table>
        </div>


      </>
    );
  }
}

export default TableView;
