import './App.css';
import ScGlobalHeader from './ScComponents/ScGlobalHeader';
import React from 'react';
import { Routes, Route } from 'react-router-dom';
import StartPage from './Pages/StartPage'
import ComponentDetailPage from './Pages/ComponentDetailPage';
import { BrowserRouter as Router } from 'react-router-dom';
import ComponentsPage from './Pages/ComponentsPage';
import SettingsPage from './Pages/SettingsPage';
import NotFoundPage from './Pages/NotFoundPage';
import NavigationBar from './Components/SharedComponents/NavigationBar';
class App extends React.Component {

  render() {
    return (

      <div className="page">
        <div className="page-header">
          <ScGlobalHeader applicationTitle="Sitecore Healthcheck" />
        </div>
        <div className="page-app">
          <Router>

            <NavigationBar />

            <Routes basename="/sitecore/shell/client/Applications/healthcheck/">
              <Route path="/" element={<StartPage />} />
              {/* <Route path="components/:id" element={<ComponentDetailPage/>} /> */}
              <Route path="components/:id" element={<ComponentDetailPage/>} />
              <Route path="components" element={<ComponentsPage />} />
              <Route path="settings" element={<SettingsPage />} />
              <Route path="*" element={<NotFoundPage />} />
            </Routes>
          </Router>
        </div>
      </div>
    );
  }
}

export default App;
