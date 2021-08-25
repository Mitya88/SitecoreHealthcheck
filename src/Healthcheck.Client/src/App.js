import './App.css';
import ScGlobalHeader from './ScComponents/ScGlobalHeader';
import React from 'react';
import { Routes, Route, Switch } from 'react-router-dom';
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
            <Switch>
              <Route exact path="/sitecore/shell/client/Applications/healthcheck/">
                <StartPage />
              </Route>
              <Route path="/sitecore/shell/client/Applications/healthcheck/components">
                <ComponentsPage />
              </Route>
              <Route path={'/sitecore/shell/client/Applications/healthcheck/detail/:id'}>
                <ComponentDetailPage />
              </Route>
              <Route path="*">
                <NotFoundPage />
              </Route>
            </Switch>
          </Router>

        </div>
      </div>
    );
  }
}

export default App;
