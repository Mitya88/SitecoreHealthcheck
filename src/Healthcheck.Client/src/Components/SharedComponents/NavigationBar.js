import React from 'react';
import ScMenu from '../../ScComponents/ScMenu';
import ScMenuCategory from '../../ScComponents/ScMenuCategory';
import ScMenuItem from '../../ScComponents/ScMenuItem';

class NavigationBar extends React.Component {

  state = { groups: [] };

  componentDidMount() {
    this.load();
  }

  load = () => {
    fetch('/sitecore/api/ssc/healthcheck/get?sc_site=shell')
      .then(data => data.json())
      .then(data => this.setState({ groups: data }));
  }

  render() {
    return (
      <aside className="page-nav">
        <ScMenu>
          <ScMenuCategory categoryTitle="Health check" isOpened={true} icon="check">
            <ScMenuItem href="/sitecore/shell/client/Applications/healthcheck/"> Home</ScMenuItem>
          </ScMenuCategory>
          <ScMenuCategory categoryTitle="Components" isOpened={false} icon="chart_pie">
            <ScMenuItem href="/sitecore/shell/client/Applications/healthcheck/components">All</ScMenuItem>
            {this.state.groups.map(group =>
              <ScMenuItem key={group.GroupId} href={`/sitecore/shell/client/Applications/healthcheck/components/${group.GroupId}`} >{group.GroupName}
              </ScMenuItem>
            )}
          </ScMenuCategory>
          <ScMenuCategory categoryTitle="Settings" isOpened={true} icon="document_gear">
            <ScMenuItem href="/sitecore/shell/client/Applications/healthcheck/settings">Settings page</ScMenuItem>
          </ScMenuCategory>
        </ScMenu>
      </aside>
    );
  }
}

export default NavigationBar;
