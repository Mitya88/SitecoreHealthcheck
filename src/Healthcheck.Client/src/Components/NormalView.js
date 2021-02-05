import React from 'react';
import ComponentState from '../Components/ComponentState';

class NormalView extends React.Component {

  render() {
    return (
      <>
        {this.props.groups.map(group =>
          <div key={group.GroupName}>
            <div className="row">
              <h4 className="group-name">{group.GroupName}</h4>
            </div>
            <div className="row">
              {group.Components.map(component =>
                <ComponentState key={component.Id} component={component} refresh={this.props.refresh(component)}>
                </ComponentState>
              )}
            </div>
          </div>
        )}
      </>
    );
  }
}

export default NormalView;
