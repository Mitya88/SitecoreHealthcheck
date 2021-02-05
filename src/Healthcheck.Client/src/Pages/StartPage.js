import ScApplicationHeader from '../ScComponents/ScApplicationHeader';
import React from 'react';
import CpuChart from '../Components/CpuChart';
import ComponentChart from '../Components/ComponentChart';
import MemoryChart from '../Components/MemoryChart';
import ErrorChart from '../Components/ErrorChart';
import CacheChart from '../Components/CacheChart';
import DataFolderStats from '../Components/DataFolderStats';
import IndexStats from '../Components/IndexStats';
import ActiveUserStats from '../Components/ActiveUserStats';
import UpcomingExpirations from '../Components/UpcomingExpirations';
import ErrorStats from '../Components/ErrorStats';
import DriveInfoChart from '../Components/DriveInfoChart';

class StartPage extends React.Component {

  render() {
    return (
      <main className="page-main">
        <ScApplicationHeader title="Advanced Sitecore Healthcheck 3.0" subTitle="Home Page" />
        <div className="page-content-section">
          <div className="page-content">
            <div className="p-4">
              
              <div className="row">
                <ComponentChart />
                <CpuChart />
                <MemoryChart />
                <ErrorChart />
              </div>

              <div className="row">
                <UpcomingExpirations />
                <ErrorStats />
                <ActiveUserStats />
              </div>

              <div className="row">
                <CacheChart />
                <DataFolderStats />
                <IndexStats />
                <DriveInfoChart />
              </div>
            </div>
          </div>
        </div>
      </main >

    );
  }
}

export default StartPage;
