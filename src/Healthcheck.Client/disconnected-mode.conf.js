const mockServerPort = 3000;

// Boot up a Sitecore mock server
// We have configured https://github.com/LingyuCoder/express-mock-middleware but feel free to change this.
const express = require('express');
const mockMiddleware = require('express-mock-middleware');
const app = express();
app.use(mockMiddleware({ glob: 'mock/**/*.js' }));
app.listen(mockServerPort, function (err) {
  return console.log(err);
});

// Angular-CLI proxy config
// Check https://webpack.js.org/configuration/dev-server/#devserver-proxy for options available
const PROXY_CONFIG = {
  '/sitecore/': {
    'target': 'http://localhost:' + mockServerPort,
    'secure': false
  }
}

module.exports = PROXY_CONFIG;
