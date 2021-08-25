import React, { useEffect, useState } from 'react';
import TableView from '../Components/TableView';
import ScApplicationHeader from '../ScComponents/ScApplicationHeader';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

import { useParams } from "react-router-dom";
import _ from 'lodash';

const ComponentDetailPage = () => {
  let { id } = useParams();
  const [isLoading, setLoading] = useState(false);
  const [groups, setGroups] = useState([]);
  const [groupName, setGroupName] = useState('');

  useEffect(() => {
    load();
  }, [id]);

  function load() {
    setLoading(true);

    fetch('/api/sitecore/api/ssc/healthcheck/get?sc_site=shell')
      .then(data => data.json())
      .then(data => {
        var selectedGroup = _.find(data, { GroupId: id })

        setLoading(false);
        setGroups([selectedGroup]);
        setGroupName(selectedGroup.GroupName);
      });
  };

  function refresh(component) {

    // var onlyState = component.Status === "Waiting" ? true : false;
    // setLoading(true);

    // fetch('/api/sitecore/api/ssc/healthcheck/component?id=' + component.Id + '&sc_site=shell&onlystate=' + onlyState)
    //   .then(data => data.json())
    //   .then(data => {

    //     var group = _.find(groups, { Components: [{ Id: component.Id }] });

    //     var index = _.findIndex(group.Components, { Id: component.Id });

    //     group.Components.splice(index, 1, data);

    //     console.log(group);

    //     setLoading(false);
    //   });
  };


  return (


    <main className="page-main">
      <ScApplicationHeader title="Advanced Sitecore Healthcheck 3.0" subTitle={groupName} />
      <div className="page-content-section">
        <div className="page-content">
          <div className="page-action-bar" >
          </div>


          <div className="p-4">
            <ScProgressIndicatior show={isLoading} />

            <TableView groups={groups} refresh={refresh} />
            {/* <TableView groups={groups} /> */}
          </div>

        </div>
      </div>
    </main >

  );
};
export default ComponentDetailPage;



