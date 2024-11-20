import React, { useEffect } from 'react';
import LetterFrequencyChart from './LetterFrequencyChart';
import { useMsal, AuthenticatedTemplate, UnauthenticatedTemplate } from "@azure/msal-react";
import { loginRequest } from "./authConfig";
import './App.css';

const App = () => {
  const { instance } = useMsal();

  useEffect(() => {
    instance.handleRedirectPromise().then((response) => {
      if (response) {
        instance.setActiveAccount(response.account);
      }
    }).catch(e => {
      console.error(e);
    });
  }, [instance]);

  const handleLogin = () => {
    instance.loginPopup(loginRequest).then((response) => {
      instance.setActiveAccount(response.account);
    }).catch(e => {
      console.error(e);
    });
  };

  const handleLogout = () => {
    instance.logoutPopup().catch(e => {
      console.error(e);
    });
  };

  return (
    <div className="App">
      <AuthenticatedTemplate>
        <LetterFrequencyChart />
        <button onClick={handleLogout}>Logout</button>
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
        <div className="center-content">
          <p>You are not logged in. Please log in.</p>
          <button onClick={handleLogin}>Login</button>
        </div>
      </UnauthenticatedTemplate>
    </div>
  );
};

export default App;