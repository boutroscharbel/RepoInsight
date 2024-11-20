// src/authConfig.js
import { LogLevel } from "@azure/msal-browser";

export const msalConfig = {
  auth: {
    clientId: "189f19ec-4ebb-4ebb-80d7-ca5fa4b3a3e3",
    authority: "https://login.microsoftonline.com/3d5791ba-c440-4efc-bd0c-f753e8d4947f",
    redirectUri: "http://localhost:3000",
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
        }
      },
    },
  },
};

export const loginRequest = {
  scopes: ["api://189f19ec-4ebb-4ebb-80d7-ca5fa4b3a3e3/RepoInsightAPI"],
};